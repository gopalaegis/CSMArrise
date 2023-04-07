using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace BCInsight.Code
{
    public class Helper
    {
        public static string GetOTP()
        {
            int maxSize = 4;
            // int minSize = 5 ;
            char[] chars = new char[62];
            string a;
            a = "1234567890";
            chars = a.ToCharArray();
            int size = maxSize;
            byte[] data = new byte[1];
            RNGCryptoServiceProvider crypto = new RNGCryptoServiceProvider();
            crypto.GetNonZeroBytes(data);
            size = maxSize;
            data = new byte[size];
            crypto.GetNonZeroBytes(data);
            StringBuilder result = new StringBuilder(size);
            foreach (byte b in data)
            {
                result.Append(chars[b % (chars.Length - 1)]);
            }
            return result.ToString();
        }

        public static string CreateRandomPassword(int PasswordLength)
        {
            string _allowedChars = "0123456789abcdefghijkmnopqrstuvwxyzABCDEFGHJKLMNOPQRSTUVWXYZ";
            Random randNum = new Random();
            char[] chars = new char[PasswordLength];
            int allowedCharCount = _allowedChars.Length;
            for (int i = 0; i < PasswordLength; i++)
            {
                chars[i] = _allowedChars[(int)((_allowedChars.Length) * randNum.NextDouble())];
            }
            return new string(chars);
        }

        public static string GenerateNewToken()
        {
            return "Blu&0b!e";
        }

        public static string SaveImage(string base64, string filepath)
        {
            var imgName = Guid.NewGuid() + ".jpg";

            using (MemoryStream ms = new MemoryStream(Convert.FromBase64String(base64)))
            {
                using (Bitmap bm2 = new Bitmap(ms))
                {
                    if (!Directory.Exists(System.Web.Hosting.HostingEnvironment.MapPath(filepath)))
                        Directory.CreateDirectory(System.Web.Hosting.HostingEnvironment.MapPath(filepath));

                    string Uploadpath = System.Web.Hosting.HostingEnvironment.MapPath(filepath);
                    string filename = Uploadpath + imgName;

                    bm2.Save(filename, ImageFormat.Jpeg);
                }
            }

            return filepath + imgName;
        }
    }
}