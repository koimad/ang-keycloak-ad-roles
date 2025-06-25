using System.Security.Cryptography;
using System.Text;

using Company.iFX.BFF.IdentityModel;

using Microsoft.Extensions.Logging;

namespace Company.iFX.BFF.OIDC.Client;

internal class CryptoHelper
{
    #region Members

    private readonly ILogger _logger;
    private readonly OidcClientOptions _options;

    #endregion

    #region Constructors

    public CryptoHelper(OidcClientOptions options)
    {
        _options = options;
        _logger = options.LoggerFactory.CreateLogger<CryptoHelper>();
    }

    #endregion

    #region Methods

    #region Public

    public Pkce CreatePkceData()
    {
        _logger.LogTrace("CreatePkceData");

        Pkce pkce = new Pkce {
            CodeVerifier = CryptoRandom.CreateUniqueId()
        };

        using (SHA256 sha256 = SHA256.Create())
        {
            Byte[] challengeBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(pkce.CodeVerifier));
            pkce.CodeChallenge = Base64Url.Encode(challengeBytes);
        }

        return pkce;
    }


    public String CreateState(Int32 length)
    {
        _logger.LogTrace("CreateState");

        return CryptoRandom.CreateUniqueId(length);
    }


    public HashAlgorithm? GetMatchingHashAlgorithm(String signatureAlgorithm)
    {
        _logger.LogTrace("GetMatchingHashAlgorithm");
        _logger.LogDebug("Determining matching hash algorithm for {signatureAlgorithm}", signatureAlgorithm);

        Int32 signingAlgorithmBits = Int32.Parse(signatureAlgorithm.Substring(signatureAlgorithm.Length - 3));

        switch (signingAlgorithmBits)
        {
            case 256:
                _logger.LogDebug("SHA256");
                return SHA256.Create();

            case 384:
                _logger.LogDebug("SHA384");
                return SHA384.Create();

            case 512:
                _logger.LogDebug("SHA512");
                return SHA512.Create();

            default:
                return null;
        }
    }


    public Boolean ValidateHash(String data, String hashedData, String signatureAlgorithm)
    {
        _logger.LogTrace("ValidateHash");

        HashAlgorithm? hashAlgorithm = GetMatchingHashAlgorithm(signatureAlgorithm);

        if (hashAlgorithm == null)
        {
            _logger.LogError("No appropriate hashing algorithm found.");
        }

        using (hashAlgorithm)
        {
            Byte[] hash = hashAlgorithm.ComputeHash(Encoding.ASCII.GetBytes(data));
            Int32 size = hashAlgorithm.HashSize / 8 / 2; // Only take the left half of the data, as per spec for at_hash 

            Byte[] leftPart = new Byte[size];
            Array.Copy(hash, leftPart, size);

            String leftPartB64 = Base64Url.Encode(leftPart);
            Boolean match = leftPartB64.Equals(hashedData);

            if (!match)
            {
                _logger.LogError($"data ({leftPartB64}) does not match hash from token ({hashedData})");
            }

            return match;
        }
    }

    #endregion

    #endregion

}