using System.ComponentModel;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Components.Forms;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Processing;

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

        public static async Task<string> GetImageBase64(IBrowserFile file) {        
            var resizedFile = await file.RequestImageFileAsync(file.ContentType, 640, 480); // le hace un resize
            var buf = new byte[resizedFile.Size]; // buffer para llenar la data de la imagen
            using (var stream = resizedFile.OpenReadStream())
            {
                await stream.ReadAsync(buf); // copia el stream a el buffer
            }
            return $"data:image;base64,{Convert.ToBase64String(buf)}";
        }

        public static async Task<string> GetImageBase64(string relativePath)
        {
            string fullPath = Path.Combine(Directory.GetCurrentDirectory(), relativePath);
            if (!File.Exists(fullPath))
            {
                throw new FileNotFoundException("File not found.", fullPath);
            }

            using var image = await Image.LoadAsync(fullPath);
            image.Mutate(x => x.Resize(new ResizeOptions
            {
                Size = new Size(640, 480),
                Mode = ResizeMode.Max
            }));

            using var ms = new MemoryStream();
            await image.SaveAsync(ms, new PngEncoder());
            var imageBytes = ms.ToArray();
            return $"data:image/png;base64,{Convert.ToBase64String(imageBytes)}";
        }
        
        public static async Task<string> GetIconBase64(IBrowserFile file) {        
            var resizedFile = await file.RequestImageFileAsync(file.ContentType, 512, 512); // le hace un resize
            var buf = new byte[resizedFile.Size]; // buffer para llenar la data de la imagen
            using (var stream = resizedFile.OpenReadStream())
            {
                await stream.ReadAsync(buf); // copia el stream a el buffer
            }
            return $"data:image;base64,{Convert.ToBase64String(buf)}";
        }

        public static async Task<string> GetIconBase64(string relativePath)
        {
            string fullPath = Path.Combine(Directory.GetCurrentDirectory(), relativePath);
            if (!File.Exists(fullPath))
            {
                throw new FileNotFoundException("File not found.", fullPath);
            }

            using var image = await Image.LoadAsync(fullPath);
            image.Mutate(x => x.Resize(new ResizeOptions
            {
                Size = new Size(512, 512),
                Mode = ResizeMode.Max
            }));

            using var ms = new MemoryStream();
            await image.SaveAsync(ms, new PngEncoder());
            var imageBytes = ms.ToArray();
            return $"data:image/png;base64,{Convert.ToBase64String(imageBytes)}";
        }

    }
}