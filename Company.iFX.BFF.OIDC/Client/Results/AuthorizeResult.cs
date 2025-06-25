namespace Company.iFX.BFF.OIDC.Client.Results
{
    public class AuthorizeResult : Result
    {
        public String? Data { get; set; }
        public AuthorizeState? State { get; set; }
    }
}