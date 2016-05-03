using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace GenericPaymentService.Crypto
{
    public class EncryptionHelper
    {
        public static string CalculateSHA256(string text, Encoding enc)
        {
            byte[] buffer = enc.GetBytes(text);
            SHA256CryptoServiceProvider cryptoTransformSHA256 = new SHA256CryptoServiceProvider();
            return BitConverter.ToString(cryptoTransformSHA256.ComputeHash(buffer)).Replace("-", "");
        }

        /// <summary>
        /// Encrypts a string with a password using AES with CBC Cipher mode
        /// </summary>
        /// <param name="toBeEncrypted"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public static string AES_Encrypt(string toBeEncrypted, string password)
        {
            byte[] bytesToBeEncrypted = Encoding.UTF8.GetBytes(toBeEncrypted);
            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);

            byte[] encryptedBytes = null;

            byte[] saltBytes = new byte[] { 5, 230, 24, 27, 198, 2, 7, 2 };

            using (MemoryStream ms = new MemoryStream())
            {
                using (RijndaelManaged aes = new RijndaelManaged())
                {
                    aes.KeySize = 256;
                    aes.BlockSize = 128;

                    var key = new Rfc2898DeriveBytes(passwordBytes, saltBytes, 1000);
                    aes.Key = key.GetBytes(aes.KeySize / 8);
                    aes.IV = key.GetBytes(aes.BlockSize / 8);

                    aes.Mode = CipherMode.CBC;
                    aes.Padding = PaddingMode.PKCS7;

                    using (var cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(bytesToBeEncrypted, 0, bytesToBeEncrypted.Length);
                        cs.Close();
                    }
                    encryptedBytes = ms.ToArray();
                }
            }

            return Convert.ToBase64String(encryptedBytes);
        }

        /// <summary>
        /// Decrypt a string with a password using AES with CBC Cipher mode
        /// 
        /// Can only decrypt text that was encrypted by AES_Encrypt
        /// </summary>
        /// <param name="toBeEncrypted"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public static string AES_Decrypt(string toBeDecrypted, string password)
        {
            byte[] bytesToBeDecrypted = Convert.FromBase64String(toBeDecrypted);
            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);

            byte[] decryptedBytes = null;

            byte[] saltBytes = new byte[] { 5, 230, 24, 27, 198, 2, 7, 2 };

            using (MemoryStream ms = new MemoryStream())
            {
                using (RijndaelManaged AES = new RijndaelManaged())
                {
                    AES.KeySize = 256;
                    AES.BlockSize = 128;

                    var key = new Rfc2898DeriveBytes(passwordBytes, saltBytes, 1000);
                    AES.Key = key.GetBytes(AES.KeySize / 8);
                    AES.IV = key.GetBytes(AES.BlockSize / 8);

                    AES.Mode = CipherMode.CBC;
                    AES.Padding = PaddingMode.PKCS7;

                    using (var cs = new CryptoStream(ms, AES.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(bytesToBeDecrypted, 0, bytesToBeDecrypted.Length);
                        cs.Close();
                    }
                    decryptedBytes = ms.ToArray();
                }
            }

            return Encoding.UTF8.GetString(decryptedBytes);
        }

        /// <summary>
        /// Converts a dictionary to a querystring style string and encrypts into a string
        /// </summary>
        /// <param name="values"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public static string EncryptDictionary(Dictionary<string, string> values, string password)
        {
            StringBuilder sb = new StringBuilder();

            int counter = 1;

            foreach (KeyValuePair<string, string> value in values)
            {
                sb.AppendFormat("{0}={1}", value.Key, HttpUtility.UrlEncode(value.Value));

                if (counter != values.Count)
                {
                    sb.Append("&");
                }

                counter++;
            }

            return AES_Encrypt(sb.ToString(), password);
        }

        /// <summary>
        /// Decrypts the encrypted dictionary into a dictionary type
        /// </summary>
        /// <param name="values"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public static Dictionary<string, string> DecryptDictionary(string value, string password)
        {
            string decryptedValue = AES_Decrypt(value, password);

            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            foreach (var keyValue in decryptedValue.Split(new[] { '&' }, StringSplitOptions.RemoveEmptyEntries))
            {
                string[] keyValueArray = keyValue.Split('=');
                dictionary.Add(keyValueArray[0], HttpUtility.UrlDecode(keyValueArray[1]));
            }

            return dictionary;
        }

        /// <summary>
        /// Safely generates a unique random string ready for encryption, or any where you need a unique string.
        /// Use this, not Guid!
        /// </summary>
        /// <param name="stringLength"></param>
        /// <returns></returns>
        public static string GenerateUniqueString(int stringLength)
        {
            string characters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
            {
                byte[] data = new byte[stringLength];

                rng.GetBytes(data);

                StringBuilder builder = new StringBuilder();
                foreach (var b in data)
                {
                    builder.Append(characters[Convert.ToInt32(b) % (characters.Length - 1)]);
                }
                return builder.ToString();
            }
        }
    }

}