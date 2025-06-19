namespace Company.iFX.BFF.Cryptography;

public class KeySet(Byte[] exponent, Byte[] modulus, String kid)
{
    #region Properties

    public Byte[] Exponent { get; private set; } = exponent;

    public Byte[] Modulus { get; private set; } = modulus;

    public String Kid { get; private set; } = kid;

    #endregion
}