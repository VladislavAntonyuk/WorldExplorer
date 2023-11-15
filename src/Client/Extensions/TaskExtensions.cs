namespace Client.Extensions;

public static class TaskExtensions
{
	public static async void AndForget(this Task task,
		bool ignoreExceptions = false,
		Func<Exception, Task>? exceptionHandler = null)
	{
		try
		{
			await task;
		}
		catch (Exception ex)
		{
			if (ignoreExceptions || exceptionHandler is null)
			{
				return;
			}

			await exceptionHandler(ex);
		}
	}

	public static async Task<T> AndSafe<T>(this Task<T> task, T defaultValue)
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