namespace Client;

using SceneKit;
using UIKit;

public sealed class ImagePlaneNode : SCNNode
{
	public ImagePlaneNode(float width, float height)
	{
		Geometry = CreateGeometry(width, height);
		Opacity = 0.2f;
	}

	private static SCNGeometry CreateGeometry(float width, float height)
	{
		var material = new SCNMaterial();
		material.Diffuse.Contents = UIColor.White;
		material.DoubleSided = true;

		var geometry = SCNPlane.Create(width, height);
		geometry.Materials = new[]
		{
			material
		};

		return geometry;
	}

	internal void UpdateImage(UIImage uIImage)
	{
		if (Geometry?.FirstMaterial is not null)
		{
			Geometry.FirstMaterial.Diffuse.Contents = uIImage;
			RunAction(SCNAction.FadeIn(1));
		}
	}
}