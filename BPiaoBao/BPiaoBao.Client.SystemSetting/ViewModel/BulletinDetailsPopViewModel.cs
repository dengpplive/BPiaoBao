using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Documents;
using System.Xml;
using BPiaoBao.AppServices.Contracts.SystemSetting;
using BPiaoBao.AppServices.DataContracts.SystemSetting;
using BPiaoBao.Client.UIExt;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Threading;

namespace BPiaoBao.Client.SystemSetting.ViewModel
{
    public class BulletinDetailsPopViewModel : BaseVM
    {

        #region 私有属性

        private int[] _ids;
        //private int _index = 1;
        //private Timer _timeShowTimer;
        private int _countDown = 11;
        #endregion


        #region 初始数据
        internal void InitData(int[] ids)
        {
            _ids = ids;
            /////////////////////////////////////
            //_timeShowTimer = new Timer();
            //_timeShowTimer.Interval = 1000;
            //_timeShowTimer.Elapsed += _timeShowTimer_Elapsed;
            if (ids.Length > 1)
            {
                GetNoticeInfo(ids[0]);
                //IsShowCloseButton = false;
                IsShowNextButton = true;
                //IsShowTime = true;
            }
            else if (ids.Length == 1)
            {
                GetNoticeInfo(ids[0]);
                //IsShowCloseButton = false;
                IsShowNextButton = false;
                //IsShowTime = true;
            }
            NoticeIndex = 0;
            //_timeShowTimer.Start();
            IsShowCloseButton = true;
        }

