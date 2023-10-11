namespace Client;

using ARKit;
using Controls;
using Foundation;
using Microsoft.Maui.Handlers;
using SceneKit;
using UIKit;

public class ArViewHandler(IPropertyMapper? mapper, CommandMapper? commandMapper) : ViewHandler<IArView, ARSCNView>(mapper ?? ArViewMapper, commandMapper ?? ArViewCommandMapper)
{
	private const float ImageHeight = 0.2f;
	private const float ImageWidth = 0.3f;
	private const float VerticalMargin = 0.01f;

	public static readonly IPropertyMapper<IArView, ArViewHandler> ArViewMapper =
		new PropertyMapper<IArView, ArViewHandler>(ViewMapper)
		{
			[nameof(IArView.Images)] = MapImages
		};

	public static readonly CommandMapper<IArView, ArViewHandler> ArViewCommandMapper = new(ViewCommandMapper);
	private bool isReady;

	public ArViewHandler() : this(ArViewMapper, ArViewCommandMapper)
	{
	}

	private static void MapImages(ArViewHandler handler, IArView view)
	{
		if (!handler.isReady)
		{
			return;
		}

		var radius = 1.25f; // 1.25m away from world origin
		var sides = 10; // images per row
		var centerNode = new SCNNode
		{
			Position = new SCNVector3(0, 0, 0)
		};
		handler.PlatformView.Scene.RootNode.AddChildNode(centerNode);

		var imagePlaneNodes = new List<ImagePlaneNode>();

		imagePlaneNodes.AddRange(AddBlankRow(handler.PlatformView.Scene.RootNode, centerNode,
											 (ImageHeight * 3) + (VerticalMargin * 3), radius - 0.15f, sides));
		imagePlaneNodes.AddRange(AddBlankRow(handler.PlatformView.Scene.RootNode, centerNode,
											 (ImageHeight * 2) + (VerticalMargin * 2), radius - 0.075f, sides));
		imagePlaneNodes.AddRange(AddBlankRow(handler.PlatformView.Scene.RootNode, centerNode,
											 ImageHeight + VerticalMargin, radius, sides));
		imagePlaneNodes.AddRange(AddBlankRow(handler.PlatformView.Scene.RootNode, centerNode, 0, radius, sides));
		imagePlaneNodes.AddRange(AddBlankRow(handler.PlatformView.Scene.RootNode, centerNode,
											 0 - ImageHeight - VerticalMargin, radius, sides));
		imagePlaneNodes.AddRange(AddBlankRow(handler.PlatformView.Scene.RootNode, centerNode,
											 0 - (ImageHeight * 2) - (VerticalMargin * 2), radius - 0.075f, sides));
		imagePlaneNodes.AddRange(AddBlankRow(handler.PlatformView.Scene.RootNode, centerNode,
											 0 - (ImageHeight * 3) - (VerticalMargin * 3), radius - 0.15f, sides));


		var images = view.Images.Take(imagePlaneNodes.Count).ToList();

		for (var i = 0; i < images.Count; i++)
		{
			var image = images[i];
			var uiImage = UIImage.LoadFromData(NSData.FromArray(image));
			if (uiImage is null)
			{
				continue;
			}

			imagePlaneNodes[i].UpdateImage(uiImage);
		}
	}

	protected override ARSCNView CreatePlatformView()
	{
		return new ARSCNView
		{
			AutoenablesDefaultLighting = true
		};
	}

	protected override async void ConnectHandler(ARSCNView platformView)
	{
		base.ConnectHandler(platformView);
		var cameraPermissionStatus = await Permissions.RequestAsync<Permissions.Camera>();
		if (cameraPermissionStatus == PermissionStatus.Granted)
		{
			platformView.Session = new ARSession();
			platformView.Scene = new SCNScene();
			platformView.Session.Run(new ARWorldTrackingConfiguration
			{
				AutoFocusEnabled = true,
				PlaneDetection = ARPlaneDetection.Horizontal,
				LightEstimationEnabled = true,
				WorldAlignment = ARWorldAlignment.GravityAndHeading
			}, ARSessionRunOptions.ResetTracking | ARSessionRunOptions.RemoveExistingAnchors);
			isReady = true;
			UpdateValue(nameof(IArView.Images));
		}
	}

	protected override void DisconnectHandler(ARSCNView platformView)
	{
		platformView.Session.Pause();
		platformView.Scene.Dispose();
		platformView.Session.Dispose();
		base.DisconnectHandler(platformView);
	}

	private static IEnumerable<ImagePlaneNode> AddBlankRow(SCNNode rootNode,
		SCNNode centerNode,
		float y,
		double radius,
		int sides)
	{
		for (var i = 0; i < sides; i++)
		{
			var imagePlaneNode = new ImagePlaneNode(ImageWidth, ImageHeight);

			var x = (float)(radius * Math.Cos(2 * Math.PI * i / sides));
			var z = (float)(radius * Math.Sin(2 * Math.PI * i / sides));

			imagePlaneNode.Position = new SCNVector3(x, y, z);

			var lookConstraint = SCNLookAtConstraint.Create(centerNode);
			lookConstraint.GimbalLockEnabled = true;
			imagePlaneNode.Constraints = new SCNConstraint[]
			{
				lookConstraint
			};

			rootNode.AddChildNode(imagePlaneNode);
			yield return imagePlaneNode;
		}
	}
}