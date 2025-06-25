using System.Runtime.CompilerServices;

namespace Company.iFX.BFF.IdentityModel.Internal;

public static class TaskHelpers
{
    #region Properties

    public static Boolean CanConfigureAwaitFalse { get; set; } = true;

    public static Boolean CanFactoryStartNew { get; set; } = true;

    #endregion

    #region Methods

    #region Internal

    internal static ConfiguredTaskAwaitable ConfigureAwait(this Task task)
    {
        return task.ConfigureAwait(!CanConfigureAwaitFalse);
    }


    internal static ConfiguredTaskAwaitable<TResult> ConfigureAwait<TResult>(this Task<TResult> task)
    {
        return task.ConfigureAwait(!CanConfigureAwaitFalse);
    }

    #endregion

    #endregion
}