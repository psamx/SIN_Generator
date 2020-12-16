using System;
using System.Linq;
using System.Security.Cryptography ;
using System.Text;
/// <summary>
/// https://en.bitcoin.it/wiki/Identity_protocol_v1#Creating_a_SIN
/// </summary>
namespace SIN_Generator
{
    class Program
    {
        static void Main(string[] args)
        {
            string Prefix = "0F"; //Prefix = 0x0F
            string Type = "02"; //SIN_Type = [0x01 | 0x02 | 0x11] -- See above for discussion of SIN types. 
            string PublicKey = "02F840A04114081690223B7069071A70D6DABB891763B638CC20C7EC3BD58E6C86"; //Your public key

            var step1 = SHA256HexHashString(PublicKey);
            Console.WriteLine(SHA256HexHashString(step1));

            var step2 = RIPEMD160HashString(step1);
            Console.WriteLine(step2);

            var step3 = Prefix + Type + step2;
            Console.WriteLine(step3);

            var step4 = SHA256HexHashString(SHA256HexHashString(step3));
            Console.WriteLine(step4);

            var step5 = step4.Substring(0, 8);
            Console.WriteLine(step5);

            var step6 = step3 + step5;
            Console.WriteLine(step6);

            var SIN = Base58Check.Base58CheckEncoding.EncodePlain(StringToByteArray(step6));
            Console.WriteLine(SIN);

            Console.ReadLine();
        }
        private static string SHA256HexHashString(string StringIn)
        {
            string hashString;
            using (var sha256 = SHA256.Create())
            {
                var hash = sha256.ComputeHash(StringToByteArray(StringIn));
                hashString = ToHex(hash, false);
            }

            return hashString;
        }

        private static string RIPEMD160HashString(string StringIn)
        {
            string hashString = string.Empty;
            using(var ripemd160 = RIPEMD160.Create())
            {
                var hash = ripemd160.ComputeHash(StringToByteArray(StringIn));
                hashString = ToHex(hash, false);
            }
            return hashString;
        }


        public static byte[] StringToByteArray(string hex)
        {
            return Enumerable.Range(0, hex.Length)
                         .Where(x => x % 2 == 0)
                         .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                         .ToArray();
        }
        private static string ToHex(byte[] bytes, bool upperCase)
        {
            StringBuilder result = new StringBuilder(bytes.Length * 2);
            for (int i = 0; i < bytes.Length; i++)
                result.Append(bytes[i].ToString(upperCase ? "X2" : "x2"));
            return result.ToString();
        }
    }
}
