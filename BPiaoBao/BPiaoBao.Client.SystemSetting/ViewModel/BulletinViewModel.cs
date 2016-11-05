using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BPiaoBao.AppServices.Contracts.SystemSetting;
using BPiaoBao.AppServices.DataContracts.SystemSetting;
using BPiaoBao.Client.UIExt;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Threading;

namespace BPiaoBao.Client.SystemSetting.ViewModel
{
    public class BulletinViewModel : PageBaseViewModel
    {
        #region 构造函数
        public BulletinViewModel()
        {
            if (IsInDesignMode)
                return;
            Search();
            
        }

        public override void Initialize()
        {
            if (IsInDesignMode) return;
            Search();
        }

        #endregion

        #region 公开属性

        #region BulletinModels

        private const string BulletinModelsPropertyName = "BulletinModels";
        private List<NoticeDto> _bulletinModels;

        public List<NoticeDto> BulletinModels
        {
            get { return _bulletinModels; }
            set
            {
               
                RaisePropertyChanging(BulletinModelsPropertyName);
                _bulletinModels = value;
                RaisePropertyChanged(BulletinModelsPropertyName);
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

        #region SelectedBulletinType

        private const string SelectedBulletinTypePropertyName = "SelectedBulletinType";
        private string _selectedBulletinType;
        public string SelectedBulletinType
        {
            get { return _selectedBulletinType; }
            set
            {
                if (_selectedBulletinType == value) return;
                RaisePropertyChanging(SelectedBulletinTypePropertyName);
                _selectedBulletinType = value;
                RaisePropertyChanged(SelectedBulletinTypePropertyName);
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

        private RelayCommand<NoticeDto> _showBulletinDetailsCommand;

        public RelayCommand<NoticeDto> ShowBulletinDetailsCommand
        {
            get
            {
                return _showBulletinDetailsCommand ??
                       (new RelayCommand<NoticeDto>(ExecuteShowBulletinDetailsCommand, CanExecuteShowBulletinDetailsCommand));
            }
        }

        private void ExecuteShowBulletinDetailsCommand(NoticeDto model)
        {
            LocalUIManager.ShowBulletionDetailsDialog(model);
        }

        private bool CanExecuteShowBulletinDetailsCommand(NoticeDto model)
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
            Action action = () =>
            {
                IsBusy = true;
                CommunicateManager.Invoke<INoticeService>(service =>
                {
                    var data = service.FindNoticeList(null, Title, StartTime, EndTime, CurrentPageIndex, PageSize);
                    TotalCount = data.TotalCount;
                    if (data.TotalCount <= 0) return;
                    BulletinModels = data.List;
                }, UIManager.ShowErr);
            };

            Task.Factory.StartNew(action).ContinueWith(task =>
            {
                Action setAction = () => { IsBusy = false; };
                DispatcherHelper.UIDispatcher.Invoke(setAction);
            });

        }
        #endregion
        #endregion
    }


}
