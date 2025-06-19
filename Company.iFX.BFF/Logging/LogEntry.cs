namespace Company.iFX.BFF.Logging;

internal class LogEntry
{
    #region Properties

    public Boolean IsException => String.IsNullOrEmpty(Message);

    public String TraceId { get; }
    public String? Message { get; }
    public Exception? Exception { get; }

    public DateTime Time { get; } = DateTime.Now;

    #endregion

    #region Constructors

    public LogEntry(String? traceId, Exception exception)
    {
        TraceId = traceId ?? String.Empty;
        Exception = exception;
    }


    public LogEntry(String? traceId, String message)
    {
        TraceId = traceId ?? String.Empty;
        Message = message;
    }

    #endregion
}