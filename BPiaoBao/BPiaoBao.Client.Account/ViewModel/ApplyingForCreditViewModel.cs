using BPiaoBao.AppServices.Contracts.Cashbag;
using BPiaoBao.AppServices.DataContracts.Cashbag;
using BPiaoBao.Client.UIExt;
using BPiaoBao.Client.UIExt.Helper;
using BPiaoBao.Client.UIExt.Model;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Threading;
using JoveZhao.Framework;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPiaoBao.Client.Account.ViewModel
{
    /// <summary>
    /// 申请信用
    /// </summary>
    public class ApplyingForCreditViewModel : BaseVM
    {
        #region 成员变量

        private const int IdIndex = 0;
        private const int CerditIndex = 1;
        private const int IncomeIndex = 2;
        private const int WorkIndex = 3;
        private const int HouseIndex = 4;
        private const int CarIndex = 5;
        private const int MarrayIndex = 6;
        private const int DiplomasIndex = 7;

        #endregion

        #region 构造函数

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplyingForCreditViewModel"/> class.
        /// </summary>
        public ApplyingForCreditViewModel()
        {
            if (IsInDesignMode)
                return;

            for (int i = 0; i < _images.Length; i++)
            {
                _images[i] = new ObservableCollection<FileInfoObject>();
            }

            Initialize();
        }

        /// <summary>
        /// 初始化数据
        /// </summary>
        public override void Initialize()
        {
            IsLoading = true;
            Action action = () => CommunicateManager.Invoke<IAccountService>(service =>
            {
                var result = service.GetGrantInfo();

                DispatcherHelper.UIDispatcher.Invoke(new Action(() => SetData(result)));

            }, UIManager.ShowErr);

            Task.Factory.StartNew(action).ContinueWith(task =>
            {
                Action setBusyAction = () => { IsLoading = false; };
                DispatcherHelper.UIDispatcher.Invoke(setBusyAction);
            });
        }

        /// name="ca">信用认证(有多张图用逗号分隔,下同) 
        /// name="ra">收入认证
        /// name="wa">工作认证
        /// name="ha">房产认证
        /// name="ba">购车认证
        /// name="ma">结婚认证
        /// name="ea">教育认证
        /// <param name="result"></param>
        private void SetData(GrantInfoDto result)
        {
            //清楚所有图片
            foreach (var item in _images)
                item.Clear();

            var idUris = GetUris(result.GrantArray, "Ia");
            var cerditUris = GetUris(result.GrantArray, "Ca");
            var incomeUris = GetUris(result.GrantArray, "Ra");
            var workUris = GetUris(result.GrantArray, "Wa");
            var houseUris = GetUris(result.GrantArray, "Ha");
            var carUris = GetUris(result.GrantArray, "Ba");
            var marrayUris = GetUris(result.GrantArray, "Ma");
            var diplomasUris = GetUris(result.GrantArray, "Ea");

            SetImages(idUris, IdIndex);
            SetImages(cerditUris, CerditIndex);
            SetImages(incomeUris, IncomeIndex);
            SetImages(workUris, WorkIndex);
            SetImages(houseUris, HouseIndex);
            SetImages(carUris, CarIndex);
            SetImages(marrayUris, MarrayIndex);
            SetImages(diplomasUris, DiplomasIndex);

            ExamineMessage = null;
            //小于0表示审核未通过,0-1表示审核中,大于1表示审核通过,-2新开用户
            CanSubmit = false;
            if (result.Applystatus < 0)
            {
                StateMessage = "审核未通过";


                ExamineMessage = result.message;
                CanSubmit = true;

                if (result.Applystatus == -2)
                {
                    StateMessage = ExamineMessage = null;//新开用户不显示
                }
            }
            else if (result.Applystatus > 1)
            {
                StateMessage = "审核通过";
            }
            else
            {
                StateMessage = "审核中";
            }
        }

        private void SetImages(ICollection<string> uris, int index)
        {
            try
            {
                _images[index].Clear();
                if (uris == null || uris.Count == 0)
                    return;

                foreach (var item in uris)
                    _images[index].Add(new FileInfoObject
                    {
                        IsUploaded = true,
                        ServerAddress = item,
                        FilePath = item,
                    });
            }
            catch (Exception e)
            {
                Logger.WriteLog(LogType.ERROR,"加载图片出错",e);
            }
        }

        #endregion

        #region 公开属性

        #region IsBusy

        ///// <summary>
        ///// The <see cref="IsBusy" /> property's name.
        ///// </summary>
        //public const string IsBusyPropertyName = "IsBusy";

        //private bool isBusy = false;

        /// <summary>
        /// 是否在繁忙
        /// </summary>
        public new bool IsBusy
        {
            get { return isBusy; }

            set
            {
                if (isBusy == value) return;

                RaisePropertyChanging(IsBusyPropertyName);
                isBusy = value;
                RaisePropertyChanged(IsBusyPropertyName);

                if (_uploadCommand != null)
                    _uploadCommand.RaiseCanExecuteChanged();
            }
        }

        #endregion

        #region IsLoading

        /// <summary>
        /// The <see cref="IsLoading" /> property's name.
        /// </summary>
        private const string IsLoadingPropertyName = "IsLoading";

        private bool _isLoading;

        /// <summary>
        /// 是否正在加载
        /// </summary>
        public bool IsLoading
        {
            get { return _isLoading; }

            set
            {
                if (_isLoading == value) return;

                RaisePropertyChanging(IsLoadingPropertyName);
                _isLoading = value;
                RaisePropertyChanged(IsLoadingPropertyName);
            }
        }

        #endregion

        #region TotalFileCount

        /// <summary>
        /// The <see cref="TotalFileCount" /> property's name.
        /// </summary>
        private const string TotalFileCountPropertyName = "TotalFileCount";

        private int _totalFileCount;

        /// <summary>
        /// 总文件数
        /// </summary>
        public int TotalFileCount
        {
            get { return _totalFileCount; }

            set
            {
                if (_totalFileCount == value) return;

                RaisePropertyChanging(TotalFileCountPropertyName);
                _totalFileCount = value;
                RaisePropertyChanged(TotalFileCountPropertyName);
            }
        }

        #endregion

        #region TotalFileSize

        /// <summary>
        /// The <see cref="TotalFileSize" /> property's name.
        /// </summary>
        private const string TotalFileSizePropertyName = "TotalFileSize";

        private decimal _totalFileSize;

        /// <summary>
        /// 总文件大小  M单位
        /// </summary>
        public decimal TotalFileSize
        {
            get { return _totalFileSize; }

            set
            {
                if (_totalFileSize == value) return;

                RaisePropertyChanging(TotalFileSizePropertyName);
                _totalFileSize = value;
                RaisePropertyChanged(TotalFileSizePropertyName);
            }
        }

        #endregion

        #region Images

        /// <summary>
        /// The <see cref="Images" /> property's name.
        /// </summary>
        private const string ImagesPropertyName = "Images";

        private ObservableCollection<FileInfoObject>[] _images = new ObservableCollection<FileInfoObject>[8];

        /// <summary>
        /// 所有图片
        /// </summary>
        public ObservableCollection<FileInfoObject>[] Images
        {
            get { return _images; }

            set
            {
                if (_images == value) return;

                RaisePropertyChanging(ImagesPropertyName);
                _images = value;
                RaisePropertyChanged(ImagesPropertyName);
            }
        }

        #endregion

        #region CurrentIndex

        /// <summary>
        /// The <see cref="CurrentIndex" /> property's name.
        /// </summary>
        private const string CurrentIndexPropertyName = "CurrentIndex";

        private int _currentIndex;

        /// <summary>
        /// 当前索引
        /// </summary>
        public int CurrentIndex
        {
            get { return _currentIndex; }

            set
            {
                if (_currentIndex == value) return;

                RaisePropertyChanging(CurrentIndexPropertyName);
                _currentIndex = value;
                RaisePropertyChanged(CurrentIndexPropertyName);
            }
        }

        #endregion

        #region IsSubmiting

        /// <summary>
        /// The <see cref="IsSubmiting" /> property's name.
        /// </summary>
        private const string IsSubmitingPropertyName = "IsSubmiting";

        private bool _isSubmiting;

        /// <summary>
        /// 是否正在提交
        /// </summary>
        public bool IsSubmiting
        {
            get { return _isSubmiting; }

            set
            {
                if (_isSubmiting == value) return;

                RaisePropertyChanging(IsSubmitingPropertyName);
                _isSubmiting = value;
                RaisePropertyChanged(IsSubmitingPropertyName);

                if (_submitCommand != null)
                    _submitCommand.RaiseCanExecuteChanged();
            }
        }

        #endregion

        #region StateMessage

        /// <summary>
        /// The <see cref="StateMessage" /> property's name.
        /// </summary>
        private const string StateMessagePropertyName = "StateMessage";

        private string _stateMessage;

        /// <summary>
        /// 审核状态描述
        /// </summary>
        public string StateMessage
        {
            get { return _stateMessage; }

            set
            {
                if (_stateMessage == value) return;

                RaisePropertyChanging(StateMessagePropertyName);
                _stateMessage = value;
                RaisePropertyChanged(StateMessagePropertyName);
            }
        }

        #endregion

        #region ExamineMessage

        /// <summary>
        /// The <see cref="ExamineMessage" /> property's name.
        /// </summary>
        private const string ExamineMessagePropertyName = "ExamineMessage";

        private string _examineMessage;

        /// <summary>
        /// 审核消息内容
        /// </summary>
        public string ExamineMessage
        {
            get { return _examineMessage; }

            set
            {
                if (_examineMessage == value) return;

                RaisePropertyChanging(ExamineMessagePropertyName);
                _examineMessage = value;
                RaisePropertyChanged(ExamineMessagePropertyName);
            }
        }

        #endregion

        #region CanSubmit

        /// <summary>
        /// The <see cref="CanSubmit" /> property's name.
        /// </summary>
        private const string CanSubmitPropertyName = "CanSubmit";

        private bool _canSubmit;

        /// <summary>
        /// 是否可以申请信用
        /// </summary>
        public bool CanSubmit
        {
            get { return _canSubmit; }

            set
            {
                if (_canSubmit == value) return;

                RaisePropertyChanging(CanSubmitPropertyName);
                _canSubmit = value;
                RaisePropertyChanged(CanSubmitPropertyName);
            }
        }

        #endregion

        #endregion

        #region 公开命令

        #region AddImagesCommand

        private RelayCommand _addImagesCommand;

        /// <summary>
        /// 添加照片命令
        /// </summary>
        public RelayCommand AddImagesCommand
        {
            get
            {
                return _addImagesCommand ?? (_addImagesCommand = new RelayCommand(ExecuteAddImagesCommand, CanExecuteAddImagesCommand));
            }
        }

        private void ExecuteAddImagesCommand()
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "所有文件|*.png;*.jpg;*.jpeg|PNG文件|*.png|JPG文件|*.jpg;*.jpeg",
                Multiselect = true
            };
            var dialogResult = openFileDialog.ShowDialog();
            if (dialogResult == null || !dialogResult.Value)
                return;

            if (openFileDialog.FileNames == null)
                return;

            foreach (var fileItem in openFileDialog.FileNames)
            {
                var model = new FileInfoObject
                {
                    FileName = Path.GetFileName(fileItem),
                    FilePath = fileItem,
                    IsUploading = false
                };

                Images[CurrentIndex].Add(model);
            }

            CalcTotalCount();
        }

        private void AddFiles(ObservableCollection<FileInfoObject> targetCollection, IEnumerable<string> files)
        {
            if (files == null)
                return;

            foreach (var model in files.Select(fileItem => new FileInfoObject
            {
                FileName = Path.GetFileName(fileItem),
                FilePath = fileItem,
                IsUploading = false
            }))
            {
                targetCollection.Add(model);
            }
        }

        private bool CanExecuteAddImagesCommand()
        {
            return _currentIndex >= 0 && _canSubmit;
        }

        #endregion

        #region DeleteImageCommand

        private RelayCommand<FileInfoObject> _deleteImageCommand;

        /// <summary>
        /// 删除图片命令
        /// </summary>
        public RelayCommand<FileInfoObject> DeleteImageCommand
        {
            get
            {
                return _deleteImageCommand ?? (_deleteImageCommand = new RelayCommand<FileInfoObject>(ExecuteDeleteImageCommand, CanExecuteDeleteImageCommand));
            }
        }

        private void ExecuteDeleteImageCommand(FileInfoObject file)
        {
            if (_currentIndex < 0) return;

            if (_images[_currentIndex].Contains(file))
                _images[_currentIndex].Remove(file);


            CalcTotalCount();
        }

        private bool CanExecuteDeleteImageCommand(FileInfoObject file)
        {
            return file != null && _canSubmit;
        }

        #endregion

        #region OpenFileCommand

        private RelayCommand<FileInfoObject> _openFileCommand;

        /// <summary>
        /// 打开文件命令
        /// </summary>
        public RelayCommand<FileInfoObject> OpenFileCommand
        {
            get
            {
                return _openFileCommand ?? (_openFileCommand = new RelayCommand<FileInfoObject>(ExecuteOpenFileCommand, CanExecuteOpenFileCommand));
            }
        }

        private void ExecuteOpenFileCommand(FileInfoObject file)
        {
            try {
                //Process.Start(file.FilePath);
                UIManager.OpenDefaultBrower(file.FilePath);
            }
            catch (Exception e)
            {
                Logger.WriteLog(LogType.ERROR, "加载图片出错,地址："+file.FilePath, e);
            }
        }

        private bool CanExecuteOpenFileCommand(FileInfoObject file)
        {
            var can = file != null && !String.IsNullOrWhiteSpace(file.FilePath);
            return can;
        }

        #endregion

        #region UploadCommand

        private RelayCommand _uploadCommand;

        /// <summary>
        /// 上传命令
        /// </summary>
        public RelayCommand UploadCommand
        {
            get
            {
                return _uploadCommand ?? (_uploadCommand = new RelayCommand(ExecuteUploadCommand, CanExecuteUploadCommand));
            }
        }

        private void ExecuteUploadCommand()
        {
            IsBusy = true;
            Action action = () =>
            {
                var needUpload = _images.FirstOrDefault(m => m.FirstOrDefault(c => !c.IsUploaded) != null);
                if (needUpload == null)
                {
                    UIManager.ShowMessage("没有可上传的文件");
                    return;
                }

                for (int i = 0; i < _images.Length; i++)
                {
                    var imageList = _images[i];
                    if (imageList == null || imageList.Count == 0)
                    {
                        continue;
                    }

                    bool hadUploadItem = imageList.FirstOrDefault(m => !m.IsUploaded) != null;//是否有上传对象
                    if (!hadUploadItem)
                    {//没有需要上传的文件
                        continue;
                    }

                    CurrentIndex = i;

                    foreach (var img in imageList)
                    {
                        if (img.IsUploaded)
                            continue;

                        img.IsUploading = true;

                        var result = UploadHelper.UploadFile(img.FilePath);

                        img.ServerAddress = result.Key;
                        img.ErrorMessage = result.Value;
                        if (img.ErrorMessage == null)
                        {
                            img.SuccessMessage = "上传成功";
                            img.IsUploaded = true;
                        }
                        System.Threading.Thread.Sleep(1000);
                        img.IsUploading = false;
                    }
                }
            };

            Task.Factory.StartNew(action).ContinueWith(task =>
            {
                Action setBusyAction = () => { IsBusy = false; };
                DispatcherHelper.UIDispatcher.Invoke(setBusyAction);
            });
        }

        private bool CanExecuteUploadCommand()
        {
            return !isBusy && _totalFileCount > 0 && _canSubmit;
        }

        #endregion

        #region SubmitCommand

        private RelayCommand _submitCommand;

        /// <summary>
        /// 提交审核命令
        /// </summary>
        public RelayCommand SubmitCommand
        {
            get
            {
                return _submitCommand ?? (_submitCommand = new RelayCommand(ExecuteSubmitCommand, CanExecuteSubmitCommand));
            }
        }

        private void ExecuteSubmitCommand()
        {
            IsSubmiting = true;

            Action action = () => CommunicateManager.Invoke<IAccountService>(service =>
            {
                var idStr = GetUris(Images[IdIndex]);

                var cerditStr = GetUris(Images[CerditIndex]);

                var incomeStr = GetUris(Images[IncomeIndex]);

                var workStr = GetUris(_images[WorkIndex]);

                var houseStr = GetUris(_images[HouseIndex]);

                var carStr = GetUris(_images[CarIndex]);

                var marrayStr = GetUris(_images[MarrayIndex]);

                var diplomasStr = GetUris(_images[DiplomasIndex]);

                if (idStr == null && cerditStr == null && incomeStr == null && workStr == null && houseStr == null && carStr == null &&
                    marrayStr == null && diplomasStr == null)
                {
                    UIManager.ShowMessage("请上传证明");
                    return;
                }

                service.GrantApply(cerditStr, incomeStr, workStr, houseStr, carStr, marrayStr, diplomasStr, idStr);

                //成功
                Initialize();

            }, UIManager.ShowErr);

            Task.Factory.StartNew(action).ContinueWith(task =>
            {
                Action setBusyAction = () => { IsSubmiting = false; };
                DispatcherHelper.UIDispatcher.Invoke(setBusyAction);
            });
        }

        private bool CanExecuteSubmitCommand()
        {
            return !isBusy && !_isSubmiting && _canSubmit;
        }

        #endregion

        #region ShowSampleCommandCommand

        private RelayCommand<string> _showSampleCommandCommand;

        /// <summary>
        /// Gets the ShowSampleCommandCommand.
        /// </summary>
        public RelayCommand<string> ShowSampleCommandCommand
        {
            get
            {
                return _showSampleCommandCommand ?? (_showSampleCommandCommand = new RelayCommand<string>(ExecuteShowSampleCommandCommand, CanExecuteShowSampleCommandCommand));
            }
        }

        private void ExecuteShowSampleCommandCommand(string url)
        {
            //string runDir = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            UIManager.ShowWeb("示图", String.Format("{0}/Samples/{1}.html", "http://www.51cbc.cn", url));
        }

        private bool CanExecuteShowSampleCommandCommand(string sampleUri)
        {
            return true;
        }

        #endregion

        #endregion

        #region 私有方法

        //计算总数
        private void CalcTotalCount()
        {
            TotalFileCount = _images.Sum(m => m.Count);
            TotalFileSize = _images.Sum(list => list.Sum(fileItem =>
            {
                long result;
                if (!File.Exists(fileItem.FilePath))
                    return 0;
                using (var fileStream = File.OpenRead(fileItem.FilePath))
                {
                    result = fileStream.Length;
                }

                var mb = (decimal)result / (1024 * 1024);
                mb = Math.Round(mb, 2);
                return mb;
            }));
        }

        //获取uri串
        private string GetUris(ObservableCollection<FileInfoObject> collection)
        {
            if (collection == null || collection.Count == 0)
                return null;

            var sb = new StringBuilder();
            foreach (var item in collection)
            {
                //有错误
                if (item.ErrorMessage != null)
                    continue;

                sb.AppendFormat("{0},", item.ServerAddress);
            }

            if (sb.Length > 0)
                sb = sb.Remove(sb.Length - 1, 1);

            var result = sb.ToString();
            return string.IsNullOrWhiteSpace(result) ? null : result;
        }

        private List<string> GetUris(string uris)
        {
            if (uris == null)
                return null;

            var array = uris.Split(',');
            var result = new List<string>();
            result.AddRange(array);

            return result;
        }

        private List<string> GetUris(IEnumerable<GrantArrayDto> list, string key)
        {
            if (list == null)
                return null;

            var exist = list.FirstOrDefault(m => m.Key.ToLower() == key.ToLower());

            if (exist == null) return null;

            var result = GetUris(exist.ImageUrl);
            return result;
        }

        #endregion
    }
}
