using AutoUpdate.MainService;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AutoUpdate
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly UpdateInfo _info = null;
        public MainWindow(UpdateInfo info)
        {
            InitializeComponent();
            //this.Left = SystemParameters.WorkArea.Width - this.Width;
            //this.Top = SystemParameters.WorkArea.Height - this.Height;

            this._info = info;
            this.Btn_Update_Click();
        }
        //下载更新
        private void Btn_Update_Click()
        {
            ProcessClose(_info.AppName);
            DownLoadFile();
        }
        /// <summary>
        /// 下载文件
        /// </summary>
        private void DownLoadFile()
        {

            var downloadUrl = string.Format("{0}/Home/Download", _info.Url);
            var client = new System.Net.WebClient();
            client.DownloadProgressChanged += (clientSender, clientE) =>
            {
                UpdateProcess(clientE.BytesReceived, clientE.TotalBytesToReceive);
            };
            #region

            client.DownloadDataCompleted += (clientSender, clientE) =>
            {
                string zipFilePath = DownloadDataCompleted(clientE);

                Action f = () =>
                {
                    txtProcess.Text = "开始更新程序...";
                };
                this.Dispatcher.Invoke(f);

                string tempDir = System.IO.Path.Combine(_info.LocalFilePathTemp, "TempFile");
                if (!Directory.Exists(tempDir))
                {
                    Directory.CreateDirectory(tempDir);
                }
                UnZipFile(zipFilePath, tempDir);
                UpdateDll(tempDir);
                //移动文件
                if (Directory.Exists(_info.LocalFilePath))
                {
                    CopyDirectory(tempDir, _info.AppFilePath);
                }

                f = () =>
                {
                    txtProcess.Text = "更新完成!";

                    try
                    {
                        FileStream fs = new FileStream(System.IO.Path.Combine(_info.LocalFilePath, _info.ConfigurationName), FileMode.Create);
                        StreamWriter streamWriter = new StreamWriter(fs);
                        streamWriter.Write(_info.ServerVersion);
                        streamWriter.Close();
                        fs.Close();

                        DeleteFolder(_info.LocalFilePathTemp);
                    }
                    catch (Exception ex)
                    {

                    }

                };
                this.Dispatcher.Invoke(f);
                try
                {
                    f = () =>
                    {
                        string readMe = _info.AppFilePath + @"\ReadMe.txt";
                        if (File.Exists(readMe))
                            System.Diagnostics.Process.Start("notepad.exe", readMe);
                        string exePath = System.IO.Path.Combine(_info.AppFilePath, _info.AppName + ".exe");
                        var info = new System.Diagnostics.ProcessStartInfo(exePath);
                        System.Diagnostics.Process.Start(info);
                        this.Close();
                    };
                    this.Dispatcher.Invoke(f);
                }
                catch (Exception)
                {

                    throw;
                }
            };

            #endregion
            client.DownloadDataAsync(new Uri(downloadUrl));
        }
        /// <summary>
        /// 删除文件夹和文件
        /// </summary>
        /// <param name="dir"></param>
        private void DeleteFolder(string dir)
        {
            if (Directory.Exists(dir))
            {
                foreach (string p in System.IO.Directory.GetFileSystemEntries(dir))
                {
                    if (!p.EndsWith("AutoUpdate", StringComparison.OrdinalIgnoreCase))
                    {
                        if (File.Exists(p))
                            File.Delete(p);
                        else
                            DeleteFolder(p);
                    }
                }
                Directory.Delete(dir);
            }
        }
        /// <summary>
        /// 清楚列表dll
        /// </summary>
        public void UpdateDll(string tempath)
        {
            string updatepath = System.IO.Path.Combine(tempath, "updatelist.txt");
            if (File.Exists(updatepath))
            {
                StreamReader streamReader = new StreamReader(updatepath);
                string updateStr = streamReader.ReadToEnd();
                streamReader.Close();
                if (string.IsNullOrEmpty(updateStr))
                    return;
                if (string.Equals(updateStr, "all", StringComparison.OrdinalIgnoreCase))
                {
                    DeleteFolder(_info.AppFilePath);
                }
                else
                {
                    string[] dllList = updateStr.Split(',');
                    for (int i = 0; i < dllList.Length; i++)
                    {
                        string deletepath = System.IO.Path.Combine(_info.AppFilePath, dllList[i]);
                        if (File.Exists(deletepath))
                            File.Delete(deletepath);
                        else
                            DeleteFolder(deletepath);
                    }
                }
            }
            File.Delete(updatepath);
        }
        /// <summary>
        /// 下载完成时执行
        /// </summary>
        /// <param name="clientE"></param>
        /// <returns></returns>
        private string DownloadDataCompleted(System.Net.DownloadDataCompletedEventArgs clientE)
        {
            string zipFilePath = System.IO.Path.Combine(_info.LocalFilePathTemp, "update.zip");
            byte[] data = clientE.Result;
            BinaryWriter writer = new BinaryWriter(new FileStream(zipFilePath, FileMode.OpenOrCreate));
            writer.Write(data);
            writer.Flush();
            writer.Close();
            return zipFilePath;
        }
        /// <summary>
        /// 关闭进程
        /// </summary>
        /// <param name="appName"></param>
        private void ProcessClose(string appName)
        {
            Process[] processes = Process.GetProcessesByName(appName);
            if (processes.Length > 0)
            {
                foreach (var p in processes)
                {
                    p.Kill();
                }
            }
        }
        /// <summary>
        /// 进度条
        /// </summary>
        /// <param name="current"></param>
        /// <param name="total"></param>
        private void UpdateProcess(long current, long total)
        {
            this.Dispatcher.Invoke(new Action(() =>
            {
                string status = (int)((float)current * 100 / (float)total) + "%";
                this.txtProcess.Text = status;
                rectProcess.Width = ((float)current / (float)total) * bProcess.ActualWidth;
            }));
        }
        /// <summary>
        /// 文件解压
        /// </summary>
        /// <param name="zipFilePath"></param>
        /// <param name="targetDir"></param>
        private void UnZipFile(string zipFilePath, string targetDir)
        {
            if (!File.Exists(zipFilePath))
                throw new Exception("文件不存在!");
            ICCEmbedded.SharpZipLib.Zip.FastZipEvents evt = new ICCEmbedded.SharpZipLib.Zip.FastZipEvents();
            ICCEmbedded.SharpZipLib.Zip.FastZip fz = new ICCEmbedded.SharpZipLib.Zip.FastZip(evt);
            fz.ExtractZip(zipFilePath, targetDir, "");

        }
        /// <summary>
        /// 移动文件
        /// </summary>
        /// <param name="sourceDirName"></param>
        /// <param name="destDirName"></param>
        public void CopyDirectory(string sourceDirName, string destDirName)
        {
            try
            {
                if (!Directory.Exists(destDirName))
                {
                    Directory.CreateDirectory(destDirName);
                    File.SetAttributes(destDirName, File.GetAttributes(sourceDirName));
                }
                if (destDirName[destDirName.Length - 1] != System.IO.Path.DirectorySeparatorChar)
                    destDirName = destDirName + System.IO.Path.DirectorySeparatorChar;
                string[] files = Directory.GetFiles(sourceDirName);
                foreach (string file in files)
                {
                    try
                    {
                        File.Copy(file, destDirName + System.IO.Path.GetFileName(file), true);
                        Console.WriteLine(file);
                        File.SetAttributes(destDirName + System.IO.Path.GetFileName(file), FileAttributes.Normal);
                    }
                    catch
                    {
                    }

                }
                string[] dirs = Directory.GetDirectories(sourceDirName);
                foreach (string dir in dirs)
                {
                    CopyDirectory(dir, destDirName + System.IO.Path.GetFileName(dir));
                }
            }
            catch (Exception)
            {

            }
        }
    }
}
