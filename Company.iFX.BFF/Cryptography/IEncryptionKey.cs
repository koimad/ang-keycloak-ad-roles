namespace Company.iFX.BFF.Cryptography;

public interface IEncryptionKey
{
    public String Decrypt(String token);
}