using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SynSecurity
{
   
        public static class EncyptDecrypt
        {


            public static string Encrypt(this String Plaintext)
            {
                string TrippleDESEncryptedOutput = "";
                try
                {
                    byte[] Buffer = new byte[] {
                    0};
                    System.Security.Cryptography.TripleDESCryptoServiceProvider DES = new System.Security.Cryptography.TripleDESCryptoServiceProvider();
                    System.Security.Cryptography.MD5CryptoServiceProvider hashMD5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
                    DES.Key = hashMD5.ComputeHash(System.Text.ASCIIEncoding.ASCII.GetBytes("252121112237244914192"));
                    DES.Mode = System.Security.Cryptography.CipherMode.ECB;
                    System.Security.Cryptography.ICryptoTransform DESEncrypt = DES.CreateEncryptor();
                    Buffer = System.Text.ASCIIEncoding.ASCII.GetBytes(Plaintext);
                    TrippleDESEncryptedOutput = Convert.ToBase64String(DESEncrypt.TransformFinalBlock(Buffer, 0, Buffer.Length));
                    return TrippleDESEncryptedOutput;
                }
                catch (Exception ex)
                {
                    throw ex;
                }

            }

            public static string Decrypt(this String base64Text)
            {
                string TripleDESDecyrptedOutput = "";
                try
                {
                    byte[] Buffer = new byte[] {
                    0};
                    System.Security.Cryptography.TripleDESCryptoServiceProvider DES = new System.Security.Cryptography.TripleDESCryptoServiceProvider();
                    System.Security.Cryptography.MD5CryptoServiceProvider hashMD5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
                    DES.Key = hashMD5.ComputeHash(System.Text.ASCIIEncoding.ASCII.GetBytes("252121112237244914192"));
                    DES.Mode = System.Security.Cryptography.CipherMode.ECB;
                    System.Security.Cryptography.ICryptoTransform DESDecrypt = DES.CreateDecryptor();
                    Buffer = Convert.FromBase64String(base64Text);
                    TripleDESDecyrptedOutput = System.Text.ASCIIEncoding.ASCII.GetString(DESDecrypt.TransformFinalBlock(Buffer, 0, Buffer.Length));
                    return TripleDESDecyrptedOutput;
                }
                catch (Exception ex)
                {
                    throw ex;
                }

            }

        }
    }


