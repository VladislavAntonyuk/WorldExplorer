namespace Client;

using Foundation;
using SceneKit;
using UIKit;

public sealed class ImageNode : SCNNode
{
	public ImageNode(float width, float height)
	{
		Geometry = CreateGeometry(width, height);
	}

	private static SCNPlane CreateGeometry(float width, float height)
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

	internal void UpdateImage(NSData uIImage)
	{
		if (Geometry?.FirstMaterial is not null)
		{
			Geometry.FirstMaterial.Diffuse.Contents = uIImage;
		}
	}

	protected override void Dispose(bool disposing)
	{
		Geometry?.FirstMaterial?.Diffuse.Contents?.Dispose();
		Geometry?.Dispose();
		base.Dispose(disposing);
	}
}