using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Nebular.Core.Cryptography
{
    public class Hash
    {
        public static char[] CharactersArray = new char[]
        {
        'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p',
        'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', 'A', 'B', 'C', 'D', 'E', 'F',
        'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V',
        'W', 'X', 'Y', 'Z', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '-', '_'
        };


        public static int GetDirNum(string dirChar)
        {
            var hash = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_";
            return hash.IndexOf(dirChar);
        }

        public static int CharToCell(string cellCode)
        {
            char char1 = cellCode.ToCharArray()[0];
            char char2 = cellCode.ToCharArray()[1];
            short code1 = 0;
            short code2 = 0;
            short a = 0;
            while (a < CharactersArray.Count())
            {
                if (CharactersArray[a] == char1)
                {
                    code1 = (short)(a * 64);
                }
                if (CharactersArray[a] == char2)
                {
                    code2 = a;
                }
                a = (short)(a + 1);
            }
            return (short)(code1 + code2);
        }


        public static char GetDirChar(int dirNum)
        {
            var hash = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_";

            if (dirNum >= hash.Length)
                return '$';

            return hash[dirNum];
        }

        /** Old Method **/
        public static string EncryptPassword(string password, string key)
        {
            StringBuilder str = new StringBuilder().Append("#1");
            for (int i = 0; i < password.Length; i++)
            {
                char ch = password[i];
                char ch2 = key[i];
                int num2 = ch / 16;
                int num3 = ch % 16;
                int index = (num2 + ch2) % CharactersArray.Length;
                int num5 = (num3 + ch2) % CharactersArray.Length;
                str.Append(CharactersArray[index]).Append(CharactersArray[num5]);
            }
            return str.ToString();
        }

        public static string DecryptIp(string packet)
        {
            StringBuilder ip = new StringBuilder();

            for (int i = 0; i < 8; i += 2)
            {
                int ascii1 = packet[i] - 48;
                int ascii2 = packet[i + 1] - 48;

                if (i != 0)
                    ip.Append('.');

                ip.Append(((ascii1 & 15) << 4) | (ascii2 & 15));
            }
            return ip.ToString();
        }

        public static int DecryptPort(char[] chars)
        {
            if (chars.Length != 3)
                throw new ArgumentOutOfRangeException("The port must be encrypted in 3 characters.");

            int port = 0;
            for (int i = 0; i < 2; i++)
                port += (int)(Math.Pow(64, 2 - i) * GetHash(chars[i]));

            port += GetHash(chars[2]);
            return port;
        }

        public static short GetHash(char ch)
        {
            for (short i = 0; i < CharactersArray.Length; i++)
                if (CharactersArray[i] == ch)
                    return i;

            throw new IndexOutOfRangeException(ch + " is not in the hash array.");
        }

        public static string GetCellChar(short cellId) => CharactersArray[cellId / 64] + "" + CharactersArray[cellId % 64];

        public static short GetCellIdFromHash(string cellCode)
        {
            char char1 = cellCode[0], char2 = cellCode[1];
            short code1 = 0, code2 = 0, a = 0;

            while (a < CharactersArray.Length)
            {
                if (CharactersArray[a] == char1)
                    code1 = (short)(a * 64);

                if (CharactersArray[a] == char2)
                    code2 = a;

                a++;
            }
            return (short)(code1 + code2);
        }

        public static int GetNewRandom(int min, int max)
        {
            int seed = Convert.ToInt32(Regex.Match(Guid.NewGuid().ToString(), @"\d+").Value);
            return new Random(seed).Next(min, max);
        }
    }

}
