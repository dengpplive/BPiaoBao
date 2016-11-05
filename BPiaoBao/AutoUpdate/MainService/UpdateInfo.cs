using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoUpdate.MainService
{
    public class UpdateInfo
    {
        /// <summary>
        /// 应用程序名称
        /// </summary>
        public string AppName { get; set; }
        /// <summary>
        /// 服务更新地址
        /// </summary>
        public string Url { get; set; }
        /// <summary>
        /// 服务器版本
        /// </summary>
        public string ServerVersion { get; set; }
        /// <summary>
        /// 更新文件地址
        /// </summary>
        public string LocalFilePath { get; set; }
        /// <summary>
        /// 临时文件位置
        /// </summary>
        public string LocalFilePathTemp
        {
            get
            {
                return System.IO.Path.Combine(LocalFilePath, "Temp");
            }
        }
        /// <summary>
        /// 配置文件名称
        /// </summary>
        public string ConfigurationName { get; set; }
        /// <summary>
        /// 应用程序路径
        /// </summary>
        public string AppFilePath
        {
            get
            {
                return LocalFilePath.Substring(0, LocalFilePath.LastIndexOf(System.IO.Path.DirectorySeparatorChar));
            }
        }
    }
}
