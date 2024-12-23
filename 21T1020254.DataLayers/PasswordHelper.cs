using System;
using System.Security.Cryptography;
using System.Text;

namespace _21T1020254.DataLayers
{
   

    public static class PasswordHelper
    {
        // Hàm mã hóa mật khẩu bằng PBKDF2
        public static string HashPassword(string password)
        {
            using (var hmac = new HMACSHA256())
            {
                // Tạo salt ngẫu nhiên
                byte[] salt = new byte[16];
                using (var rng = new RNGCryptoServiceProvider())
                {
                    rng.GetBytes(salt);
                }

                // Sử dụng PBKDF2 để mã hóa mật khẩu với salt
                byte[] hash = Rfc2898DeriveBytes.Pbkdf2(password, salt, 10000, HashAlgorithmName.SHA256, 32);

                // Kết hợp salt và hash vào một chuỗi để lưu vào DB
                byte[] result = new byte[salt.Length + hash.Length];
                Buffer.BlockCopy(salt, 0, result, 0, salt.Length);
                Buffer.BlockCopy(hash, 0, result, salt.Length, hash.Length);

                return Convert.ToBase64String(result);
            }
        }

        // Hàm kiểm tra mật khẩu khi đăng nhập
        public static bool VerifyPassword(string enteredPassword, string storedPasswordHash)
        {
            byte[] storedBytes = Convert.FromBase64String(storedPasswordHash);

            // Tách salt và hash từ mật khẩu đã lưu
            byte[] salt = new byte[16];
            byte[] storedHash = new byte[32];
            Buffer.BlockCopy(storedBytes, 0, salt, 0, salt.Length);
            Buffer.BlockCopy(storedBytes, salt.Length, storedHash, 0, storedHash.Length);

            // Tính toán lại hash từ mật khẩu nhập vào và salt lưu trữ
            byte[] hash = Rfc2898DeriveBytes.Pbkdf2(enteredPassword, salt, 10000, HashAlgorithmName.SHA256, 32);

            // So sánh hash đã tính toán với hash lưu trữ
            return hash.SequenceEqual(storedHash);
        }
    }

}
