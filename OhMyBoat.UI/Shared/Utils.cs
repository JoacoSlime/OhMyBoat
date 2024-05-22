using System.ComponentModel;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Components.Forms;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Tiff;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Formats;

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
            return IsLong(password) && 
            HasTwoNum(password) &&
            password.Any(IsMayusc) && 
            password.Any(IsSymbol);
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
        private static bool IsLong(string password){
            return password.Length >= 8;
        }
        public static bool HasTwoNum(string nombre){
            return nombre.Count(char.IsDigit) >= 2;
        }
        public static bool HasNum(string nombre){
            return nombre.Any(char.IsDigit);
        }
        public static bool IsMayusc(char c){
            return char.IsAsciiLetterUpper(c);
        }
        public static bool IsSymbol(char c){
            return !char.IsLetterOrDigit(c);
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
                
        public static async Task<string> GetIconBase64(IBrowserFile file) {        
            var resizedFile = await file.RequestImageFileAsync(file.ContentType, 512, 512); // le hace un resize
            using var image = await Image.LoadAsync(resizedFile.OpenReadStream());
            image.Mutate(x => x.Resize(512, 512)); // Resize a un cuadrado de 512x512
            using var ms = new MemoryStream();
            await image.SaveAsPngAsync(ms); // Guarda la imagen en formato PNG
            return $"data:image/png;base64,{Convert.ToBase64String(ms.ToArray())}";
        }

        public static async Task<bool> IsValidImageFormat(IBrowserFile file)
        {
            try
            {
                // Ensure the file is not null
                if (file == null)
                {
                    return false;
                }

                // Read the file into a memory stream
                using var memoryStream = new MemoryStream();
                await file.OpenReadStream().CopyToAsync(memoryStream);
                memoryStream.Position = 0; // Reset stream position to the beginning

                // Detect the image format
                IImageFormat format = Image.DetectFormat(memoryStream);

                // Check if the format is either PNG or JPG
                if (format == PngFormat.Instance || format == JpegFormat.Instance || format == TiffFormat.Instance)
                {
                    return true;
                }
            }
            catch
            {
                // Catch any exception and return false
                return false;
            }

            return false;
        }
    }
}