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
        SemaphoreSlim? result = null;

        if (_collection.TryGetValue(key, out SemaphoreSlim? value))
        {
            result = value;
        }
        else
        {
            value = new SemaphoreSlim(1);

            result = _collection.TryAdd(key, value) || _collection.TryGetValue(key, out value) ? value : throw new ApplicationException("Unable to create a lock.");
        }

        return result;
    }


    public static void RemoveInstance(String key)
    {
        _collection.Remove(key, out _);
    }

    #endregion

    #endregion
}