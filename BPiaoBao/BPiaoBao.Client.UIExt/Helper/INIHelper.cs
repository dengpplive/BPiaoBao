using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace BPiaoBao.Client.UIExt.Helper
{
    /// <summary>
    /// INI配置文件读写帮助类
    /// </summary>
    public class IniHelper
    {
        private string _filePath;

        public string FilePath
        {
            get { return _filePath; }
            set { _filePath = value; }
        }
        public IniHelper(string filePath)
        {
            _filePath = filePath;
            if (!IsIniFileExists())
            {
                File.Create(_filePath);
            }
        }
       

        /// <summary>
        /// 写入Ini配置信息
        /// </summary>
        /// <param name="section"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void WriteInfo(string section, string key, string value)
        {
            WritePrivateProfileString(section, key, value, _filePath);
        }

        /// <summary>
        /// 读取Ini配置文件信息
        /// </summary>
        /// <param name="section"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public string ReadIniInfo(string section, string key)
        {
            var sb=new StringBuilder();

            GetPrivateProfileString(section, key, "", sb, 65535, _filePath);

            return sb.ToString();
        }

        /// <summary>
        /// 判断文件是否存在
        /// </summary>
        /// <returns></returns>
        public bool IsIniFileExists()
        {
            return File.Exists(_filePath);
        }

        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);

        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);
    }
}
