namespace Company.iFX.BFF.OIDC.Client.Browser;

public interface IBrowser
{
    Task<BrowserResult> InvokeAsync(BrowserOptions options, CancellationToken cancellationToken = default);
}