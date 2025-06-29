using System.Security.Claims;

namespace Company.iFX.BFF.IdentityModel;

public class ClaimComparer : EqualityComparer<Claim>
{
    #region Members

    private readonly ClaimOptions _options = new();

    #endregion

    #region Constructors

    public ClaimComparer() { }


    public ClaimComparer(ClaimOptions options)
    {
        _options = options ?? throw new ArgumentNullException(nameof(options));
    }

    #endregion

    #region Methods

    #region Public

    public override Boolean Equals(Claim? x, Claim? y)
    {
        if (x == null && y == null)
        {
            return true;
        }

        if (x == null && y != null)
        {
            return false;
        }

        if (x != null && y == null)
        {
            return false;
        }

        if (x == null)
        {
            throw new ArgumentNullException(nameof(x));
        }

        if (y == null)
        {
            throw new ArgumentNullException(nameof(y));
        }

        StringComparison valueComparison = StringComparison.Ordinal;

        if (_options.IgnoreValueCase)
        {
            valueComparison = StringComparison.OrdinalIgnoreCase;
        }

        Boolean equal = String.Equals(x.Type, y.Type, StringComparison.OrdinalIgnoreCase) &&
                        String.Equals(x.Value, y.Value, valueComparison) &&
                        String.Equals(x.ValueType, y.ValueType, StringComparison.Ordinal);

        return _options.IgnoreIssuer ? equal : equal && String.Equals(x.Issuer, y.Issuer, valueComparison);
    }


    public override Int32 GetHashCode(Claim? claim)
    {
        if (claim is null)
        {
            return 0;
        }

        Int32 typeHash = claim.Type?.ToLowerInvariant().GetHashCode() ?? 0 ^ claim.ValueType?.GetHashCode() ?? 0;
        Int32 valueHash;
        Int32 issuerHash;

        if (_options.IgnoreValueCase)
        {
            valueHash = claim.Value?.ToLowerInvariant().GetHashCode() ?? 0;
            issuerHash = claim.Issuer?.ToLowerInvariant().GetHashCode() ?? 0;
        }
        else
        {
            valueHash = claim.Value?.GetHashCode() ?? 0;
            issuerHash = claim.Issuer?.GetHashCode() ?? 0;
        }

        return _options.IgnoreIssuer ? typeHash ^ valueHash : typeHash ^ valueHash ^ issuerHash;
    }

    #endregion

    #endregion
}