using System.Security.Cryptography;
using System.Text;

namespace BonsaiShop.Extension
{
    public static class HashPassword
    {
        public static string MD5Hash(string password)
        {
            MD5 md5= new MD5CryptoServiceProvider();
            md5.ComputeHash(ASCIIEncoding.ASCII.GetBytes(password));
            byte[] result = md5.Hash;
            StringBuilder strbuiler = new StringBuilder();
            for(int i=0; i<result.Length; i++)
            {
                strbuiler.Append(result[i].ToString("x2"));
            }
            return strbuiler.ToString();
        }
        public static string MD5Password (string? password)
        {
            string str = MD5Hash(password);
            for(int i=0; i<=5; i++)
            {
                str = MD5Hash(str + "_" + str);
            }
            return str;
        }
    }
}
