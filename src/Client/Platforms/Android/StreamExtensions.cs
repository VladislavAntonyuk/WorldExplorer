namespace Client;

using Android.OS;

public static class StreamExtensions
{
	public static async Task SaveAsImage(this Stream stream, Func<string, Task> onError)
	{
		try
		{
			var memoryStream = new MemoryStream();
			await stream.CopyToAsync(memoryStream);
			var storagePath = Environment.GetExternalStoragePublicDirectory(Environment.DirectoryPictures);
			if (storagePath is null)
			{
				return;
			}

			var path = Path.Combine(storagePath.ToString(), Path.GetRandomFileName());
			await File.WriteAllBytesAsync(path, memoryStream.ToArray());
		}
		catch (Exception ex)
		{
			await onError(ex.Message);
		}
	}
}