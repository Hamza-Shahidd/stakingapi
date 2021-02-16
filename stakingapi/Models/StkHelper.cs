using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Web;

namespace stakingapi.Models
{
    public class StkHelper
    {
        public static string CalcHMACSHA256Hash(string plaintext, string salt)
        {
            string result = "";
            var enc = Encoding.Default;
            byte[]
            baText2BeHashed = enc.GetBytes(plaintext),
            baSalt = enc.GetBytes(salt);
            System.Security.Cryptography.HMACSHA256 hasher = new System.Security.Cryptography.HMACSHA256(baSalt);
            byte[] baHashedText = hasher.ComputeHash(baText2BeHashed);
            result = string.Join("", baHashedText.ToList().Select(b => b.ToString("x2")).ToArray());
            return result;
        }
        public static bool checkTxHash(txencrypted txdetails)
        {
            if (txdetails.transactionHash.key == CalcHMACSHA256Hash(txdetails.transactionHash.txhash, ConfigurationManager.AppSettings["sha256privatekey"].ToString())
               && txdetails.blockNumber.key == CalcHMACSHA256Hash(txdetails.blockNumber.BN.ToString(), ConfigurationManager.AppSettings["sha256privatekey"].ToString())
               && txdetails.gasPrice.key == CalcHMACSHA256Hash(txdetails.gasPrice.GasPrice.ToString(), ConfigurationManager.AppSettings["sha256privatekey"].ToString())
               && txdetails.gasLimit.key == CalcHMACSHA256Hash(txdetails.gasLimit.GasLimit.ToString(), ConfigurationManager.AppSettings["sha256privatekey"].ToString())
               )
            {
                return true;
            }
            else
            {
                return false;
            }

        }
    }
}