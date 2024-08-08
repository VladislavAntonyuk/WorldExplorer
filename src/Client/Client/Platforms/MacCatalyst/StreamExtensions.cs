namespace Client;

using Foundation;
using UIKit;

public static class StreamExtensions
{
	public static async Task SaveAsImage(this Stream stream, Func<string, Task> onError)
	{
		var nsData = NSData.FromStream(stream);
		if (nsData is null)
		{
			await onError("Unable to read data from stream");
			return;
		}

		var imageData = new UIImage(nsData);
		imageData.SaveToPhotosAlbum(async (image, error) =>
		{
			if (error != null)
			{
				await onError(error.ToString());
			}
		});
	}
}