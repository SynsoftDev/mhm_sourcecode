using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SynSecurity;

namespace MHM.Api.Helpers
{
    public static class GenerateEncryptedString
    {
        public static string GetEncryptedString(string inputString)
        {
            Random r = new Random();
            string FinalString;
            do
            {
                FinalString = r.Next(1001, 9998).ToString();
            } while (FinalString.ToString().Length != 4);
            FinalString = FinalString.Insert(2, inputString).Encrypt();
            return FinalString;
        }

        public static string GetDecryptedString(string inputString)
        {
            string FinalString;
            FinalString = inputString.Decrypt().Remove(0, 2);
            FinalString = FinalString.Remove(FinalString.Length - 2, 2);
            return FinalString;
        }
    }
}