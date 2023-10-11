namespace WebApp.Components.Dialogs;

using Microsoft.AspNetCore.Components;
using Shared.Models;

public partial class PlaceDetailsDialog : BaseDialog
{
	[Parameter]
	public required Place Place { get; set; }
}