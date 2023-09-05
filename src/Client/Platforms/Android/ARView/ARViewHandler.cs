namespace Client;

using System.Collections.Concurrent;
using Android.Content;
using Android.Graphics;
using Android.Opengl;
using Android.Util;
using Android.Views;
using Android.Widget;
using Google.Android.Material.Snackbar;
using Google.AR.Core;
using Java.IO;
using Java.Lang;
using Javax.Microedition.Khronos.Opengles;
using Microsoft.Maui.Handlers;
using Renderings;
using Config = Google.AR.Core.Config;
using EGLConfig = Javax.Microedition.Khronos.Egl.EGLConfig;
using Exception = System.Exception;

public class ArViewHandler : ViewHandler<IArView, GLSurfaceView>
{
	public static readonly IPropertyMapper<IArView, ArViewHandler> ArViewMapper =
		new PropertyMapper<IArView, ArViewHandler>(ViewMapper)
		{
			[nameof(IArView.Images)] = MapImages
		};

	public static readonly CommandMapper<IArView, ArViewHandler> ArViewCommandMapper = new(ViewCommandMapper);
	private Session? session;

	public ArViewHandler(IPropertyMapper? mapper, CommandMapper? commandMapper) : base(
		mapper ?? ArViewMapper, commandMapper ?? ArViewCommandMapper)
	{
	}

	public ArViewHandler() : this(ArViewMapper, ArViewCommandMapper)
	{
	}

	private static void MapImages(ArViewHandler handler, IArView view)
	{
		// map images
	}

	protected override GLSurfaceView CreatePlatformView()
	{
		var view = new GLSurfaceView(Context);
		return view;
	}

	protected override async void ConnectHandler(GLSurfaceView platformView)
	{
		base.ConnectHandler(platformView);
		if (!ArCoreApk.Instance.CheckAvailability(Context).IsSupported)
		{
			return;
		}

		var result = ArCoreApk.Instance.RequestInstall(Platform.CurrentActivity, true,
													   ArCoreApk.InstallBehavior.Required,
													   ArCoreApk.UserMessageType.Application);
		if (result != ArCoreApk.InstallStatus.Installed)
		{
			return;
		}

		var cameraPermissionStatus = await Permissions.RequestAsync<Permissions.Camera>();
		if (cameraPermissionStatus == PermissionStatus.Granted)
		{
			// Create default config, check is supported, create session from that config.
			session = new Session(Context);
			var config = new Config(session);
			session.Configure(config);

			PlatformView.SetOnTouchListener(new ArTouchListener(Context));

			// Set up renderer.
			PlatformView.PreserveEGLContextOnPause = true;
			PlatformView.SetEGLContextClientVersion(2);
			PlatformView.SetEGLConfigChooser(8, 8, 8, 8, 16, 0); // Alpha used for plane blending.
			PlatformView.SetRenderer(new ArRenderer(Context, session));
			PlatformView.RenderMode = Rendermode.Continuously;
			session.Resume();
			PlatformView.OnResume();
		}
	}

	protected override void DisconnectHandler(GLSurfaceView platformView)
	{
		platformView.OnPause();
		session?.Pause();
		session?.Close();
		base.DisconnectHandler(platformView);
	}
}

public class ArRenderer : Object, GLSurfaceView.IRenderer
{
	private static readonly float[] MAnchorMatrix = new float[16];
	private readonly Context context;

	private readonly List<Anchor> mAnchors = new();

	private readonly BackgroundRenderer mBackgroundRenderer = new();
	private readonly PlaneRenderer mPlaneRenderer = new();
	private readonly PointCloudRenderer mPointCloud = new();
	private readonly ObjectRenderer mVirtualObject = new();
	private readonly ObjectRenderer mVirtualObjectShadow = new();
	private readonly Session session;

	private Snackbar? mLoadingMessageSnackbar;

