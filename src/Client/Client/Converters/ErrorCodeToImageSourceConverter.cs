namespace Client.Converters;

using System.Globalization;
using CommunityToolkit.Maui.Converters;
using Models.Enums;
using SkiaSharp.Extended.UI.Controls;

[AcceptEmptyServiceProvider]
public partial class ErrorCodeToImageSourceConverter : BaseConverterOneWay<ErrorCode?, SKLottieImageSource>
{
	public override SKLottieImageSource DefaultConvertReturnValue { get; set; } = new SKFileLottieImageSource
	{
		File = "ErrorCodes/Unknown.json"
	};

	public override SKLottieImageSource ConvertFrom(ErrorCode? value, CultureInfo? culture)
	{
		return value.HasValue
			? new SKFileLottieImageSource
			{
				File = $"ErrorCodes/{value}.json"
			}
			: DefaultConvertReturnValue;
	}
}