using System;
using System.Threading.Tasks;
using System.Windows.Documents;
using BPiaoBao.AppServices.Contracts.SystemSetting;
using BPiaoBao.AppServices.DataContracts.SystemSetting;
using BPiaoBao.Client.UIExt;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Threading;

namespace BPiaoBao.Client.Message.ViewModel
{
    public class MessageDetailsViewModel : BaseVM
    {
        public MessageDetailsViewModel(MyMessageDto model = null)
        {
            if (model != null) InitData(model.ID);
        }

        #region 初始数据
        internal void InitData(int id)
        {
            GetNoticeInfo(id);
        }


        internal void InitData(MyMessageDto model)
        {
            if (model == null) return;
            Title = model.Title;
            CreateTime = model.CreateTime;
            var titleHtml = "<div style='text-align:center;font-size:20px;font-weight:bold;margin:10px'>" + Title +
                            "</div>";
            HtmlString =
                string.Format(
                    "<html><head><meta http-equiv=\"X-UA-Compatible\" content=\"IE=9\" />" +
                    "<meta http-equiv=\"content-type\" content=\"text/html;charset=utf-8\" /></head>" +
                    "<body><div>{0}{1}</div><div style='width: 100%; height: 300px; resize: none; background-color: #646464; color: #CDCB67; overflow: auto; font-size: 14px;'>{2}</div>" +
                    "<div style='text-align:right;padding:20px; margin-top:50px;font-size:12px'>" +
                    "{3}</div></body></html>", titleHtml, model.Content, model.QnContent, model.CreateTime.ToString("yyyy-MM-dd HH:mm:ss"));
        }

        #endregion

        #region 私有方法

        /// <summary>
        /// 获取单条通知信息
        /// </summary>
        /// <param name="id"></param>
        private void GetNoticeInfo(int id)
        {
            IsBusy = true;
            DispatcherHelper.UIDispatcher.Invoke(new Action(() =>
            {
                Action action = () => CommunicateManager.Invoke<IMyMessageService>(service =>
                {
                    var model = service.FindMyMsgById(id);
                    if (model == null) return;
                    InitData(model);
                    if (!model.State) service.ModifyMsgState(model.ID);
                }, UIManager.ShowErr);

                Task.Factory.StartNew(action).ContinueWith(task =>
                {
                    Action setAction = () => { IsBusy = false; };
                    DispatcherHelper.UIDispatcher.Invoke(setAction);
                });
            }));
        }
        #endregion


        #region 公开属性

        #region HtmlString

        private const string HtmlStringPropertyName = "HtmlString";
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

        #endregion

    }
}
