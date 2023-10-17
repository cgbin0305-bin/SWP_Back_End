using System.Security.Cryptography;
using System.Text;

namespace API.Helper
{
    public static class HashPasswordHelper
    {
        public static (byte[], byte[]) GetPasswordHash(this string password)
        {
            byte[] PasswordHash;
            byte[] PasswordSalt;
            using (var hmac = new HMACSHA512())
            {
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
                PasswordSalt = hmac.Key;
            };
            return (PasswordHash, PasswordSalt);
        }
    }
}