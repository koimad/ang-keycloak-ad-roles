
using System.Security.Claims;

namespace Company.iFX.BFF.OIDC.Client.Results
{
    public class UserInfoResult : Result
    {
        public virtual IEnumerable<Claim>? Claims { get; set; }
    }
}