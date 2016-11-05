using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BPiaoBao.AppServices.Contracts.SystemSetting;
using BPiaoBao.AppServices.DataContracts.SystemSetting;
using BPiaoBao.Client.UIExt;
using BPiaoBao.Client.UIExt.Utils;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Threading;

namespace BPiaoBao.Client.Message.ViewModel
{
    public class MessageViewModel : PageBaseViewModel
    {
        #region 构造函数
        public MessageViewModel()
        {
            Initialize();
            Messenger.Default.Unregister<bool>(this, "mymessage");
            Messenger.Default.Register<bool>(this, "mymessage", p =>
             {
                 if (p)
                 {
                     Search();
                 }
             });
        }

        public override void Initialize()
        {
            if (IsInDesignMode) return;
            Search();
        }

        #endregion

        #region 公开属性

        #region MyMessageModels

        private const string MyMessageModelsPropertyName = "MyMessageModels";
        private List<MyMessageDto> _myMessageModels;

        public List<MyMessageDto> MyMessageModels
        {
            get { return _myMessageModels; }
            set
            {

                RaisePropertyChanging(MyMessageModelsPropertyName);
                _myMessageModels = value;
                RaisePropertyChanged(MyMessageModelsPropertyName);
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

        #endregion

        #region 公开命令

        #region QueryCommand
        protected override void ExecuteQueryCommand()
        {
            Search();
        }
        #endregion

        #region ShowBulletinDetailsCommand

        private RelayCommand<MyMessageDto> _showBulletinDetailsCommand;

        public RelayCommand<MyMessageDto> ShowBulletinDetailsCommand
        {
            get
            {
                return _showBulletinDetailsCommand ??
                       (new RelayCommand<MyMessageDto>(ExecuteShowBulletinDetailsCommand, CanExecuteShowBulletinDetailsCommand));
            }
        }

        private void ExecuteShowBulletinDetailsCommand(MyMessageDto model)
        {
            LocalUIManager.ShowMessageDetailsDialog(model, Search);
        }

        private bool CanExecuteShowBulletinDetailsCommand(MyMessageDto model)
        {
            return model != null;
        }

        #endregion

        #endregion

        #region 私有方法
        #region Search
        /// <summary>
        /// 查询方法
        /// </summary>
        private void Search()
        {
            if (StartTime != null & EndTime != null)
            {
                if (DateTime.Compare(StartTime.Value, EndTime.Value) > 0)
                {
                    UIManager.ShowMessage("结束时间大于开始时间");
                    return;
                }
            }
            IsBusy = true;
            DispatcherHelper.UIDispatcher.Invoke(new Action(() =>
            {
                Action action = () => CommunicateManager.Invoke<IMyMessageService>(service =>
                {
                    GlobalData.ClientMainVm.TipCount = service.GetUnReadMsgCount();
                    var data = service.FindMyMsgList(CurrentPageIndex, PageSize);
                    TotalCount = data.TotalCount;
                    if (data.TotalCount <= 0) return;
                    MyMessageModels = data.List;
                }, UIManager.ShowErr);

                Task.Factory.StartNew(action).ContinueWith(task =>
                {
                    Action setAction = () =>{ IsBusy = false; };
                    DispatcherHelper.UIDispatcher.Invoke(setAction);
                });
            }));
        }
        #endregion

        #endregion
    }
}
