/***********************************************************************   
* Copyright(c)    
* CLR 版本: 4.0.30319.34014   
* 命名空间: BPiaoBao.Common.DESCrypt
* 文 件 名：DESCryptHelper.cs   
* 创 建 人：duanwei   
* 创建日期：2014/11/14 16:31:39       
* 备注描述：           
************************************************************************/
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace BPiaoBao.Common.DESCrypt
{
    /// <summary>
    /// DESC加密解密算法
    /// </summary>
    public class DESCryptHelper
    {
        /// <summary>
        /// 默认密钥向量
        /// </summary>
        private static byte[] DefaultIVKeys = { 0x12, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xCD, 0xEF };
        /// <summary>
        /// 默认不足8位时添加的字符
        /// </summary>
        private const string CompletionDefaultKey = "@";

        /// <summary>
        /// 根据数字获取字符串
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        private static string GetCompletionDefaultKey(int length)
        {
            var sb = new StringBuilder();
            for (var i = 0; i < length; i++)
            {
                sb.Append(CompletionDefaultKey);
            }
            return sb.ToString();
        }

        /// <summary>
        /// 补全加密解密Key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        private static string CompletionKey(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                return GetCompletionDefaultKey(8);
            }
            if (key.Length > 8)
            {
                key = key.Substring(0, 8);
            }
            else if (key.Length < 8)
            {
                switch (key.Length)
                {
                    case 1:
                        key = key + GetCompletionDefaultKey(7);
                        break;
                    case 2:
                        key = key + GetCompletionDefaultKey(6);
                        break;
                    case 3:
                        key = key + GetCompletionDefaultKey(5);
                        break;
                    case 4:
                        key = key + GetCompletionDefaultKey(4);
                        break;
                    case 5:
                        key = key + GetCompletionDefaultKey(3);
                        break;
                    case 6:
                        key = key + GetCompletionDefaultKey(2);
                        break;
                    case 7:
                        key = key + GetCompletionDefaultKey(1);
                        break;
                }
            }

            return key;
        }

        /// <summary>
        /// DES 加密(数据加密标准，速度较快，适用于加密大量数据的场合)
        /// </summary>
        /// <param name="encryptString">待加密的密文</param>
        /// <param name="encryptKey">加密的密钥</param>
        /// <returns>returns</returns>
        public static string DesEncrypt(string encryptString, string encryptKey)
        {
            //if (string.IsNullOrEmpty(EncryptString)) { throw (new Exception("密文不得为空")); }
            //if (string.IsNullOrEmpty(EncryptKey)) { throw (new Exception("密钥不得为空")); }
            //if (EncryptKey.Length != 8) { throw (new Exception("密钥必须为8位")); } 

            encryptKey = CompletionKey(encryptKey);
            var mStrEncrypt = "";
            var mDesProvider = new DESCryptoServiceProvider();
            try
            {
                var mBtEncryptString = Encoding.Default.GetBytes(encryptString);
                var mStream = new MemoryStream();
                var mCstream = new CryptoStream(mStream,mDesProvider.CreateEncryptor(Encoding.Default.GetBytes(encryptKey), DefaultIVKeys), CryptoStreamMode.Write);
                mCstream.Write(mBtEncryptString, 0, mBtEncryptString.Length);
                mCstream.FlushFinalBlock();
                mStrEncrypt = Convert.ToBase64String(mStream.ToArray());
                mStream.Close();
                mStream.Dispose();
                mCstream.Close();
                mCstream.Dispose();
            }
            catch
            {

            }
            finally
            {
                mDesProvider.Clear();

            }
            return mStrEncrypt;
        }


        /// <summary>
        /// DES 解密(数据加密标准，速度较快，适用于加密大量数据的场合)
        /// </summary>
        /// <param name="decryptString">待解密的密文</param>
        /// <param name="decryptKey">解密的密钥</param>
        /// <returns>returns</returns>
        public static string DesDecrypt(string decryptString, string decryptKey)
        {
            //if (string.IsNullOrEmpty(DecryptString)) { throw (new Exception("密文不得为空")); }
            //if (string.IsNullOrEmpty(DecryptKey)) { throw (new Exception("密钥不得为空")); }
            //if (DecryptKey.Length != 8) { throw (new Exception("密钥必须为8位")); } 

            decryptKey = CompletionKey(decryptKey);
            var mStrDecrypt = "";
            var mDesProvider = new DESCryptoServiceProvider();
            try
            {
                var mBtDecryptString = Convert.FromBase64String(decryptString);
                var mStream = new MemoryStream();
                var mCstream = new CryptoStream(mStream,mDesProvider.CreateDecryptor(Encoding.Default.GetBytes(decryptKey), DefaultIVKeys), CryptoStreamMode.Write);
                mCstream.Write(mBtDecryptString, 0, mBtDecryptString.Length);
                mCstream.FlushFinalBlock();
                mStrDecrypt = Encoding.Default.GetString(mStream.ToArray());
                mStream.Close();
                mStream.Dispose();
                mCstream.Close();
                mCstream.Dispose();
            }
            catch
            {

            }
            finally
            {
                mDesProvider.Clear();
            }
            return mStrDecrypt;
        }

    }
}
