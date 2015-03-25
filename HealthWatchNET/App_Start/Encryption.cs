using System;
using System.IO;
using System.Security.Cryptography;

namespace HealthWatchNET
{
    class Encryption
    {
        private static readonly byte[] ENC_SALT = new byte[] { 0x24, 0xdc, 0xff, 0x03, 0xad, 0xed, 0x7a, 0xee, 0xc5, 0xfe, 0x07, 0xaf, 0x4d, 0x08, 0x22, 0x3c };
        private static readonly string ENC_PASSWORD = "HCS010";

        public static string Encrypt(string str_plain)
        {
            byte[] plain = GetBytes(str_plain);

            MemoryStream memoryStream;
            CryptoStream cryptoStream;
            Rijndael rijndael = Rijndael.Create();
            Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(ENC_PASSWORD, ENC_SALT);
            rijndael.Key = pdb.GetBytes(32);
            rijndael.IV = pdb.GetBytes(16);
            memoryStream = new MemoryStream();
            cryptoStream = new CryptoStream(memoryStream, rijndael.CreateEncryptor(), CryptoStreamMode.Write);
            cryptoStream.Write(plain, 0, plain.Length);
            cryptoStream.Close();
            return GetBase64String(memoryStream.ToArray());
        }

        public static string Decrypt(string str_base64cipher)
        {
            if (str_base64cipher != "")
            {
                byte[] cipher = GetBytesFromBase64(str_base64cipher);
                MemoryStream memoryStream;
                CryptoStream cryptoStream;
                Rijndael rijndael = Rijndael.Create();
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(ENC_PASSWORD, ENC_SALT);
                rijndael.Key = pdb.GetBytes(32);
                rijndael.IV = pdb.GetBytes(16);
                memoryStream = new MemoryStream();
                cryptoStream = new CryptoStream(memoryStream, rijndael.CreateDecryptor(), CryptoStreamMode.Write);
                cryptoStream.Write(cipher, 0, cipher.Length);
                cryptoStream.Close();
                return GetString(memoryStream.ToArray());
            }
            else
            {
                return "";
            }
        }

        public static byte[] GetBytes(string str)
        {
            byte[] bytes = new byte[str.Length * sizeof(char)];
            System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }

        public static string GetString(byte[] bytes)
        {
            char[] chars = new char[bytes.Length / sizeof(char)];
            System.Buffer.BlockCopy(bytes, 0, chars, 0, bytes.Length);
            return new string(chars);
        }

        public static byte[] GetBytesFromBase64(string base64_str)
        {
            byte[] str = Convert.FromBase64String(base64_str);

            byte[] bytes = new byte[str.Length * sizeof(byte)];
            System.Buffer.BlockCopy(str, 0, bytes, 0, bytes.Length);
            return bytes;
        }

        public static string GetBase64String(byte[] bytes)
        {
            byte[] to_bytes = new byte[bytes.Length / sizeof(byte)];
            System.Buffer.BlockCopy(bytes, 0, to_bytes, 0, bytes.Length);
            return Convert.ToBase64String(to_bytes);
        }
    }
}