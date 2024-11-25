namespace Client.Converters;

using System.Globalization;
using CommunityToolkit.Maui.Converters;

[AcceptEmptyServiceProvider]
public sealed class WorldExplorerImageSourceConverter : BaseConverterOneWay<string?, ImageSource>
{
	public override ImageSource ConvertFrom(string? value, CultureInfo? culture)
	{
		if (value?.StartsWith("data:image;base64,") == true)
		{
			return ImageSource.FromStream(() => new MemoryStream(Convert.FromBase64String(value.Replace("data:image;base64,", null))));
		}

		if (Uri.TryCreate(value, UriKind.RelativeOrAbsolute, out var uri))
		{
			return ImageSource.FromUri(uri);
		}

		return DefaultConvertReturnValue;
	}

	public override ImageSource DefaultConvertReturnValue { get; set; } = ImageSource.FromUri(new Uri("https://placehold.co/300?text=Loading..."));
}