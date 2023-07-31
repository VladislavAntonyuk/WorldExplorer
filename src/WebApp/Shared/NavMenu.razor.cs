namespace WebApp.Shared;

using System.Reflection;
using System.Runtime.Versioning;

public partial class NavMenu : WorldExplorerBaseComponent
{
	private string? frameworkName = string.Empty;

	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		frameworkName = Assembly.GetEntryAssembly()?.GetCustomAttribute<TargetFrameworkAttribute>()?.FrameworkName;
	}
}