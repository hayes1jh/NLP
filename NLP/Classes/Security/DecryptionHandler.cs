using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace NLP
{
    public class DecryptionHandler
    {
        public static bool Decryption(string sInput, out string sOutput)
        {
            byte[] rgbKey = new byte[8] { 123, 56, 55, 23, 34, 98, 65, 69 };
            byte[] rgbIV = new byte[8] { 1, 235, 55, 23, 34, 98, 65, 69 };

            bool bResult = false;
            try
            {
                SymmetricAlgorithm objAlgorithm = SymmetricAlgorithm.Create("DES");
                ICryptoTransform objDecryptor = objAlgorithm.CreateDecryptor(rgbKey, rgbIV);

                byte[] bInput = Convert.FromBase64String(sInput);


                MemoryStream objOutput = new MemoryStream();
                using (CryptoStream objCryptoStream = new CryptoStream(objOutput, objDecryptor, CryptoStreamMode.Write))
                {
                    objCryptoStream.Write(bInput, 0, bInput.Length);
                    objCryptoStream.Flush();
                    objCryptoStream.Close();
                }

                sOutput = Encoding.UTF8.GetString(objOutput.ToArray());
                bResult = true;

            }
            catch (CryptographicException objError)
            {
                sOutput = objError.Message;
            }

            return bResult;
        }


        public static bool DecryptionWithExpiration(string sInput, out string sOutput)
        {
            bool bResult = false;
            try
            {
                string sTemp;
                if (Decryption(sInput, out sTemp))
                {
                    string[] _value = sTemp.Split('@');
                    DateTime _date = DateTime.Parse(_value[1]);

                    if (_date < DateTime.Now)
                    {
                        sOutput = "key is expired.";
                    }
                    else
                    {
                        bResult = true;
                        sOutput = _value[0];
                    }
                }
                else
                {
                    sOutput = sTemp;
                }
            }
            catch (System.Exception objError)
            {
                sOutput = objError.Message;
            }

            return bResult;
        }
    }
}
