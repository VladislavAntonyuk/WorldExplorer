namespace Client;

using ARKit;
using Controls;
using Extensions;
using Foundation;
using Microsoft.Maui.Handlers;
using SceneKit;

public class ArViewHandler(IPropertyMapper? mapper, CommandMapper? commandMapper) : ViewHandler<IArView, ARSCNView>(
	mapper ?? ArViewMapper, commandMapper ?? ArViewCommandMapper)
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
	private readonly List<ImageNode> imagePlaneNodes = [];

	public ArViewHandler() : this(ArViewMapper, ArViewCommandMapper)
	{
	}

	protected override ARSCNView CreatePlatformView()
	{
		return new ARSCNView
		{
			AutoenablesDefaultLighting = true,
			Session = new ARSession(),
			Scene = []
		};
	}

	protected override async void ConnectHandler(ARSCNView platformView)
	{
		base.ConnectHandler(platformView);
		var cameraPermissionStatus = await Permissions.RequestAsync<Permissions.Camera>();
		if (cameraPermissionStatus != PermissionStatus.Granted)
		{
			return;
		}

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

	protected override void DisconnectHandler(ARSCNView platformView)
	{
		isReady = false;
		platformView.Session.Pause();
		platformView.Session.Dispose();
		platformView.Dispose();
		base.DisconnectHandler(platformView);
	}

	private static void MapImages(ArViewHandler handler, IArView view)
	{
		if (!handler.isReady)
		{
			return;
		}

		handler.imagePlaneNodes.Clear();
		handler.PlatformView.Scene.RootNode.EnumerateChildNodes((SCNNode node, out bool stop) =>
		{
			stop = false;
			node.RemoveFromParentNode();
		});
		BuildImagePlaceholders(handler.PlatformView.Scene.RootNode, handler.imagePlaneNodes);
		int i = 0;
		foreach (var imageUrl in view.Images)
		{
			Task.Run(() =>
				{
					int currentIndex = Interlocked.Increment(ref i) - 1;
					if (currentIndex < handler.imagePlaneNodes.Count)
					{
						var data = NSData.FromUrl(new NSUrl(imageUrl));
						if (data.Length == 0)
						{
							Interlocked.Decrement(ref i);
							return;
						}

						handler.imagePlaneNodes[currentIndex].UpdateImage(data);
					}
				})
				.AndForget(false, _ =>
				{
					Interlocked.Decrement(ref i);
					return Task.CompletedTask;
				});
		}
	}


	private static void BuildImagePlaceholders(SCNNode rootNode, List<ImageNode> imagePlaneNodes)
	{
		var centerNode = new SCNNode
		{
			Position = new SCNVector3(0, 0, 0)
		};
		rootNode.AddChildNode(centerNode);
		const float radius = 1.25f; // 1.25m away from world origin
		imagePlaneNodes.AddRange(AddBlankRow(rootNode, centerNode, (ImageHeight * 3) + (VerticalMargin * 3), radius - 0.15f));
		imagePlaneNodes.AddRange(AddBlankRow(rootNode, centerNode, (ImageHeight * 2) + (VerticalMargin * 2), radius - 0.075f));
		imagePlaneNodes.AddRange(AddBlankRow(rootNode, centerNode, ImageHeight + VerticalMargin, radius));
		imagePlaneNodes.AddRange(AddBlankRow(rootNode, centerNode, 0, radius));
		imagePlaneNodes.AddRange(AddBlankRow(rootNode, centerNode, 0 - ImageHeight - VerticalMargin, radius));
		imagePlaneNodes.AddRange(AddBlankRow(rootNode, centerNode, 0 - (ImageHeight * 2) - (VerticalMargin * 2), radius - 0.075f));
		imagePlaneNodes.AddRange(AddBlankRow(rootNode, centerNode, 0 - (ImageHeight * 3) - (VerticalMargin * 3), radius - 0.15f));
	}

	private static IEnumerable<ImageNode> AddBlankRow(SCNNode rootNode, SCNNode centerNode, float y, double radius)
	{
		const int imagesPerRow = 10;
		for (var i = 0; i < imagesPerRow; i++)
		{
			var imagePlaneNode = new ImageNode(ImageWidth, ImageHeight);

			var x = (float)(radius * Math.Cos(2 * Math.PI * i / imagesPerRow));
			var z = (float)(radius * Math.Sin(2 * Math.PI * i / imagesPerRow));

			imagePlaneNode.Position = new SCNVector3(x, y, z);

			var lookConstraint = SCNLookAtConstraint.Create(centerNode);
			lookConstraint.GimbalLockEnabled = true;
			imagePlaneNode.Constraints = [lookConstraint];

			rootNode.AddChildNode(imagePlaneNode);
			yield return imagePlaneNode;
		}
	}
}