namespace Client;

using Android.Opengl;
using Android.Widget;
using Controls;
using Google.AR.Core;
using Microsoft.Maui.Handlers;

public class ArViewHandler(IPropertyMapper? mapper, CommandMapper? commandMapper) : ViewHandler<IArView, GLSurfaceView>(mapper ?? ArViewMapper, commandMapper ?? ArViewCommandMapper)
{
	private ArRenderer? arRenderer;
	public static readonly IPropertyMapper<IArView, ArViewHandler> ArViewMapper =
		new PropertyMapper<IArView, ArViewHandler>(ViewMapper)
		{
			[nameof(IArView.Images)] = MapImages
		};

	public static readonly CommandMapper<IArView, ArViewHandler> ArViewCommandMapper = new(ViewCommandMapper);

	public ArViewHandler() : this(ArViewMapper, ArViewCommandMapper)
	{
	}

	private static void MapImages(ArViewHandler handler, IArView view)
	{
		handler.arRenderer?.AddImages(view.Images);
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

		// Set up renderer.
		arRenderer = new ArRenderer(Context);
		PlatformView.SetRenderer(arRenderer);
		PlatformView.RenderMode = Rendermode.Continuously;
		PlatformView.OnResume();
	}

	protected override void DisconnectHandler(GLSurfaceView platformView)
	{
		platformView.Dispose();
		base.DisconnectHandler(platformView);
	}
}