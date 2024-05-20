using System.ComponentModel;
using System.Security.Cryptography;
using System.Text;

namespace OhMyBoat.UI.Shared
{
    public static class Utils {
        public static String CapitalizeString(String role)
        {
            return Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(role);
        }
        public static bool IsValidEmail(String email) {
                var trimmedEmail = email.Trim();

                if (trimmedEmail.EndsWith(".")) {
                    return false; // suggested by @TK-421
                }
                try {
                    var addr = new System.Net.Mail.MailAddress(email);
                    return addr.Address == trimmedEmail;
                }
                catch {
                    return false;
                }
        }
        public static bool IsValidPassword(string password){
            return
            IsLong(password) && 
            HasNum(password) &&
            password.Any(c => IsMayusc(c)) && 
            password.Any(c => IsSymbol(c));
        }

        public static string HashWithSha256(string ActualData)
        {
            byte[] bytes = SHA256.HashData(Encoding.UTF8.GetBytes(ActualData));
            StringBuilder b = new();
            for (int i = 0; i < bytes.Length; i++)
            {
                b.Append(bytes[i].ToString("x2"));
            }
            return b.ToString();
        }
        public static bool IsMayusc(char c){
            return (c >= 'A' && c <= 'Z');
        }
        public static bool HasNum(string password){
            char[] characters = password.ToCharArray();
            int numCounter = 0;
            foreach (char c in characters){
                if (char.IsNumber(c)){
                    numCounter ++;
                }
            }
            return numCounter > 1;
        }
        public static bool IsSymbol(char c){
            String specials = "!?¿¡#$%&/=+-*_@.,";
            return specials.Contains(c);
        }
        private static bool IsLong(string password){
            return password.Length >= 8;
        }
    }
}