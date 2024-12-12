namespace Client;

using Android.Webkit;
using Microsoft.Maui.Handlers;
using Microsoft.Maui.Platform;

public class WorldExplorerMapWebViewClient(HybridWebViewHandler handler) : MauiHybridWebViewClient(handler)
{
	public override bool ShouldOverrideUrlLoading(WebView? view, IWebResourceRequest? request)
	{
		if (request?.Url?.Host == "0.0.0.1" && OperatingSystem.IsAndroidVersionAtLeast(24))
		{
			return base.ShouldOverrideUrlLoading(view, request);
		}

		return true;
	}
}