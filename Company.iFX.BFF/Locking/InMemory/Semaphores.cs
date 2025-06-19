using System.Collections.Concurrent;

namespace Company.iFX.BFF.Locking.InMemory;

internal static class Semaphores
{
    #region Members

    private static readonly ConcurrentDictionary<String, SemaphoreSlim> _collection = new();

    #endregion

    #region Methods

    #region Public

    public static SemaphoreSlim GetInstance(String key)
    {
        if (_collection.TryGetValue(key, out SemaphoreSlim? value))
        {
            return value;
        }

        value = new SemaphoreSlim(1);

        if (_collection.TryAdd(key, value))
        {
            return value;
        }

        if (_collection.TryGetValue(key, out value))
        {
            return value;
        }

        throw new ApplicationException("Unable to create a lock.");
    }


    public static void RemoveInstance(String key)
    {
        _collection.Remove(key, out _);
    }

    #endregion

    #endregion
}