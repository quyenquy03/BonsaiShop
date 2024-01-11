using System.Security.Cryptography;
using System.Text;

namespace BonsaiShop.Ultilities
{
    public class Functions
    {
        public static string TitleSlugGeneration(string type, string title, long id)
        {
            string sTitle = type + "-" + SlugGenerator.SlugGenerator.GenerateSlug(title) + "-" + id.ToString() + ".html";
            return sTitle;
        }
		public static string UrlLink(string title, long id)
		{
			string sTitle =SlugGenerator.SlugGenerator.GenerateSlug(title) + "-" + id.ToString();
			return sTitle;
		}
		public static string AliasLink(string tilte)
        {
            Random rnd = new Random();
            string sTitle = SlugGenerator.SlugGenerator.GenerateSlug(tilte);
            return sTitle;
        }
        public static string FormatDate(string d)
        {
            string dateFomat;
            int location = d.IndexOf(" ");
            dateFomat = d.Substring(0, location);
            return dateFomat;
        }

        public static int PAGESIZE = 20;
        public static void CreateIfMissing(string path)
        {
            bool folderExits = Directory.Exists(path);
            if (!folderExits)
            {
                Directory.CreateDirectory(path);
            }
        }
        public static async Task<string> UploadFile(Microsoft.AspNetCore.Http.IFormFile file, string sDerectory, string newname)
        {
            try
            {
                if (newname == null) newname = file.FileName;
                string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", sDerectory);
                CreateIfMissing(path);
                string pathFile = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", sDerectory, newname);
                var supportedTypes = new[] { "jpg", "jpeg", "png", "gif", "webp" };
                var fileExt = System.IO.Path.GetExtension(file.FileName).Substring(1);
                if (!supportedTypes.Contains(fileExt.ToLower()))
                {
                    return null;
                }
                else
                {
                    using (var stream = new FileStream(pathFile, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }
                    return newname;
                }
            }
            catch
            {
                return null;
            }
        }
        public static string MD5Hash(string text)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            md5.ComputeHash(ASCIIEncoding.ASCII.GetBytes(text));
            byte[] result = md5.Hash;
            StringBuilder sb = new StringBuilder();
            for(int i=0; i<result?.Length; i++)
            {
                sb.Append(result[i].ToString("x2"));
            }
            return sb.ToString();
        }
        public static string MD5Password(string text)
        {
            string str = MD5Hash(text);
            for(int i=0;i<5; i++)
            {
                str += MD5Hash(str + "_" + str);
            }
            return str;
        }
    }
}
