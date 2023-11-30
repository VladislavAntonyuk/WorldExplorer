namespace Client;

using Android.Opengl;
using Android.Widget;
using Controls;
using Google.AR.Core;
using Microsoft.Maui.Handlers;
using Config = Google.AR.Core.Config;

public class ArViewHandler(IPropertyMapper? mapper, CommandMapper? commandMapper) : ViewHandler<IArView, GLSurfaceView>(mapper ?? ArViewMapper, commandMapper ?? ArViewCommandMapper)
{
	public static readonly IPropertyMapper<IArView, ArViewHandler> ArViewMapper =
		new PropertyMapper<IArView, ArViewHandler>(ViewMapper)
		{
			[nameof(IArView.Images)] = MapImages
		};

	public static readonly CommandMapper<IArView, ArViewHandler> ArViewCommandMapper = new(ViewCommandMapper);
	private Session? session;
	private ArRenderer? arRenderer;
	bool isInitialized;

	public ArViewHandler() : this(ArViewMapper, ArViewCommandMapper)
	{
	}

	private static void MapImages(ArViewHandler handler, IArView view)
	{
		// map images
	}

	protected override GLSurfaceView CreatePlatformView()
	{
		var view = new GLSurfaceView(Context)
		{
			PreserveEGLContextOnPause = true
		};
		view.SetEGLContextClientVersion(2);
		view.SetEGLConfigChooser(8, 8, 8, 8, 16, 0); // Alpha used for plane blending.
		view.SetOnTouchListener(new ArTouchListener(Context));
		return view;
	}

	protected override async void ConnectHandler(GLSurfaceView platformView)
	{
		base.ConnectHandler(platformView);
		if (!ArCoreApk.Instance.CheckAvailability(Context).IsSupported)
		{
			Toast.MakeText(Context, "ARCore is not supported", ToastLength.Long)?.Show();
			return;
		}

		var result = ArCoreApk.Instance.RequestInstall(Platform.CurrentActivity, true,
													   ArCoreApk.InstallBehavior.Required,
													   ArCoreApk.UserMessageType.Application);
		if (result != ArCoreApk.InstallStatus.Installed)
		{
			Toast.MakeText(Context, "ARCore is not installed", ToastLength.Long)?.Show();
			return;
		}

		var cameraPermissionStatus = await Permissions.RequestAsync<Permissions.Camera>();
		if (cameraPermissionStatus != PermissionStatus.Granted)
		{
			Toast.MakeText(Context, "Camera permission is not granted", ToastLength.Long)?.Show();
			return;
		}

		session = new Session(Context);
		var config = new Config(session);
		config.SetUpdateMode(Config.UpdateMode.LatestCameraImage);
		session.Configure(config);

		// Set up renderer.
		arRenderer = new ArRenderer(Context, session);
		PlatformView.SetRenderer(arRenderer);
		PlatformView.RenderMode = Rendermode.Continuously;
		session.Resume();
		PlatformView.OnResume();
		isInitialized = true;
	}

	protected override void DisconnectHandler(GLSurfaceView platformView)
	{
		if (isInitialized)
		{
			session?.Pause();
			session?.Close();
			platformView.OnPause();
			arRenderer?.Dispose();
			session?.Dispose();
			session = null;
			isInitialized = false;
		}

		platformView.Dispose();
		base.DisconnectHandler(platformView);
}
}