namespace Client;

using Android.App;
using Android.Content;
using Android.Provider;

public static class StreamExtensions
{
	public static async Task SaveAsImage(this Stream stream, Func<string, Task> onError)
	{
		try
		{
			var uri = MediaStore.Images.Media.ExternalContentUri;
			if (uri is not null)
			{
				var values = new ContentValues();
				values.Put("_display_name", $"{DateTime.Now}.jpg");
				var content = Application.Context.ContentResolver;
				using var url = content?.Insert(uri, values);
				if (url is not null)
				{
					using var memoryStream = new MemoryStream();
					await using var newStream = content?.OpenOutputStream(url);
					await stream.CopyToAsync(memoryStream);
					if (newStream is not null)
					{
						await newStream.WriteAsync(memoryStream.ToArray(), 0, (int)memoryStream.Length);
					}
				}
			}
		}
		catch (Exception ex)
		{
			await onError(ex.Message);
		}
	}
}