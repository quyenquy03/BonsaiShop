namespace BonsaiShop.Ultilities
{
    public class Functions
    {
        public static string TitleSlugGeneration(string type, string title, long id)
        {
            string sTitle = type + "-" + SlugGenerator.SlugGenerator.GenerateSlug(title) + "-" + id.ToString() + ".html";
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
    }
}
