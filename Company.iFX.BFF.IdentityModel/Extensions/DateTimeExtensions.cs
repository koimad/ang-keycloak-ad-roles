// ReSharper disable once CheckNamespace
namespace System;

public static class DateTimeExtensions
{
    #region Methods

    #region Public

    public static DateTime ToDateTimeFromEpoch(this Int64 date)
    {
        Int64 timeInTicks = date * TimeSpan.TicksPerSecond;
        return new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddTicks(timeInTicks);
    }


    public static Int64 ToEpochTime(this DateTime dateTime)
    {
        DateTime date = dateTime.ToUniversalTime();
        Int64 ticks = date.Ticks - new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).Ticks;
        Int64 ts = ticks / TimeSpan.TicksPerSecond;
        return ts;
    }

    #endregion

    #endregion
}