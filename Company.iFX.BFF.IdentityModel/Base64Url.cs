namespace Company.iFX.BFF.IdentityModel;

public static class Base64Url
{
    #region Methods

    #region Public

    public static Byte[] Decode(String arg)
    {
        String s = arg;
        s = s.Replace('-', '+'); 
        s = s.Replace('_', '/'); 

        switch (s.Length % 4) 
        {
            case 0: break; 

            case 2: s += "=="; break; 

            case 3: s += "="; break; 

            default: 
                throw new Exception("Illegal base64url string!");
        }

        return Convert.FromBase64String(s); 
    }


    public static String Encode(Byte[] arg)
    {
        String s = Convert.ToBase64String(arg); // Standard base64 encoder

        s = s.Split('=')[0]; 
        s = s.Replace('+', '-'); 
        s = s.Replace('/', '_');

        return s;
    }

    #endregion

    #endregion
}