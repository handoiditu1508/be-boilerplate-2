using Hamburger.Helpers.Extensions;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Serialization;

namespace Hamburger.Helpers
{
    public static class CipherHelper
    {
        private static readonly byte[] _aesKey = Encoding.UTF8.GetBytes("01234567890123456789012345678901");

        private static readonly RSAParameters _rsaPrivateKey = BuildRsaKey("<?xml version=\"1.0\" encoding=\"utf-16\"?><RSAParameters xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"><D>BeZ8P/HPSqAA3iJHho51VNKJd4mz5MXCsF2vvW09UVRONekVguuz11q6+1MRgNJgtQp4H7x+qWE1Yy++AfZjCmNYsLdR6KVLyMTu+Nq6nISwvrPfkY16qr99vMsABlmkMHc2z1lB7SxwRUhp2Nxla++5mgIMJIAx6gL8FhPFoot//2BzwdghV+D++ixqIqXC1RknNj5rnZQObWk5fgs7jV+sISYXLT6wLagMLzhjnYSOe95kb0WmFNJoPp4B71iKmVdbSIQYquYuYIVIuFVPESZ18lRRV+yCvYuVk/4F8U8IfGcDSA1IYhbDyeeJqB9W8SL2Efb9JilLGJeuaiHxUQ==</D><DP>AnJWveHj32Ne4GFppO/yCHvwVe8jmQyNoM3nHLyoWI4B3B2kawJjVfRRPSpMEFQWqE1bh9HZmEnwrJHmsNE2M8dkAWv2rES+8IyBN677Z5f+2BaXCvZCESrefQAbeBTxZqr3V3eWlvavyq/Nf/wH1HmxTuY57jVyFUwaceCvlX8=</DP><DQ>HsrP59LnEEZ37c1iHE9oqqFXE3hbh08W4ZbW/l2k1WjpWK4nYVY17NddCBGAZGW7yaV3fYj8dqTa+6LjBCdC0zCiTT8i6sDvkBwxfdv4EAX0P4Z6alDomE4Vzi4GuCUQhmnS93IowOuNug6GbBUcgPpjZFEbUgKWBOuaygbvQTE=</DQ><Exponent>AQAB</Exponent><InverseQ>BkwSNsBUSDsk5vjj6tMlouOegdT0iYnNXZ3QLo6TikGZK9N8FVV5ZYQqmNe8YxkO6/MtyKobZF2iJI+pllkg/zuEwb3HyyFsG+mR8AbNo/MDb8SevhUtU+mwNQkbz3WrW5A5b0n/uds0U7QgT8CdrecDZsRerLzYZ9N4K5LxJ48=</InverseQ><Modulus>0bOqQ3qeUzMEvT5s9StMiJbO3XSJgZ4wm4pJu9URBBzP/sqcC0ZaYqXO3L6WeZjse7iGsTsyPRELKUAG5oKjXrwfqV9SvLJQzq044+pEwScMIfXDqy6tSigMz8o/fkmncTOLLzYK43O0t1lI7FHyt/fiQOxOpzJQRYoCXmJALs+hiXYieEMOH0qEo62nbiXugU+LAPJeuoHLee3tuwXLKKM+aj9i+uDDn7QmePXwW1GNr8OZmK6uP7Wn0K0pR3ijUiz0Wbpq31O/LONStAx6cCVr+T7WOrORaMmkp/0roAmGk3gzaNk1A7uFOLOfdqu0nYYjSWwKEvWI78pGRDat0Q==</Modulus><P>3mSMrupBE0J9mo4IpVA7zZI4w87V0Hmr+icG+SwOogirLwvoFkzQ5fUPy0k5Yx3R+wjRMdgGcL6nxUON7Mq/YBPOlGe2Orvm5SxMwqUipT8AYoGk8eeC0GYiv/H21eTU5jX5DXetHIDNAOvT5Y8GNzhgyYKaQGZclHt4XAHwcR8=</P><Q>8WQnxE/UAGoUtXHpROKConsHvdbQOC4ABVHXAOB+/bnmFM7C/EBn9pud/f7KCnoA9f6QoLgJyafbwjNnWbshZE0XNTQvVpgzehz0y0CcLAjzLx5vu8Dio9k8ZQwnYpij/tjYmQWj+7jqXnIsa6cXdY35l0m6CEPb+TC9yLAtUw8=</Q></RSAParameters>");
        private static readonly RSAParameters _rsaPublicKey = BuildRsaKey("<?xml version=\"1.0\" encoding=\"utf-16\"?><RSAParameters xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"><Exponent>AQAB</Exponent><Modulus>0bOqQ3qeUzMEvT5s9StMiJbO3XSJgZ4wm4pJu9URBBzP/sqcC0ZaYqXO3L6WeZjse7iGsTsyPRELKUAG5oKjXrwfqV9SvLJQzq044+pEwScMIfXDqy6tSigMz8o/fkmncTOLLzYK43O0t1lI7FHyt/fiQOxOpzJQRYoCXmJALs+hiXYieEMOH0qEo62nbiXugU+LAPJeuoHLee3tuwXLKKM+aj9i+uDDn7QmePXwW1GNr8OZmK6uP7Wn0K0pR3ijUiz0Wbpq31O/LONStAx6cCVr+T7WOrORaMmkp/0roAmGk3gzaNk1A7uFOLOfdqu0nYYjSWwKEvWI78pGRDat0Q==</Modulus></RSAParameters>");

        /// <summary>
        /// Encrypt text with AES algorithm.
        /// </summary>
        /// <param name="plainText">Text to encrypt.</param>
        /// <returns>Encrypted text.</returns>
        public static string AesEncrypt(string plainText)
        {
            // Check arguments.
            if (plainText.IsNullOrEmpty())
                throw CustomException.Validation.PropertyIsNullOrEmpty(nameof(plainText));

            byte[] encrypted;

            // Create an Aes object
            // with the specified key and IV.
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = _aesKey;
                aesAlg.IV = new byte[16];

                // Create an encryptor to perform the stream transform.
                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for encryption.
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            //Write all data to the stream.
                            swEncrypt.Write(plainText);
                        }
                        encrypted = msEncrypt.ToArray();
                    }
                }
            }

