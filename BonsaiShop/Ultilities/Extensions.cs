using System.Text;
using System.Text.RegularExpressions;

namespace BonsaiShop.Extension
{
    public static class Extensions
    {
        public static string ToVnd (this double donGia)
        {
            return donGia.ToString ("#, ##0") + "đ";
        }
        public static string ToTitleCase (string str)
        {
            string result = str;
            if(!string.IsNullOrEmpty(str))
            {
                var word = str.Split(' ');
                for(int i = 0; i<word.Length; i++)
                {
                    var s = word[i];
                    if(s.Length > 0)
                    {
                        word[i] = s[0].ToString().ToUpper() +s.Substring(1);
                    }
                }
                result = string.Join(" ", word);
            }
            return result;
        }
        public static string ToUrlFriendly(this string url)
        {
            var result = url.ToLower().Trim();
            Regex regex = new Regex("\\p{IsCombiningDiacriticalMarks}+");
            result = result.Normalize(NormalizationForm.FormD);
            result = regex.Replace(result, String.Empty).Replace("đ", "d").Replace("Đ", "D");
            result = Regex.Replace(result, @"\W+", "-");
            Random ran = new Random();
            int number = ran.Next(10000);
            result = result + "-" + number.ToString();
            return result;
        }
    }
}
