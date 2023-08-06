namespace Client;

public static class StreamExtensions
{
	public static async Task SaveAsImage(this Stream stream, Func<string, Task> onError)
	{
		try
		{
			
		}
		catch (Exception e)
		{
			await onError(e.Message);
		}
	}
}