using System;
using System.Security.Cryptography;
using System.Text;

using Android.App;

namespace XamarinApp
{
    class AppData:Application
    {
        public static string LoggedInUser { get; set; } = "None";

        public static string EncryptPassword(string password)
        {
            byte[] bytPassword = Encoding.UTF8.GetBytes(password);
            SHA512Managed SHA5 = new SHA512Managed();
            byte[] hash = SHA5.ComputeHash(bytPassword);
            {
                return Convert.ToBase64String(hash);
            }
        }
    }
}