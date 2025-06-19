using Microsoft.AspNetCore.Http;

using RedLockNet;

namespace Company.iFX.BFF.Locking.Distributed.Redis;

public class RedisConcurrentContext(IDistributedLockFactory redisLockFactory) : IConcurrentContext
{
    #region Methods

    #region Public

    public async Task ExecuteOncePerSession(ISession session, String identifier, Func<Boolean> actionRequired, Func<Task> @delegate)
    {
        TimeSpan expiryTime = TimeSpan.FromSeconds(15);
        TimeSpan waitTime = TimeSpan.FromMilliseconds(100);
        TimeSpan retryTime = TimeSpan.FromSeconds(5);

        if (!actionRequired())
        {
            return;
        }

        String cacheKey = $"{typeof(RedisConcurrentContext).FullName}+{session.Id}+{identifier}";

        await using IRedLock? resourceLock = await redisLockFactory.CreateLockAsync(cacheKey, expiryTime, waitTime, retryTime);

        Boolean isActionRequired = actionRequired();

        if (!resourceLock.IsAcquired && isActionRequired)
        {
            throw new ApplicationException($"Unable to renew the expired access_token. Unable to acquire a lock. Try again. Error: {resourceLock.InstanceSummary.ToString()}");
        }

        if (!isActionRequired)
        {
            return;
        }

        await @delegate();
    }

    #endregion

    #endregion
}