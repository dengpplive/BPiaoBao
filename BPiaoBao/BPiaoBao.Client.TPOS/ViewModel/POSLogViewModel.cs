using BPiaoBao.AppServices.Contracts.TPos;
using BPiaoBao.AppServices.DataContracts.TPos;
using BPiaoBao.Client.UIExt;
using GalaSoft.MvvmLight.Threading;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPiaoBao.Client.TPOS.ViewModel
{
    /// <summary>
    /// Pos机日志视图模型
    /// </summary>
    public class POSLogViewModel : BaseVM
    {
        #region 构造函数
        public POSLogViewModel(string posNo)
        {
            this.InputPos = posNo;
        }
        /// <summary>
        /// 初始化数据
        /// </summary>
        public override void Initialize()
        {
            IsBusy = true;
            PosAssignLogs.Clear();

            Action action = () =>
            {
                CommunicateManager.Invoke<ITPosService>(service =>
                {
                    //获取Pos列表
                    var temp = service.GetPosAssignLogs(inputPos);
                    if (temp != null)
                        foreach (var item in temp)
                            DispatcherHelper.UIDispatcher.Invoke(new Action<PosAssignLogDataObject>(PosAssignLogs.Add), item);

                }, UIManager.ShowErr);
            };

            Task.Factory.StartNew(action).ContinueWith((task) =>
            {
                Action setAction = () => { IsBusy = false; };
                DispatcherHelper.UIDispatcher.Invoke(setAction);
            });
        }

        #endregion

        #region 公开属性

        #region InputPos

        /// <summary>
        /// The <see cref="InputPos" /> property's name.
        /// </summary>
        public const string InputPosPropertyName = "InputPos";

        private string inputPos = null;

        /// <summary>
        /// 输入Pos
        /// </summary>
        public string InputPos
        {
            get { return inputPos; }

            set
            {
                if (inputPos == value) return;

                RaisePropertyChanging(InputPosPropertyName);
                inputPos = value;
                RaisePropertyChanged(InputPosPropertyName);
            }
        }

        #endregion

        #region PosAssignLogs

        /// <summary>
        /// The <see cref="PosAssignLogs" /> property's name.
        /// </summary>
        public const string PosAssignLogsPropertyName = "PosAssignLogs";

        private ObservableCollection<PosAssignLogDataObject> posAssignLogs = new ObservableCollection<PosAssignLogDataObject>();

        /// <summary>
        /// 日志
        /// </summary>
        public ObservableCollection<PosAssignLogDataObject> PosAssignLogs
        {
            get { return posAssignLogs; }

            set
            {
                if (posAssignLogs == value) return;

                RaisePropertyChanging(PosAssignLogsPropertyName);
                posAssignLogs = value;
                RaisePropertyChanged(PosAssignLogsPropertyName);
            }
        }

        #endregion

        #endregion
    }
}
