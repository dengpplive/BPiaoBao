using System.IO;
using System.IO.IsolatedStorage;

namespace BPiaoBao.Client.UIExt.Helper
{
    /// <summary>
    /// 独立存储帮助类
    /// </summary>
    public class IsolatedStorageHelper
    {
        /// <summary>
        /// 保存一个对象，一种type的对象只保留一个
        /// </summary>
        /// <param name="model">保存的对象</param>
        public static void Save(object model)
        {
            Save(model, model.GetType().ToString());
        }

        /// <summary>
        /// 保存一个对象
        /// </summary>
        /// <param name="model">保存的对象.</param>
        /// <param name="path">保存路径.</param>
        public static void Save(object model, string path)
        {
            var store = IsolatedStorageFile.GetUserStoreForAssembly();
            var fileStream = store.CreateFile(path);

            using (StreamWriter writer = new StreamWriter(fileStream))
            {
                string xml = SerializeHelper.ObjectToXml(model);
                writer.Write(xml);
            }
        }

        /// <summary>
        /// 获取一个对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T Get<T>()
        {
            var result = Get<T>(typeof(T).ToString());
            return result;
        }

        /// <summary>
        /// 获取一个对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path">获取路径.</param>
        /// <returns></returns>
        public static T Get<T>(string path)
        {
            var store = IsolatedStorageFile.GetUserStoreForAssembly();
            try
            {
                using (StreamReader reader = new StreamReader(new IsolatedStorageFileStream(path, FileMode.Open, store)))
                {
                    string xml = reader.ReadToEnd();
                    return SerializeHelper.XmlToObject<T>(xml);
                }
            }
            catch (FileNotFoundException)
            {
                return default(T);
            }
        }

        /// <summary>
        /// 删除配置
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public static void Delete<T>()
        {
            Delete(typeof(T).ToString());
        }

        /// <summary>
        /// 删除配置
        /// </summary>
        /// <param name="path">路劲.</param>
        public static void Delete(string path)
        {
            var store = IsolatedStorageFile.GetUserStoreForAssembly();
            if (store.FileExists(path))
                store.DeleteFile(path);
        }
    }
}