        /// <summary>
        /// 每秒钟显示（倒计时）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void _timeShowTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            DispatcherHelper.UIDispatcher.Invoke(new Action(() =>
            {
                _countDown--;
                CountDownTime = _countDown + " 秒";
                if (_countDown >= 0) return;
                CountDownTime = "0 秒";
                IsShowCloseButton = true;
                IsShowTime = false;
                //_timeShowTimer.Stop();
            }));
        }

        internal void InitData(NoticeDto model)
        {
            DispatcherHelper.UIDispatcher.Invoke(new Action(() =>
            {
                Title = model.Title;
                CreateTime = model.CreateTime;
                CreateName = model.CreateName;
                EffectiveStartTime = model.EffectiveStartTime;
                EffectiveEndTime = model.EffectiveEndTime;
                NoticeAttachmentDtos = model.NoticeAttachments;
                var titleHtml = "<div style='text-align:center;font-size:20px;font-weight:bold;margin:10px'>" + Title +
                                "</div>";
                var dateHtml = "<div style='text-align:center;color:gray;font-size:12px'>有效期：" + EffectiveStartTime +
                             " 至 " + EffectiveEndTime + "</div>";
                var download = new StringBuilder();
                foreach (var m in model.NoticeAttachments)
                {
                    download.AppendFormat("<a href='" + m.Url + "' title='" + m.Name + "' target='_blank'>" + m.Name + "</a> <br/> ");
                }

                HtmlString =
                    string.Format(
                        "<html><head><meta http-equiv=\"X-UA-Compatible\" content=\"IE=9\" />" +
                        "<meta http-equiv=\"content-type\" content=\"text/html;charset=utf-8\" /></head>" +
                        "<body>{0}{1}{2}<div style='text-align:right;padding:20px; margin-top:50px;font-size:12px'>" +
                        "{3}</div><div style='margin-top:50px'>{4}</div></body></html>", titleHtml, dateHtml,
                        model.Contents, model.CreateTime.ToString("yyyy-MM-dd HH:mm:ss"), download);

                if (model.NoticeAttachments == null || model.NoticeAttachments.Count < 1)
                {
                    IsShowAttachMent = false;
                }
                else
                {
                    IsShowAttachMent = true;
                }
            }));
        }

        #endregion

        #region 私有方法

        /// <summary>
        /// 获取单条通知信息
        /// </summary>
        /// <param name="id"></param>
        private void GetNoticeInfo(int id)
        {
            DispatcherHelper.UIDispatcher.Invoke(new Action(() =>
            {
                Action action = () =>
                {
                    IsBusy = true;
                    CommunicateManager.Invoke<INoticeService>(service =>
                    {
                        var model = service.FindNoticeById(id);
                        if (model != null)
                        {
                            InitData(model);
                        }
                    }, UIManager.ShowErr);
                };

                Task.Factory.StartNew(action).ContinueWith(task =>
                {
                    Action setAction = () =>
                    {
                        IsBusy = false;
                    };
                    DispatcherHelper.UIDispatcher.Invoke(setAction);
                });
            }));
        }

        /// <summary>
        /// 为节点添加命令
        /// </summary>
        /// <param name="xaml"></param>
        /// <returns></returns>
        private static string AddAttributes(string xaml)
        {
            var doc = new XmlDocument();
            doc.LoadXml(xaml);
            var list = doc.GetElementsByTagName("Hyperlink");
            foreach (XmlNode node in list)
            {
                if (node.Attributes == null || node.Attributes.Count <= 0) continue;
                var value = node.Attributes["NavigateUri"].Value;
                if (string.IsNullOrEmpty(value)) continue;
                var attrCommand = doc.CreateAttribute("Command");
                attrCommand.Value = "{Binding OpenLink}";
                var attrParams = doc.CreateAttribute("CommandParameter");
                attrParams.Value = "{Binding RelativeSource={RelativeSource Self}}";//绑定自身对象
                node.Attributes.Append(attrCommand);
                node.Attributes.Append(attrParams);
            }
            return doc.InnerXml;
        }

        #endregion


        #region 公开属性

        #region HtmlString

        public string HtmlStringPropertyName = "HtmlString";
        private string _htmlString;

        public string HtmlString
        {
            get { return _htmlString; }
            set
            {
                if (_htmlString == value) return;
                RaisePropertyChanging(HtmlStringPropertyName);
                _htmlString = value;
                RaisePropertyChanged(HtmlStringPropertyName);
            }
        }

        #endregion


        #region IsShowTime

        public string IsShowTimePropertyName = "IsShowTime";
        private bool _isShowTime;
        /// <summary>
        /// 是否显示倒计时
        /// </summary>
        public bool IsShowTime
        {
            get { return _isShowTime; }
            set
            {
                if (_isShowTime == value) return;
                RaisePropertyChanging(IsShowTimePropertyName);
                _isShowTime = value;
                RaisePropertyChanged((IsShowTimePropertyName));
            }

        }

        #endregion

        #region IsShowCloseButton

        public string IsShowCloseButtonPropertyName = "IsShowCloseButton";
        private bool _isShowCloseButton;
        /// <summary>
        /// 是否显示关闭按钮
        /// </summary>
        public bool IsShowCloseButton
        {
            get { return _isShowCloseButton; }
            set
            {
                if (_isShowCloseButton == value) return;
                RaisePropertyChanging(IsShowCloseButtonPropertyName);
                _isShowCloseButton = value;
                RaisePropertyChanged((IsShowCloseButtonPropertyName));
            }

        }

        #endregion

        #region IsShowNextButton

        public string IsShowNextButtonPropertyName = "IsShowNextButton";
        private bool _isShowNextButton;
        /// <summary>
        /// 是否显示下一条按钮
        /// </summary>
        public bool IsShowNextButton
        {
            get { return _isShowNextButton; }
            set
            {
                if (_isShowNextButton == value) return;
                RaisePropertyChanging(IsShowNextButtonPropertyName);
                _isShowNextButton = value;
                RaisePropertyChanged(IsShowNextButtonPropertyName);
            }

        }

        #endregion

        #region CountDownTime

        public string CountDownTimePropertyName = "CountDownTime";
        private string _countDownTime;

        public string CountDownTime
        {
            get { return _countDownTime; }
            set
            {
                if (_countDownTime == value) return;
                RaisePropertyChanging(CountDownTimePropertyName);
                _countDownTime = value;
                RaisePropertyChanged(CountDownTimePropertyName);
            }
        }

        #endregion

        #region NoticeIndex

        public string NoticeIndexPropertyName = "NoticeIndex";
        private int _noticeIndex;

        public int NoticeIndex
        {
            get { return _noticeIndex; }
            set
            {
                if (_noticeIndex == value) return;
                RaisePropertyChanging(NoticeIndexPropertyName);
                _noticeIndex = value;
                RaisePropertyChanged(NoticeIndexPropertyName);
            }
        }

        #endregion

        #region BulletinFlowDocument

        private const string BulletinFlowDocumentPropertyName = "BulletinFlowDocument";
        private FlowDocument _bulletinFlowDocument;

        public FlowDocument BulletinFlowDocument
        {
            get { return _bulletinFlowDocument; }
            set
            {
                if (_bulletinFlowDocument == value) return;
                RaisePropertyChanging(BulletinFlowDocumentPropertyName);
                _bulletinFlowDocument = value;
                RaisePropertyChanged(BulletinFlowDocumentPropertyName);
            }
        }

        #endregion

        #region IsShowAttachMent

        private const string IsShowAttachMentPropertyName = "IsShowAttachMent";
        private bool _isShowAttachMent;

        public bool IsShowAttachMent
        {
            get { return _isShowAttachMent; }
            set
            {
                if (_isShowAttachMent == value) return;
                RaisePropertyChanging(IsShowAttachMentPropertyName);
                _isShowAttachMent = value;
                RaisePropertyChanged(IsShowAttachMentPropertyName);
            }
        }

        #endregion

        #region Title

        private const string TitlePropertyName = "Title";
        private string _title;

        public string Title
        {
            get { return _title; }
            set
            {
                if (_title == value) return;
                RaisePropertyChanging(TitlePropertyName);
                _title = value;
                RaisePropertyChanged(TitlePropertyName);
            }
        }

        #endregion

        #region CreateTime

        private const string CreateTimePropertyName = "CreateTime";
        private DateTime? _createTime;

        public DateTime? CreateTime
        {
            get { return _createTime; }
            set
            {
                if (_createTime == value) return;
                RaisePropertyChanging(CreateTimePropertyName);
                _createTime = value;
                RaisePropertyChanged(CreateTimePropertyName);
            }
        }

        #endregion

        #region CreateName

        private const string CreateNamePropertyName = "CreateName";
        private string _createName;

        public string CreateName
        {
            get { return _createName; }
            set
            {
                if (_createName == value) return;
                RaisePropertyChanging(CreateNamePropertyName);
                _createName = value;
                RaisePropertyChanged(CreateNamePropertyName);
            }
        }

        #endregion

        #region NoticeShowTypeText

        private const string NoticeShowTypeTextPropertyName = "NoticeShowTypeText";
        private string _noticeShowTypeText;

        public string NoticeShowTypeText
        {
            get { return _noticeShowTypeText; }
            set
            {
                if (_noticeShowTypeText == value) return;
                RaisePropertyChanging(NoticeShowTypeTextPropertyName);
                _noticeShowTypeText = value;
                RaisePropertyChanged(NoticeShowTypeTextPropertyName);
            }
        }

        #endregion

        #region NoticeType

        private const string NoticeTypePropertyName = "NoticeType";
        private string _noticeType;

        public string NoticeType
        {
            get { return _noticeType; }
            set
            {
                if (_noticeType == value) return;
                RaisePropertyChanging(NoticeTypePropertyName);
                _noticeType = value;
                RaisePropertyChanged(NoticeTypePropertyName);
            }
        }

        #endregion

        #region EffectiveStartTime

        private const string EffectiveStartTimePropertyName = "EffectiveStartTime";
        private DateTime? _effectiveStartTime;

        public DateTime? EffectiveStartTime
        {
            get { return _effectiveStartTime; }
            set
            {
                if (_effectiveStartTime == value) return;
                RaisePropertyChanging(EffectiveStartTimePropertyName);
                _effectiveStartTime = value;
                RaisePropertyChanged(EffectiveStartTimePropertyName);
            }
        }

        #endregion

        #region EffectiveEndTime

        private const string EffectiveEndTimePropertyName = "EffectiveEndTime";
        private DateTime? _effectiveEndTime;

        public DateTime? EffectiveEndTime
        {
            get { return _effectiveEndTime; }
            set
            {
                if (_effectiveEndTime == value) return;
                RaisePropertyChanging(EffectiveEndTimePropertyName);
                _effectiveEndTime = value;
                RaisePropertyChanged(EffectiveEndTimePropertyName);
            }
        }

        #endregion

        #region NoticeAttachmentDtos

        private const string NoticeAttachmentDtosPropertyName = "NoticeAttachmentDtos";
        private List<NoticeAttachmentDto> _noticeAttachmentDtos;

        public List<NoticeAttachmentDto> NoticeAttachmentDtos
        {
            get { return _noticeAttachmentDtos; }
            set
            {
                if (_noticeAttachmentDtos == value) return;
                RaisePropertyChanging(NoticeAttachmentDtosPropertyName);
                _noticeAttachmentDtos = value;
                RaisePropertyChanged(NoticeAttachmentDtosPropertyName);
            }
        }


        #endregion

        #endregion


        #region 公开命令

        #region OpenLink

        private RelayCommand<Hyperlink> _openLink;

        public RelayCommand<Hyperlink> OpenLink
        {
            get
            {
                return _openLink ?? (new RelayCommand<Hyperlink>(ExecuteOpenLink, CanExecuteOpenLink));
            }
        }

        private void ExecuteOpenLink(Hyperlink hyperLink)
        {
            if (hyperLink == null || hyperLink.NavigateUri == null)
            {
                return;
            }
            var url = hyperLink.NavigateUri.AbsoluteUri;
            UIManager.OpenDefaultBrower(url);
        }

        private bool CanExecuteOpenLink(Hyperlink hyperLink)
        {
            return true;
        }
        #endregion

        #region DownloadCommand

        private RelayCommand<string> _downloadCommand;

        public RelayCommand<string> DownloadCommand
        {
            get
            {
                return _downloadCommand ?? (new RelayCommand<string>(ExecuteDownloadCommand, CanExecuteDownloadCommand));
            }
        }

        private void ExecuteDownloadCommand(string url)
        {
            UIManager.OpenDefaultBrower(url);
        }

        private bool CanExecuteDownloadCommand(string url)
        {
            return url != null;
        }
        #endregion


        #region NextShowNoticeCommand

        private RelayCommand<int> _nextShowNoticeCommand;

        public RelayCommand<int> NextShowNoticeCommand
        {
            get
            {
                return _nextShowNoticeCommand ?? (new RelayCommand<int>(ExecuteNextShowNoticeCommand, CanExecuteNextShowNoticeCommand));
            }
        }

        private void ExecuteNextShowNoticeCommand(int index)
        {
            index++;
            if (index >= _ids.Length)
            {
                UIManager.ShowMessage("已经是最后一条了");
                return;
            }
            NoticeIndex = index;
            GetNoticeInfo(_ids[index]);

        }

        private bool CanExecuteNextShowNoticeCommand(int index)
        {
            if (index < 0)
            {
                return false;
            }
            return index <= _ids.Length - 1;
        }
        #endregion


        #region PreShowNoticeCommand

        private RelayCommand<int> _preShowNoticeCommand;

        public RelayCommand<int> PreShowNoticeCommand
        {
            get
            {
                return _nextShowNoticeCommand ?? (new RelayCommand<int>(ExecutePreShowNoticeCommand, CanExecutePreShowNoticeCommand));
            }
        }

        private void ExecutePreShowNoticeCommand(int index)
        {
            index--;
            if (index < 0)
            {
                UIManager.ShowMessage("已经是第一条了");
                return;
            }
            NoticeIndex = index;
            GetNoticeInfo(_ids[index]);

        }

        private bool CanExecutePreShowNoticeCommand(int index)
        {

            if (index < 0)
            {
                return false;
            }
            return index <= _ids.Length - 1;
        }
        #endregion
        #endregion


    }
}