            // Return the encrypted bytes from the memory stream.
            return Convert.ToBase64String(encrypted);
        }

        /// <summary>
        /// Decrypt text with AES algorithm.
        /// </summary>
        /// <param name="cipherText">Text to decrypt.</param>
        /// <returns>Decrypted text.</returns>
        public static string AesDecrypt(string cipherText)
        {
            // Check arguments.
            if (cipherText.IsNullOrEmpty())
                throw CustomException.Validation.PropertyIsNullOrEmpty(nameof(cipherText));

            byte[] buffer = Convert.FromBase64String(cipherText);

            // Declare the string used to hold
            // the decrypted text.
            string plaintext = null;

            // Create an Aes object
            // with the specified key and IV.
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = _aesKey;
                aesAlg.IV = new byte[16];

                // Create a decryptor to perform the stream transform.
                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for decryption.
                using (MemoryStream msDecrypt = new MemoryStream(buffer))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            // Read the decrypted bytes from the decrypting stream
                            // and place them in a string.
                            plaintext = srDecrypt.ReadToEnd();
                        }
                    }
                }
            }

            return plaintext;
        }

        /// <summary>
        /// Encrypt text with RSA algorithm.
        /// </summary>
        /// <param name="plainText">Text to encrypt.</param>
        /// <returns>Encrypted text.</returns>
        public static string RsaEncrypt(string plainText)
        {
            // Check arguments.
            if (plainText.IsNullOrEmpty())
                throw CustomException.Validation.PropertyIsNullOrEmpty(nameof(plainText));

            byte[] dataToEncrypt = Encoding.UTF8.GetBytes(plainText);

            byte[] encryptedData;
            //Create a new instance of RSACryptoServiceProvider.
            using (var rsa = new RSACryptoServiceProvider())
            {
                //Import the RSA Key information. This only needs
                //toinclude the public key information.
                rsa.ImportParameters(_rsaPublicKey);

                //Encrypt the passed byte array and specify OAEP padding.  
                //OAEP padding is only available on Microsoft Windows XP or
                //later.  
                encryptedData = rsa.Encrypt(dataToEncrypt, false);
            }
            return Convert.ToBase64String(encryptedData);
        }

        /// <summary>
        /// Decrypt text with RSA algorithm.
        /// </summary>
        /// <param name="cipherText">Text to decrypt.</param>
        /// <returns>Decrypted text.</returns>
        public static string RsaDecrypt(string cipherText)
        {
            // Check arguments.
            if (cipherText.IsNullOrEmpty())
                throw CustomException.Validation.PropertyIsNullOrEmpty(nameof(cipherText));

            byte[] dataToDecrypt = Convert.FromBase64String(cipherText);

            byte[] decryptedData;
            //Create a new instance of RSACryptoServiceProvider.
            using (var rsa = new RSACryptoServiceProvider())
            {
                //Import the RSA Key information. This needs
                //to include the private key information.
                rsa.ImportParameters(_rsaPrivateKey);

                //Decrypt the passed byte array and specify OAEP padding.  
                //OAEP padding is only available on Microsoft Windows XP or
                //later.  
                decryptedData = rsa.Decrypt(dataToDecrypt, false);
            }
            return Encoding.UTF8.GetString(decryptedData);
        }

        private static RSAParameters BuildRsaKey(string rsaKeyString)
        {
            var stringReader = new StringReader(rsaKeyString);
            //we need a deserializer
            var xmlSerializer = new XmlSerializer(typeof(RSAParameters));
            //get the object back from the stream
            var key = (RSAParameters)xmlSerializer.Deserialize(stringReader);

            return key;
        }

        private static (string, string) GenerateRsaKeyStringPair()
        {
            string privateKeyString;
            string publicKeyString;

            //lets take a new CSP with a new 2048 bit rsa key pair
            using (var rsa = new RSACryptoServiceProvider(2048))
            {
                //how to get the private key
                var privateKey = rsa.ExportParameters(true);

                //and the public key ...
                var publicKey = rsa.ExportParameters(false);

                //converting the keys into a string representation
                privateKeyString = GetRsaKeyString(privateKey);
                publicKeyString = GetRsaKeyString(publicKey);
            }

            return (privateKeyString, publicKeyString);
        }

        private static string GetRsaKeyString(RSAParameters rsaKey)
        {
            //we need some buffer
            var sw = new StringWriter();
            //we need a serializer
            var xs = new XmlSerializer(typeof(RSAParameters));
            //serialize the key into the stream
            xs.Serialize(sw, rsaKey);
            //get the string from the stream
            var rsaKeyString = sw.ToString();

            return rsaKeyString;
        }
    }
}
