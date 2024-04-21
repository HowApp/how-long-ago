namespace How.Common.Helpers;

using System.Security.Cryptography;
using System.Text;

public class HashHelper
{
    public static string ComputeMd5(string str)
    {
        using (MD5 md5 = MD5.Create())
        {
            byte[] inputBytes = Encoding.ASCII.GetBytes(str);
            byte[] hashByte = md5.ComputeHash(inputBytes);

            return Convert.ToHexString(hashByte);
        }
    }
}