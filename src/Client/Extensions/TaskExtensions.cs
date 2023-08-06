namespace Client.Extensions;

public static class TaskExtensions
{
	public static async void AndForget(this Task task, bool ignoreExceptions = false, Func<Exception, Task>? exceptionHandler = null)
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
}