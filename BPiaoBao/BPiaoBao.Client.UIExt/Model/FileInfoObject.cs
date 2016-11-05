using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.Client.UIExt.Model
{
    /// <summary>
    /// 文件详细
    /// </summary>
    public class FileInfoObject : ObservableObject
    {
        #region IsUploading

        /// <summary>
        /// The <see cref="IsUploading" /> property's name.
        /// </summary>
        public const string IsUploadingPropertyName = "IsUploading";

        private bool isUploading = false;

        /// <summary>
        /// 是否正在上传 或 下载
        /// </summary>
        public bool IsUploading
        {
            get { return isUploading; }

            set
            {
                if (isUploading == value) return;

                RaisePropertyChanging(IsUploadingPropertyName);
                isUploading = value;
                RaisePropertyChanged(IsUploadingPropertyName);

                if (value)
                    ErrorMessage = SuccessMessage = null;
            }
        }

        #endregion

        #region IsUploaded

        /// <summary>
        /// The <see cref="IsUploaded" /> property's name.
        /// </summary>
        public const string IsUploadedPropertyName = "IsUploaded";

        private bool isUploaded = false;

        /// <summary>
        /// 是否已经上传成功
        /// </summary>
        public bool IsUploaded
        {
            get { return isUploaded; }

            set
            {
                if (isUploaded == value) return;

                RaisePropertyChanging(IsUploadedPropertyName);
                isUploaded = value;
                RaisePropertyChanged(IsUploadedPropertyName);
            }
        }

        #endregion

        #region FilePath

        /// <summary>
        /// The <see cref="FilePath" /> property's name.
        /// </summary>
        public const string FilePathPropertyName = "FilePath";

        private string filePath = null;

        /// <summary>
        /// 文件路径
        /// </summary>
        public string FilePath
        {
            get { return filePath; }

            set
            {
                if (filePath == value) return;

                RaisePropertyChanging(FilePathPropertyName);
                filePath = value;
                RaisePropertyChanged(FilePathPropertyName);
            }
        }

        #endregion

        #region FileName

        /// <summary>
        /// The <see cref="FileName" /> property's name.
        /// </summary>
        public const string FileNamePropertyName = "FileName";

        private string fileName = null;

        /// <summary>
        /// 文件名
        /// </summary>
        public string FileName
        {
            get { return fileName; }

            set
            {
                if (fileName == value) return;

                RaisePropertyChanging(FileNamePropertyName);
                fileName = value;
                RaisePropertyChanged(FileNamePropertyName);
            }
        }

        #endregion

        #region ServerAddress

        /// <summary>
        /// The <see cref="ServerAddress" /> property's name.
        /// </summary>
        public const string ServerAddressPropertyName = "ServerAddress";

        private string serverAddress = null;

        /// <summary>
        /// 服务端地址
        /// </summary>
        public string ServerAddress
        {
            get { return serverAddress; }

            set
            {
                if (serverAddress == value) return;

                RaisePropertyChanging(ServerAddressPropertyName);
                serverAddress = value;
                RaisePropertyChanged(ServerAddressPropertyName);
            }
        }

        #endregion

        #region ErrorMessage

        /// <summary>
        /// The <see cref="ErrorMessage" /> property's name.
        /// </summary>
        public const string ErrorMessagePropertyName = "ErrorMessage";

        private string errorMessage = null;

        /// <summary>
        /// 错误消息
        /// </summary>
        public string ErrorMessage
        {
            get { return errorMessage; }

            set
            {
                if (errorMessage == value) return;

                RaisePropertyChanging(ErrorMessagePropertyName);
                errorMessage = value;
                RaisePropertyChanged(ErrorMessagePropertyName);
            }
        }

        #endregion

        #region SuccessMessage

        /// <summary>
        /// The <see cref="SuccessMessage" /> property's name.
        /// </summary>
        public const string SuccessMessagePropertyName = "SuccessMessage";

        private string successMessage = null;

        /// <summary>
        /// 成功消息
        /// </summary>
        public string SuccessMessage
        {
            get { return successMessage; }

            set
            {
                if (successMessage == value) return;

                RaisePropertyChanging(SuccessMessagePropertyName);
                successMessage = value;
                RaisePropertyChanged(SuccessMessagePropertyName);
            }
        }

        #endregion
    }
}
