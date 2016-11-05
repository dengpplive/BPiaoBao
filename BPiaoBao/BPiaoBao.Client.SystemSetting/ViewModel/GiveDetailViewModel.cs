using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BPiaoBao.AppServices.Contracts.SystemSetting;
using BPiaoBao.AppServices.DataContracts.SystemSetting;
using BPiaoBao.Client.UIExt;
using GalaSoft.MvvmLight.Threading;

namespace BPiaoBao.Client.SystemSetting.ViewModel
{
    /// <summary>
    /// 赠送短信ViewModel
    /// </summary>
    public class GiveDetailViewModel : PageBaseViewModel
    {
        #region 实例对象
        private static GiveDetailViewModel _instance;


        public static GiveDetailViewModel CreateInstance()
        {
            if (_instance == null)
                _instance = new GiveDetailViewModel();
            _instance.Init();
            return _instance;
        }
        #endregion

        #region 公开属性
        #region GiveDetailDto

        private const string GiveDetailDtoPropertyName = "GiveDetailDtos";
        private List<GiveDetailDto> _giveDetailDtos;

        public List<GiveDetailDto> GiveDetailDtos
        {
            get { return _giveDetailDtos; }
            set
            {
                if (_giveDetailDtos == value) return;
                RaisePropertyChanging(GiveDetailDtoPropertyName);
                _giveDetailDtos = value;
                RaisePropertyChanged(GiveDetailDtoPropertyName);
            }
        }
        #endregion

        #endregion

        #region 公开命令
        #region QueryCommand
        protected override void ExecuteQueryCommand()
        {
            Init();
        }
        #endregion
        #endregion

        #region 公开方法

        public void Init()
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
            Action action = () => CommunicateManager.Invoke<IBusinessmanService>(service =>
            {
                var data = service.GetSmsGiveDetail(CurrentPageIndex, PageSize, StartTime, EndTime);
                if (data.TotalCount <= 0) return;
                TotalCount = data.TotalCount;
                GiveDetailDtos = data.List;
            }, UIManager.ShowErr);

            Task.Factory.StartNew(action).ContinueWith(task =>
            {
                Action setAction = () => { IsBusy = false; };
                DispatcherHelper.UIDispatcher.Invoke(setAction);
            });
        }
        #endregion
    }
}
