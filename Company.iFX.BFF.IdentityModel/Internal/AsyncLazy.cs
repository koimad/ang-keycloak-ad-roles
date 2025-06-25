namespace Company.iFX.BFF.IdentityModel.Internal;

internal class AsyncLazy<T> : Lazy<Task<T>>
{
	public AsyncLazy(Func<Task<T>> taskFactory) : base(() => GetTaskAsync(taskFactory).Unwrap())
	{ }

	private static async Task<Task<T>> GetTaskAsync(Func<Task<T>> taskFactory)
	{
		if (TaskHelpers.CanFactoryStartNew)
		{
			return Task<Task<T>>.Factory.StartNew(taskFactory).Unwrap();
		}
		else
		{
			await Task.Yield();

			return taskFactory();
		}
	}
    
}