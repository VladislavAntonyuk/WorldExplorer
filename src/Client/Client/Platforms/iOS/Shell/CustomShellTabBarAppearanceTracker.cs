﻿namespace Client;

using CoreAnimation;
using CoreGraphics;
using Microsoft.Maui.Controls.Platform.Compatibility;
using UIKit;

internal class CustomShellTabBarAppearanceTracker : ShellTabBarAppearanceTracker
{
	public override void UpdateLayout(UITabBarController controller)
	{
		base.UpdateLayout(controller);
		const int bottomSpace = 50;
		const int margin = 30;
		controller.TabBar.Frame = new CGRect(controller.TabBar.Frame.X + margin,
		                                     controller.TabBar.Frame.Y - bottomSpace,
		                                     controller.TabBar.Frame.Width - (2 * margin),
		                                     controller.TabBar.Frame.Height);

		const int cornerRadius = 30;
		var uIBezierPath = UIBezierPath.FromRoundedRect(controller.TabBar.Bounds, UIRectCorner.AllCorners,
		                                                new CGSize(cornerRadius, cornerRadius));

		var cAShapeLayer = new CAShapeLayer
		{
			Frame = controller.TabBar.Bounds,
			Path = uIBezierPath.CGPath
		};
		controller.TabBar.Layer.Mask = cAShapeLayer;
	}
}