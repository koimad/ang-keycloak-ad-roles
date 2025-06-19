using Microsoft.AspNetCore.Http;

namespace Company.iFX.BFF.Locking;

public interface IConcurrentContext
{
    Task ExecuteOncePerSession(ISession session, String identifier, Func<Boolean> actionRequired, Func<Task> @delegate);
}