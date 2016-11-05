using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Windows.Documents;

namespace ProjectHelper.Utils
{
    public static class RandomHelper
    {
        public static string GetRandomStringEx(this Random random, int num = 10, RandomStrType randomStrType = RandomStrType.En)
        {
            if (num <= 0)
            {
                throw new ArgumentException("num<=0");
            }

            return string.Concat(Enumerable.Range(0, num).Select(p =>
            {
                char c;
                switch (randomStrType)
                {
                    case RandomStrType.Number:
                        c = (char)(random.Next('0', '9'));
                        break;
                    case RandomStrType.En:
                    default:
                        c = (char)(random.Next('a', 'z' + 1));
                        break;
                }
                return c;
            }
                ));
        }

        public static T GetRandomEumEx<T>(this Random random) where T : struct
        {
            var buf = GetEnumValuesEx<T>();
            return buf[random.Next(0, buf.Count)];
        }

        public static List<T> GetEnumValuesEx<T>() where T : struct
        {
            var buf = Enum.GetValues(typeof(T)).Cast<T>().ToList();
            return buf;
        }
    }

    public enum RandomStrType
    {
        En,
        Number,
    }
}
