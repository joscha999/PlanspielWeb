using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace DAL {
    public class PasswordHasher {
        private const int PepperLength = 16;
        private const int KeyLength = 48;
        private const int Iterations = 10000;

        private readonly RNGCryptoServiceProvider rng;

        public PasswordHasher() {
            rng = new RNGCryptoServiceProvider();
        }

        ~PasswordHasher() {
            rng.Dispose();
        }

        public string Hash(string pw) {
            byte[] pepper = new byte[PepperLength];
            rng.GetBytes(pepper);

            using var pbkdf2 = new Rfc2898DeriveBytes(pw, pepper, Iterations);
            var hash = pbkdf2.GetBytes(KeyLength);

            var final = new byte[KeyLength + PepperLength];
            Array.Copy(pepper, 0, final, 0, PepperLength);
            Array.Copy(hash, 0, final, PepperLength, KeyLength);

            return Convert.ToBase64String(final);
        }

        //NOTE: usually you want to have some slower method to check if the hash
        //      is correct but we don't expect brute forces on this one so it's fine
        public bool CheckHash(string pw, string base64) {
            var hashBytes = Convert.FromBase64String(base64);
            var pepper = new byte[PepperLength];

            Array.Copy(hashBytes, 0, pepper, 0, PepperLength);

            using var pbkdf2 = new Rfc2898DeriveBytes(pw, pepper, Iterations);
            var hash = pbkdf2.GetBytes(KeyLength);

            for (int i = 0; i < KeyLength; i++) {
                if (hashBytes[i + PepperLength] != hash[i])
                    return false;
            }

            return true;
        }
    }
}