using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace NLP
{ 
    public class EncryptionHandler
    {
        public static bool Encryption(string sInput, out string sOutput)
        {
            byte[] rgbKey = new byte[8] { 123, 56, 55, 23, 34, 98, 65, 69 };
            byte[] rgbIV = new byte[8] { 1, 235, 55, 23, 34, 98, 65, 69 };

            bool bResult = false;
            try
            {
                SymmetricAlgorithm objAlgorithm = SymmetricAlgorithm.Create("DES");
                ICryptoTransform objEncryptor = objAlgorithm.CreateEncryptor(rgbKey, rgbIV);

                byte[] bInput = Encoding.UTF8.GetBytes(sInput);

                MemoryStream objOutput = new MemoryStream();
                using (CryptoStream objCryptoStream = new CryptoStream(objOutput, objEncryptor, CryptoStreamMode.Write))
                {
                    objCryptoStream.Write(bInput, 0, bInput.Length);
                    objCryptoStream.Flush();
                    objCryptoStream.Close();
                }

                sOutput = Convert.ToBase64String(objOutput.ToArray());
                bResult = true;
            }
            catch (CryptographicException objError)
            {
                sOutput = objError.Message;
            }

            return bResult;
        }

        public static bool EncryptionWithExpiration(string sInput, int iInput, out string sOutput)
        {
            bool bResult = false;

            try
            {
                bResult = Encryption(sInput + "@" + DateTime.Now.AddDays(iInput).ToShortDateString(), out sOutput);
            }
            catch (Exception objError)
            {
                sOutput = objError.Message;
            }

            return bResult;
        }
    }
}
