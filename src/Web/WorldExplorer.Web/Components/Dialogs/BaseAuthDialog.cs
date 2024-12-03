namespace WorldExplorer.Web.Components.Dialogs;

using Microsoft.AspNetCore.Components;
using MudBlazor;

public abstract class BaseAuthDialog : WorldExplorerAuthBaseComponent
{
	[CascadingParameter]
	protected IMudDialogInstance? MudDialog { get; set; }
}