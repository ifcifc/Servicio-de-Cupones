using System.Security.Cryptography;
using System.Text;

namespace Common.Services
{
    public class HashPasswordService
    {
        public string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(password);
                byte[] hash = sha256.ComputeHash(bytes);

                StringBuilder builder = new StringBuilder();
                foreach (byte b in hash)
                {
                    //x2 -> convierte el byte en un hexadecimal de dos digitos
                    builder.Append(b.ToString("x2"));
                }

                return builder.ToString();
            }
        }
    }
}
