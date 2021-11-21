using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace ExamCreator
{
    public class Crypto
    {
        /// <summary>
        /// Generates a 32 byte long random byte array
        /// </summary>
        /// <returns>32 byte long array with random values</returns>
        public static byte[] CreateSalt()
        {
            var buffer = new byte[32];
            var rng = new RNGCryptoServiceProvider();
            rng.GetBytes(buffer);
            return buffer;
        }

        /// <summary>
        /// Generates a hash from given password and salt
        /// </summary>
        /// <param name="password">Password to hash with a salt</param>
        /// <param name="salt">The salt generated with <see cref="CreateSalt"/></param>
        /// <returns>Hashed sequence of bytes</returns>
        public static byte[] HashPassword(string password, byte[] salt)
        {
            byte[] passbytes = Encoding.UTF8.GetBytes(password);
            byte[] passwithsalt = new byte[passbytes.Length + salt.Length];

            for (int i = 0; i < passwithsalt.Length; ++i)
            {
                if(i < passbytes.Length)
                    passwithsalt[passwithsalt.Length - i - 1] = passbytes[i];
                if(i < salt.Length)
                    passwithsalt[i] = salt[i];
            }

            return SHA256.HashData(passwithsalt);
        }

        /// <summary>
        /// Verifies that given password and salt's hashed value matches (or collides with) given hash
        /// </summary>
        /// <param name="password">Password to hash with the salt</param>
        /// <param name="salt">The salt which will be used to *salt* the password</param>
        /// <param name="hash">Hash function to compare</param>
        /// <returns><see langword="true"/> if hashes are the same, else <see langword="false"/></returns>
        public static bool VerifyHash(string password, byte[] salt, byte[] hash)
        {
            var newHash = HashPassword(password, salt);
            return hash.SequenceEqual(newHash);
        }
    }
}
