using System.Runtime.CompilerServices;

namespace Company.iFX.BFF.IdentityModel.Extensions;

public static class TimeConstantComparer
{
    #region Methods

    #region Public

    [MethodImpl(MethodImplOptions.NoOptimization)]
    public static Boolean IsEqual(String? s1, String? s2)
    {
        if (s1 == null && s2 == null)
        {
            return true;
        }

        if (s1 == null || s2 == null)
        {
            return false;
        }

        if (s1.Length != s2.Length)
        {
            return false;
        }

        Char[] s1Chars = s1.ToCharArray();
        Char[] s2Chars = s2.ToCharArray();

        Int32 hits = 0;

        for (Int32 i = 0; i < s1.Length; i++)
        {
            if (s1Chars[i].Equals(s2Chars[i]))
            {
                hits += 2;
            }
            else
            {
                hits += 1;
            }
        }

        Boolean same = hits == s1.Length * 2;

        return same;
    }

    #endregion

    #endregion
}