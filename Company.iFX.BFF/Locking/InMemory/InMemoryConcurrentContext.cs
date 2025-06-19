using Microsoft.AspNetCore.Http;

namespace Company.iFX.BFF.Locking.InMemory;

internal class InMemoryConcurrentContext : IConcurrentContext
{
    #region Methods

    #region Public

    public async Task ExecuteOncePerSession(ISession session, String identifier, Func<Boolean> actionRequired, Func<Task> @delegate)
    {
        String cacheKey = $"{typeof(InMemoryConcurrentContext).FullName}+{session.Id}+{identifier}";

        SemaphoreSlim semaphore = Semaphores.GetInstance(cacheKey);

        try
        {
            if (!actionRequired())
            {
                return;
            }

            Boolean acquired = await semaphore.WaitAsync(TimeSpan.FromSeconds(15));

            if (acquired && actionRequired())
            {
                await @delegate();
            }
        }
        finally
        {
            semaphore.Release();
            Semaphores.RemoveInstance(cacheKey);
        }
    }

    #endregion

    #endregion
}