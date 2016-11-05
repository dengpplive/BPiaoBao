using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Globalization;

namespace PnrAnalysis
{
    /// <summary>
    /// 进制转化显示
    /// </summary>
    public class JZFormat
    {
        /// <summary>
        /// 普通字符串转换为十六进制字符串
        /// </summary>
        /// <param name="strData"></param>
        /// <returns></returns>
        public static string strToHxString(string strData)
        {
            List<string> lstArr = new List<string>();
            if (!string.IsNullOrEmpty(strData))
            {
                char[] strChArr = strData.ToCharArray();
                foreach (char ch in strChArr)
                {
                    int value = Convert.ToInt32(ch);
                    lstArr.Add(string.Format("{0:X}", value));
                }
            }
            return string.Join(" ", lstArr.ToArray());
        }
        /// <summary>
        /// 十六进制字符串转换为普通字符串
        /// </summary>
        /// <param name="strData"></param>
        /// <returns></returns>
        public static string strHxStringToString(string strData)
        {
            StringBuilder sbChar = new StringBuilder();
            if (!string.IsNullOrEmpty(strData))
            {
                string[] strChArr = strData.Split(' ');
                if (strChArr.Length > 0)
                {
                    List<string> lstArr = new List<string>();
                    foreach (string ch in strChArr)
                    {
                        sbChar.Append(Char.ConvertFromUtf32(Convert.ToInt32(ch, 16)));
                    }
                }
            }
            return sbChar.ToString();
        }
        /// <summary>
        /// 将含有汉字的字符串转换为Unicode编码
        /// </summary>
        /// <param name="str">要编码的汉字字符串</param>
        /// <returns>Unicode编码的的字符串</returns>
        public static string ToUnicode(string strDATA)
        {
            byte[] bts = Encoding.Unicode.GetBytes(strDATA);
            string r = "";
            for (int i = 0; i < bts.Length; i += 2)
            {
                r += "\\u" + bts[i + 1].ToString("x").PadLeft(2, '0') + bts[i].ToString("x").PadLeft(2, '0');
            }
            return r;
        }
        /// <summary>
        /// 将含有Unicode编码的字符串转换为普通字符串
        /// </summary>
        /// <param name="str">Unicode编码字符串</param>
        /// <returns>汉字字符串</returns>
        public static string ToGB2312(string strDATA)
        {
            MatchEvaluator me = new MatchEvaluator(delegate(Match m)
            {
                string strVal = "";
                if (m.Success)
                {
                    byte[] bts = new byte[2];
                    bts[0] = (byte)int.Parse(m.Groups[2].Value, NumberStyles.HexNumber);
                    bts[1] = (byte)int.Parse(m.Groups[1].Value, NumberStyles.HexNumber);
                    strVal = Encoding.Unicode.GetString(bts);
                }
                return strVal;
            });
            return Regex.Replace(strDATA, @"\\u([\w]{2})([\w]{2})", me, RegexOptions.Compiled | RegexOptions.IgnoreCase);
        }

        /// <summary>
        /// 转全角的函数(SBC case)
        /// </summary>
        /// <param name="input">任意字符串</param>
        /// <returns>全角字符串</returns>
        ///<remarks>
        ///全角空格为12288，半角空格为32
        ///其他字符半角(33-126)与全角(65281-65374)的对应关系是：均相差65248
        ///</remarks>        
        public static string ToSBC(string input)
        {
            //半角转全角：
            char[] c = input.ToCharArray();
            for (int i = 0; i < c.Length; i++)
            {
                if (c[i] == 32)
                {
                    c[i] = (char)12288;
                    continue;
                }
                if (c[i] < 127)
                    c[i] = (char)(c[i] + 65248);
            }
            return new string(c);
        }
        /// <summary>
        /// 转半角的函数(DBC case)
        /// </summary>
        /// <param name="input">任意字符串</param>
        /// <returns>半角字符串</returns>
        ///<remarks>
        ///全角空格为12288，半角空格为32
        ///其他字符半角(33-126)与全角(65281-65374)的对应关系是：均相差65248
        ///</remarks>
        public static string ToDBC(string input)
        {
            char[] c = input.ToCharArray();
            for (int i = 0; i < c.Length; i++)
            {
                if (c[i] == 12288)
                {
                    c[i] = (char)32;
                    continue;
                }
                if (c[i] > 65280 && c[i] < 65375)
                    c[i] = (char)(c[i] - 65248);
            }
            return new string(c);
        }


        //---------------------------------------------------------------
        /// <summary>
        /// 十进制字节数组到十六制字符串转换
        /// </summary>
        /// <param name="ByteArr">字节数组</param>
        /// <param name="SplitChar">字节分割符 默认为空格</param>
        /// <returns></returns>
        public static string ByteToHxString(byte[] ByteArr, string SplitChar)
        {
            List<string> listByte = ByteToHxStringArr(ByteArr, SplitChar);
            return string.Join(SplitChar, listByte.ToArray());
        }
        /// <summary>
        /// 十进制字节数组到十六制字符串转换
        /// </summary>
        /// <param name="ByteArr">字节数组</param>
        /// <param name="SplitChar">字节分割符 默认为空格</param>
        /// <returns></returns>
        public static string ByteToHxString(byte[] ByteArr, int start, int Len, string SplitChar)
        {
            List<string> listByte = ByteToHxStringArr(ByteArr, start, Len, SplitChar);
            return string.Join(SplitChar, listByte.ToArray());
        }
        /// <summary>
        /// 十进制字节数组转换十六制字符串 返回十六制字符串数组
        /// </summary>
        /// <param name="ByteArr">字节数组</param>
        /// <param name="SplitChar">字节分割符 默认为空格</param>
        /// <returns></returns>
        public static List<string> ByteToHxStringArr(byte[] ByteArr, string SplitChar)
        {
            if (SplitChar == null)
            {
                SplitChar = " ";
            }
            List<string> listByte = new List<string>();
            if (ByteArr != null && ByteArr.Length > 0)
            {
                foreach (byte item in ByteArr)
                {
                    listByte.Add(string.Format("{0:X000}", item).PadLeft(2, '0'));
                }
            }
            return listByte;
        }
        /// <summary>
        /// 十进制字节数组转换十六制字符串 返回十六制字符串数组
        /// </summary>
        /// <param name="ByteArr">字节数组</param>
        /// <param name="SplitChar">字节分割符 默认为空格</param>
        /// <returns></returns>
        public static List<string> ByteToHxStringArr(byte[] ByteArr, int start, int Len, string SplitChar)
        {
            if (SplitChar == null)
            {
                SplitChar = " ";
            }
            List<string> listByte = new List<string>();
            if (ByteArr != null && ByteArr.Length > 0)
            {
                int max = ByteArr.Length;
                int end = start + Len;
                if (end > max)
                {
                    end = max;
                }
                if (start < 0)
                {
                    start = 0;
                }
                int i = 0;
                foreach (byte item in ByteArr)
                {
                    if (i >= start && i < end)
                    {
                        listByte.Add(string.Format("{0:X000}", item).PadLeft(2, '0'));
                    }
                    i++;
                }
            }
            return listByte;
        }
    }
}
