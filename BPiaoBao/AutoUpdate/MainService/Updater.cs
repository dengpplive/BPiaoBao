using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Linq;
using System.Net;
using System.Reflection;

namespace AutoUpdate.MainService
{
    public class Updater
    {
        public void CheckUpdateStatus(string appName, string url)
        {
            UpdateInfo updateInfo = new UpdateInfo();
            updateInfo.ConfigurationName = "Version.txt";
            updateInfo.AppName = appName;
            updateInfo.Url = url;
            updateInfo.LocalFilePath = Assembly.GetExecutingAssembly().Location.Substring(0, Assembly.GetExecutingAssembly().Location.LastIndexOf(Path.DirectorySeparatorChar));
            StreamReader streamReader = new StreamReader(Path.Combine(updateInfo.LocalFilePath, updateInfo.ConfigurationName));
            string localVersion = streamReader.ReadToEnd();
            streamReader.Close();
            updateInfo.ServerVersion = CheckServerVersion(url, localVersion);
            if (string.IsNullOrEmpty(updateInfo.ServerVersion) || updateInfo.ServerVersion == "F")
                return;
            this.StartUpdate(updateInfo);
        }
        string CheckServerVersion(string url, string localVersion)
        {
            string serverUrl = string.Format("{0}/Home/CheckUpdate?version={1}", url, localVersion);
            return serverUrl.Get();
        }
        private void StartUpdate(UpdateInfo info)
        {
            //更新程序复制到缓存文件夹
            string updateFileDir = info.LocalFilePathTemp;
            if (!Directory.Exists(updateFileDir))
            {
                Directory.CreateDirectory(updateFileDir);
            }
            App app = new App();
            MainWindow mainWindow = new MainWindow(info);
            app.Run(mainWindow);
        }
    }
}
