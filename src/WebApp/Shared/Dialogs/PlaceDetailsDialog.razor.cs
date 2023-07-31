namespace WebApp.Shared.Dialogs;

using global::Shared.Models;
using Microsoft.AspNetCore.Components;

public partial class PlaceDetailsDialog : BaseDialog
{
	[Parameter]
	public Place? Place { get; set; }
}