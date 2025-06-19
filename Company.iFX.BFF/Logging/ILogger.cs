namespace Company.iFX.BFF.Logging;

public interface ILogger
{
    Task TraceAsync(String message);


    Task InformAsync(String message);


    Task WarnAsync(String message);


    Task WarnAsync(Exception exception);


    Task ErrorAsync(String message);


    Task ErrorAsync(Exception exception);
}