using System.Security.Cryptography;

namespace Company.iFX.BFF.IdentityModel;

public class CryptoRandom : Random
{
    #region Members

    private static readonly RandomNumberGenerator Rng = RandomNumberGenerator.Create();
    private readonly Byte[] _uint32Buffer = new Byte[4];

    #endregion

    #region Methods

    #region Public

    public static Byte[] CreateRandomKey(Int32 length)
    {
        Byte[] bytes = new Byte[length];
        Rng.GetBytes(bytes);

        return bytes;
    }


    public static String CreateUniqueId(Int32 length = 32, OutputFormat format = OutputFormat.Base64Url)
    {
        Byte[] bytes = CreateRandomKey(length);

        switch (format)
        {
            case OutputFormat.Base64Url:
                return Base64Url.Encode(bytes);

            case OutputFormat.Base64:
                return Convert.ToBase64String(bytes);

            case OutputFormat.Hex:
                return BitConverter.ToString(bytes).Replace("-", "");

            default:
                throw new ArgumentException("Invalid output format", nameof(format));
        }
    }


    public override Int32 Next()
    {
        Rng.GetBytes(_uint32Buffer);
        return BitConverter.ToInt32(_uint32Buffer, 0) & 0x7FFFFFFF;
    }


    public override Int32 Next(Int32 maxValue)
    {
        if (maxValue < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(maxValue));
        }

        return Next(0, maxValue);
    }


   public override Int32 Next(Int32 minValue, Int32 maxValue)
    {
        if (minValue > maxValue)
        {
            throw new ArgumentOutOfRangeException(nameof(minValue));
        }

        if (minValue == maxValue)
        {
            return minValue;
        }

        Int64 diff = maxValue - minValue;

        while (true)
        {
            Rng.GetBytes(_uint32Buffer);
            UInt32 rand = BitConverter.ToUInt32(_uint32Buffer, 0);

            Int64 max = 1 + (Int64)UInt32.MaxValue;
            Int64 remainder = max % diff;

            if (rand < max - remainder)
            {
                return (Int32)(minValue + rand % diff);
            }
        }
    }


    public override void NextBytes(Byte[] buffer)
    {
        if (buffer == null)
        {
            throw new ArgumentNullException(nameof(buffer));
        }

        Rng.GetBytes(buffer);
    }


    public override Double NextDouble()
    {
        Rng.GetBytes(_uint32Buffer);
        UInt32 rand = BitConverter.ToUInt32(_uint32Buffer, 0);
        return rand / (1.0 + UInt32.MaxValue);
    }

    #endregion

    #endregion

}