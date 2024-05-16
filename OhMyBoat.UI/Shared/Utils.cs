using System.Security.Cryptography;
using System.Text;

namespace OhMyBoat.UI.Shared
{
    public static class Utils {
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
        public static string HashWithSha256(string ActualData)
        {
            using (SHA256 s = SHA256.Create())
            {
                byte[] bytes = s.ComputeHash(Encoding.UTF8.GetBytes(ActualData));
                StringBuilder b = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    b.Append(bytes[i].ToString("x2"));
                }
                return b.ToString();
            }
        }
    }
}