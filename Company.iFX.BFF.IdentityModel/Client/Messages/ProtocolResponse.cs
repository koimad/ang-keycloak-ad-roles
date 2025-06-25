using System.Net;
using System.Text;
using System.Text.Json;

using Company.iFX.BFF.IdentityModel.Client.Extensions;
using Company.iFX.BFF.IdentityModel.Internal;

namespace Company.iFX.BFF.IdentityModel.Client.Messages;

public class ProtocolResponse
{
    #region Properties

    public HttpResponseMessage? HttpResponse { get; protected set; }

    public String? Raw { get; protected set; }

    public JsonElement? Json { get; protected set; }

    public Exception? Exception { get; protected set; }

    public Boolean IsError => Error.IsPresent() || ErrorType != ResponseErrorType.None;

    public ResponseErrorType ErrorType { get; protected set; } = ResponseErrorType.None;

    public HttpStatusCode HttpStatusCode => HttpResponse?.StatusCode ?? default;

    public String? HttpErrorReason => HttpResponse?.ReasonPhrase ?? null;

    public String? Error
    {
        get
        {
            if (ErrorMessage.IsPresent())
            {
                return ErrorMessage;
            }

            if (ErrorType == ResponseErrorType.Http)
            {
                return HttpErrorReason;
            }

            if (ErrorType == ResponseErrorType.Exception)
            {
                return Exception!.Message;
            }

            return TryGet(OidcConstants.TokenResponse.Error);
        }
    }

    public String? DPoPNonce { get; set; }

    protected String? ErrorMessage { get; set; }

    #endregion

    #region Methods

    #region Protected

    protected virtual Task InitializeAsync(Object? initializationData = null)
    {
        return Task.CompletedTask;
    }

    #endregion

    #region Public

    public static T FromException<T>(Exception ex, String? errorMessage = null) where T : ProtocolResponse, new()
    {
        T response = new T {
            Exception = ex,
            ErrorType = ResponseErrorType.Exception,
            ErrorMessage = errorMessage
        };

        return response;
    }


    public static async Task<T> FromHttpResponseAsync<T>(HttpResponseMessage httpResponse, Object? initializationData = null, Boolean skipJson = false) where T : ProtocolResponse, new()
    {
        T response = new T {
            HttpResponse = httpResponse
        };

        String content = String.Empty;

        try
        {
            content = await httpResponse.Content.ReadAsStringAsync().ConfigureAwait();

            response.Raw = content;
        }
        catch (Exception ex)
        {
            response.ErrorType = ResponseErrorType.Exception;
            response.Exception = ex;
        }

        if (httpResponse.IsSuccessStatusCode != true && httpResponse.StatusCode != HttpStatusCode.BadRequest)
        {
            response.ErrorType = ResponseErrorType.Http;

            if (!skipJson && content.IsPresent())
            {
                try
                {
                    response.Json = JsonDocument.Parse(content!).RootElement;
                }
                catch { }
            }

            await response.InitializeAsync(initializationData).ConfigureAwait();
            return response;
        }

        if (httpResponse.StatusCode == HttpStatusCode.BadRequest)
        {
            response.ErrorType = ResponseErrorType.Protocol;
        }

        try
        {
            if (!skipJson && content.IsPresent())
            {
                response.Json = JsonDocument.Parse(content!).RootElement;
            }
        }
        catch (Exception ex)
        {
            response.ErrorType = ResponseErrorType.Exception;
            response.Exception = ex;
        }

        if (httpResponse.Headers.TryGetValues(OidcConstants.HttpHeaders.DPoPNonce, out IEnumerable<String>? nonceHeaders))
        {
            if (nonceHeaders.Count() == 1)
            {
                response.DPoPNonce = nonceHeaders.First();
            }
        }

        if (!skipJson)
        {
            await response.InitializeAsync(initializationData).ConfigureAwait();
        }

        return response;
    }


    public String? TryGet(String name)
    {
        return Json?.TryGetString(name);
    }


    #endregion

    #endregion
}