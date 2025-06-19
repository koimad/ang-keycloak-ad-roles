using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Company.iFX.BFF.Logging;

internal class DefaultLogger(IHttpContextAccessor httpContextAccessor, ILogger<DefaultLogger> logger) : ILogger
{
    #region Methods

    #region Private

    private void Write(String message, LogLevel logLevel)
    {
        HttpContext? context = httpContextAccessor.HttpContext;
        LogEntry logEntry = new LogEntry(context?.TraceIdentifier, message);
        Write(logEntry, logLevel);
    }


    private void Write(Exception exception, LogLevel logLevel)
    {
        var context = httpContextAccessor.HttpContext;
        var logEntry = new LogEntry(context?.TraceIdentifier, exception);
        Write(logEntry, logLevel);
    }


    private void Write(LogEntry logEntry, LogLevel logLevel)
    {
        if (logEntry.IsException)
        {
            const String exceptionMessageFormat = $"{{@{nameof(LogEntry.Time)}}}\", \"{{@{nameof(LogEntry.TraceId)}}}\", \"{{@{nameof(LogEntry.Exception)}}}\" ";

            logger.Log(logLevel, exceptionMessageFormat, logEntry.Time, logEntry.TraceId, logEntry.Exception);
            return;
        }

        const String messageFormat = $"{{@{nameof(LogEntry.Time)}}}\", \"{{@{nameof(LogEntry.TraceId)}}}\", \"{{@{nameof(LogEntry.Message)}}}\" ";

        logger.Log(logLevel, messageFormat, logEntry.Time, logEntry.TraceId, logEntry.Message);
    }

    #endregion

    #region Public

    public Task ErrorAsync(String message)
    {
        Write(message, LogLevel.Error);
        return Task.CompletedTask;
    }


    public Task ErrorAsync(Exception exception)
    {
        Write(exception, LogLevel.Error);
        return Task.CompletedTask;
    }


    public Task InformAsync(String message)
    {
        Write(message, LogLevel.Information);
        return Task.CompletedTask;
    }


    public Task TraceAsync(String message)
    {
        Write(message, LogLevel.Trace);
        return Task.CompletedTask;
    }


    public Task WarnAsync(String message)
    {
        Write(message, LogLevel.Warning);
        return Task.CompletedTask;
    }


    public Task WarnAsync(Exception exception)
    {
        Write(exception, LogLevel.Error);
        return Task.CompletedTask;
    }

    #endregion

    #endregion
}