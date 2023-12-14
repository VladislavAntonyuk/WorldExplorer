namespace Shared.Extensions;

public static class TaskExtensions
{
	public static async Task<T> Safe<T>(this Task<T> task, T defaultValue)
	{
		try
		{
			return await task;
		}
		catch
		{
			return defaultValue;
		}
	}
}