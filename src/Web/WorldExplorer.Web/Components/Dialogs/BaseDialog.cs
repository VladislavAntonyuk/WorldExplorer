﻿namespace WorldExplorer.Web.Components.Dialogs;

using Microsoft.AspNetCore.Components;
using MudBlazor;

public abstract class BaseDialog : WorldExplorerBaseComponent
{
	[CascadingParameter]
	protected IMudDialogInstance? MudDialog { get; set; }
}