	public ArRenderer(Context context, Session session)
	{
		this.context = context;
		this.session = session;
		MainThread.BeginInvokeOnMainThread(() =>
		{
			ArgumentNullException.ThrowIfNull(Platform.CurrentActivity?.Window?.DecorView);
			mLoadingMessageSnackbar =
				Snackbar.Make(
					Platform.CurrentActivity.Window.DecorView, "Searching for surfaces...",
					BaseTransientBottomBar.LengthIndefinite);
			mLoadingMessageSnackbar.View.SetBackgroundColor(Color.DarkGray);
			mLoadingMessageSnackbar.Show();
		});
	}

	public void OnDrawFrame(IGL10? gl)
	{
		// Clear screen to notify driver it should not load any pixels from previous frame.
		GLES20.GlClear(GLES20.GlColorBufferBit | GLES20.GlDepthBufferBit);

		try
		{
			// Obtain the current frame from ARSession. When the configuration is set to
			// UpdateMode.BLOCKING (it is by default), this will throttle the rendering to the
			// camera framerate.
			var frame = session.Update();
			var camera = frame.Camera;

			// Handle taps. Handling only one tap per frame, as taps are usually low frequency
			// compared to frame rate.
			ArTouchListener.MQueuedSingleTaps.TryDequeue(out var tap);

			if (tap != null && camera.TrackingState == TrackingState.Tracking)
			{
				foreach (var hit in frame.HitTest(tap))
				{
					var trackable = hit.Trackable;

					// Check if any plane was hit, and if it was hit inside the plane polygon.
					if (trackable is Plane plane && plane.IsPoseInPolygon(hit.HitPose))
					{
						// Cap the number of objects created. This avoids overloading both the
						// rendering system and ARCore.
						if (mAnchors.Count >= 16)
						{
							mAnchors[0].Detach();
							mAnchors.RemoveAt(0);
						}

						// Adding an Anchor tells ARCore that it should track this position in
						// space.  This anchor is created on the Plane to place the 3d model
						// in the correct position relative to both the world and to the plane
						mAnchors.Add(hit.CreateAnchor());

						// Hits are sorted by depth. Consider only closest hit on a plane.
						break;
					}
				}
			}

			// Draw background.
			mBackgroundRenderer.Draw(frame);

			// If not tracking, don't draw 3d objects.
			if (camera.TrackingState == TrackingState.Paused)
			{
				return;
			}

			// Get projection matrix.
			var projmtx = new float[16];
			camera.GetProjectionMatrix(projmtx, 0, 0.1f, 100.0f);

			// Get camera matrix and draw.
			var viewmtx = new float[16];
			camera.GetViewMatrix(viewmtx, 0);

			// Compute lighting from average intensity of the image.
			var lightIntensity = frame.LightEstimate.PixelIntensity;

			// Visualize tracked points.
			var pointCloud = frame.AcquirePointCloud();
			mPointCloud.Update(pointCloud);
			mPointCloud.Draw(camera.DisplayOrientedPose, viewmtx, projmtx);

			// App is repsonsible for releasing point cloud resources after using it
			pointCloud.Release();

			var planes = new List<Plane>();
			foreach (var p in session.GetAllTrackables(Class.FromType(typeof(Plane))))
			{
				var plane = (Plane)p;
				planes.Add(plane);
			}

			// Check if we detected at least one plane. If so, hide the loading message.
			if (mLoadingMessageSnackbar != null)
			{
				foreach (var plane in planes)
				{
					if (plane.GetType() == Plane.Type.HorizontalUpwardFacing &&
						plane.TrackingState == TrackingState.Tracking)
					{
						HideLoadingMessage();
						break;
					}
				}
			}

			// Visualize planes.
			mPlaneRenderer.DrawPlanes(planes, camera.DisplayOrientedPose, projmtx);

			// Visualize anchors created by touch.
			var scaleFactor = 1.0f;
			foreach (var anchor in mAnchors)
			{
				if (anchor.TrackingState != TrackingState.Tracking)
				{
					continue;
				}

				// Get the current combined pose of an Anchor and Plane in world space. The Anchor
				// and Plane poses are updated during calls to session.update() as ARCore refines
				// its estimate of the world.
				anchor.Pose.ToMatrix(MAnchorMatrix, 0);

				// Update and draw the model and its shadow.
				mVirtualObject.UpdateModelMatrix(MAnchorMatrix, scaleFactor);
				mVirtualObjectShadow.UpdateModelMatrix(MAnchorMatrix, scaleFactor);
				mVirtualObject.Draw(viewmtx, projmtx, lightIntensity);
				mVirtualObjectShadow.Draw(viewmtx, projmtx, lightIntensity);
			}
		}
		catch (Exception ex)
		{
			// Avoid crashing the application due to unhandled exceptions.
			Log.Error("TAG", "Exception on the OpenGL thread", ex);
		}
	}

