namespace Client;

using Android.Content;
using Android.Net;
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
			var mediaScanIntent = new Intent(Intent.ActionMediaScannerScanFile);
			mediaScanIntent.SetData(Uri.FromFile(new Java.IO.File(path)));
			Platform.CurrentActivity?.SendBroadcast(mediaScanIntent);
		}
		catch (Exception ex)
		{
			await onError(ex.Message);
		}
	}
}