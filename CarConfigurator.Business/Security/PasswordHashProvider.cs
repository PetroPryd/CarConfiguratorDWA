using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace CarConfigurator.Business.Security
{
    public class PasswordHashProvider
    {
        public static string GetSalt()
        {
            byte[] salt = RandomNumberGenerator.GetBytes(128 / 8);
            return Convert.ToBase64String(salt);
        }

        public static string GetHash(string password, string b64salt)
        {
            byte[] salt = Convert.FromBase64String(b64salt);
            byte[] hash = KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 100000,
                numBytesRequested: 256 / 8);
            return Convert.ToBase64String(hash);
        }
    }
}