	public void OnSurfaceChanged(IGL10? gl, int width, int height)
	{
		var rotation = (int)DeviceDisplay.MainDisplayInfo.Rotation - 1;
		session.SetDisplayGeometry(rotation, width, height);
		GLES20.GlViewport(0, 0, width, height);
	}

	public void OnSurfaceCreated(IGL10? gl, EGLConfig? config)
	{
		GLES20.GlClearColor(0.1f, 0.1f, 0.1f, 1.0f);

		// Create the texture and pass it to ARCore session to be filled during update().
		mBackgroundRenderer.CreateOnGlThread(context);
		session.SetCameraTextureName(mBackgroundRenderer.TextureId);

		// Prepare the other rendering objects.
		try
		{
			mVirtualObject.CreateOnGlThread(context, "andy.obj", "andy.png");
			mVirtualObject.SetMaterialProperties(0.0f, 3.5f, 1.0f, 6.0f);

			mVirtualObjectShadow.CreateOnGlThread(context, "andy_shadow.obj", "andy_shadow.png");
			mVirtualObjectShadow.SetBlendMode(ObjectRenderer.BlendMode.Shadow);
			mVirtualObjectShadow.SetMaterialProperties(1.0f, 0.0f, 0.0f, 1.0f);
		}
		catch (IOException)
		{
			MainThread.BeginInvokeOnMainThread(() =>
			{
				Toast.MakeText(context, "Failed to read obj file", ToastLength.Long)?.Show();
			});
			Log.Error("TAG", "Failed to read obj file");
		}

		try
		{
			mPlaneRenderer.CreateOnGlThread(context, "trigrid.png");
		}
		catch (IOException)
		{
			Log.Error("TAG", "Failed to read plane texture");
		}

		mPointCloud.CreateOnGlThread(context);
	}

	private void HideLoadingMessage()
	{
		MainThread.BeginInvokeOnMainThread(() =>
		{
			mLoadingMessageSnackbar?.Dismiss();
			mLoadingMessageSnackbar = null;
		});
	}
}

public class ArTouchListener : Object, View.IOnTouchListener
{
	public static readonly ConcurrentQueue<MotionEvent> MQueuedSingleTaps = new();
	private readonly GestureDetector mGestureDetector;

	public ArTouchListener(Context context)
	{
		mGestureDetector = new GestureDetector(context, new SimpleTapGestureDetector
		{
			SingleTapUpHandler = arg =>
			{
				OnSingleTap(arg);
				return true;
			},
			DownHandler = _ => true
		});
	}

	public bool OnTouch(View? v, MotionEvent? e)
	{
		if (e is null)
		{
			return false;
		}

		return mGestureDetector.OnTouchEvent(e);
	}

	private void OnSingleTap(MotionEvent e)
	{
		// Queue tap if there is space. Tap is lost if queue is full.
		if (MQueuedSingleTaps.Count < 16)
		{
			MQueuedSingleTaps.Enqueue(e);
		}
	}
}

internal class SimpleTapGestureDetector : GestureDetector.SimpleOnGestureListener
{
	public Func<MotionEvent, bool>? SingleTapUpHandler { get; set; }

	public Func<MotionEvent, bool>? DownHandler { get; set; }

	public override bool OnSingleTapUp(MotionEvent e)
	{
		return SingleTapUpHandler?.Invoke(e) ?? false;
	}

	public override bool OnDown(MotionEvent e)
	{
		return DownHandler?.Invoke(e) ?? false;
	}
}