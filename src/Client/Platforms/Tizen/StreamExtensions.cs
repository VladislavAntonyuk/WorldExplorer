namespace Client;

public static class StreamExtensions
{
	public static async Task SaveAsImage(this Stream stream, Func<string, Task> onError)
	{
		try
		{
			var memoryStream = new MemoryStream();
			await stream.CopyToAsync(memoryStream);
			await File.WriteAllBytesAsync(
				Environment.GetFolderPath(Environment.SpecialFolder.MyPictures, Environment.SpecialFolderOption.Create),
				memoryStream.ToArray());
		}
		catch (Exception e)
		{
			await onError(e.Message);
		}
	}
}