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

		var imagePlaneNodes = new List<ImagePlaneNode>();
		BuildImagePlaceholders(handler.PlatformView.Scene.RootNode, imagePlaneNodes);

		int imageNumber = 0;
		foreach (var imageUrl in view.Images)
		{
			if (imageNumber >= imagePlaneNodes.Count - 1)
			{
				break;
			}

			try
			{
				var url = NSUrl.FromString(imageUrl);
				if (url is null)
				{
					continue;
				}

				var uiImage = UIImage.LoadFromData(NSData.FromUrl(url));
				if (uiImage is null)
				{
					continue;
				}

				imagePlaneNodes[imageNumber].UpdateImage(uiImage);
				imageNumber++;
			}
			catch
			{
				// ignore invalid image
			}
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
		if (cameraPermissionStatus != PermissionStatus.Granted)
		{
			return;
		}

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

	protected override void DisconnectHandler(ARSCNView platformView)
	{
		platformView.Session.Pause();
		platformView.Scene.Dispose();
		platformView.Session.Dispose();
		base.DisconnectHandler(platformView);
	}

	private static void BuildImagePlaceholders(SCNNode rootNode, List<ImagePlaneNode> imagePlaneNodes)
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

	private static IEnumerable<ImagePlaneNode> AddBlankRow(SCNNode rootNode,
		SCNNode centerNode,
		float y,
		double radius)
	{
		const int imagesPerRow = 10;
		for (var i = 0; i < imagesPerRow; i++)
		{
			var imagePlaneNode = new ImagePlaneNode(ImageWidth, ImageHeight);

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