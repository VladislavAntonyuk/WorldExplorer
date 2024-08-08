namespace WorldExplorer.Web.Components.Dialogs;

using Microsoft.AspNetCore.Components;
using MudBlazor;
using WorldExplorer.Web.Components;

public abstract class BaseDialog : WorldExplorerBaseComponent
{
	protected bool IsBusy { get; set; }

	[CascadingParameter]
	protected MudDialogInstance? MudDialog { get; set; }
}