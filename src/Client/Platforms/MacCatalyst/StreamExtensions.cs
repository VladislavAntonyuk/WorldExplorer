namespace Client;

using Foundation;
using UIKit;

public static class StreamExtensions
{
	public static Task SaveAsImage(this Stream stream, Func<string, Task> onError)
	{
		var imageData = new UIImage(NSData.FromStream(stream));
		imageData.SaveToPhotosAlbum(async (image, error) =>
		{
			if (error != null)
			{
				await onError(error.ToString());
			}
		});

		return Task.CompletedTask;
	}
}