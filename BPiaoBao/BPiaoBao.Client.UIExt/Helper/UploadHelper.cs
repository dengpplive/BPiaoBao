using CashBag.FileEntity;
using Microsoft.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.RegularExpressions;

namespace BPiaoBao.Client.UIExt.Helper
{
    public class UploadHelper
    {
        private static string uploadUri;
        private static string viewImagUri;

        static UploadHelper()
        {
            uploadUri = System.Configuration.ConfigurationManager.AppSettings["UploadImageFilePath"];
            viewImagUri = System.Configuration.ConfigurationManager.AppSettings["ImageViewUrl"];
        }

        /// <summary>
        /// 上传图片，并返回结果，key表示路径，value表示上传结果(value不为空表示有错)
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static KeyValuePair<string, string> UploadFile(string filePath)
        {
            var serverModel = new FileInfoEntity();
            var s = Path.GetExtension(filePath);
            if (s != null)
                serverModel.FileType = s.Replace(".", "");
            serverModel.FileName = String.Format("{0}.{1}", Guid.NewGuid().ToString(), serverModel.FileType);
            serverModel.Content = ReadData(filePath);

            string key = String.Format("{0}{1}", viewImagUri, serverModel.FileName);
            string value = UploadFile(serverModel);

            var result = new KeyValuePair<string, string>(key, value);

            return result;
        }

        private static byte[] ReadData(string filePath)
        {
            byte[] result;

            if (!File.Exists(filePath))
                return null;

            using (var stream = File.OpenRead(filePath))
            {
                result = new byte[stream.Length];
                stream.Read(result, 0, result.Length);
            }
            return result;
        }

        private static string UploadFile(FileInfoEntity entity)
        {
            string errorMsg = null;
            IFormatter formatter = new BinaryFormatter();
            MemoryStream ms = new MemoryStream();
            formatter.Serialize(ms, entity);
            ms.Position = 0;

            var content = HttpContent.Create(ms, "application/octet-stream", ms.Length);
            var client = new HttpClient();

            using (var response = client.Post(uploadUri, content))
            {
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    response.Content.LoadIntoBuffer();
                }
                else
                {
                    errorMsg = response.Content.ReadAsString();
                    Regex rg = new Regex(@"<P><B>Details[:：][\s]*<\/B>([^\/]+)</P>");
                    if (rg.Match(errorMsg).Success)
                    {
                        errorMsg = rg.Match(errorMsg).Result("$1");
                    }
                }
            }
            return errorMsg;
        }
    }
}
