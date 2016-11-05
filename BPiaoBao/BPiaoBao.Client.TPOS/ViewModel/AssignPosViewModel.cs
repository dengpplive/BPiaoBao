using BPiaoBao.AppServices.Contracts.TPos;
using BPiaoBao.AppServices.DataContracts.TPos;
using BPiaoBao.Client.UIExt;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Threading;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPiaoBao.Client.TPOS.ViewModel
{
    public class AssignPosViewModel : PageBaseViewModel
    {
        private List<string> cache = new List<string>();

        #region 构造函数

        /// <summary>
        /// 初始化数据
        /// </summary>
        public override void Initialize()
        {
            if (CanExecuteQueryCommand())
                ExecuteQueryCommand();
        }

        protected override bool CanExecuteQueryCommand()
        {
            return !IsBusy;
        }

        protected override void ExecuteQueryCommand()
        {
            POSList.Clear();
            if (Merchant == null)
                return;

            IsBusy = true;

            Action action = () =>
            {
                CommunicateManager.Invoke<ITPosService>(service =>
                {
                    //获取Pos列表Merchant.BusinessmanName
                    var temp = service.GetPosList(null, string.Empty, false, (CurrentPageIndex - 1) * PageSize, PageSize);
                    TotalCount = temp.TotalCount;
                    if (temp.List != null)
                        foreach (var item in temp.List)
                            DispatcherHelper.UIDispatcher.Invoke(new Action<POSCheckModel>(POSList.Add), new POSCheckModel()
                            {
                                IsChecked = cache.Contains(item.PosNo),
                                //PosName=item
                                PosNo = item.PosNo
                            });

                    //todo 获取pos机数量

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

        #region POSList

        /// <summary>
        /// The <see cref="POSList" /> property's name.
        /// </summary>
        public const string POSListPropertyName = "POSList";

        private ObservableCollection<POSCheckModel> posList = new ObservableCollection<POSCheckModel>();

        /// <summary>
        /// pos机列表
        /// </summary>
        public ObservableCollection<POSCheckModel> POSList
        {
            get { return posList; }

            set
            {
                if (posList == value) return;

                RaisePropertyChanging(POSListPropertyName);
                posList = value;
                RaisePropertyChanged(POSListPropertyName);
            }
        }

        #endregion

        #region IsOk

        /// <summary>
        /// The <see cref="IsOk" /> property's name.
        /// </summary>
        public const string IsOkPropertyName = "IsOk";

        private bool isOk = false;

        /// <summary>
        /// desc
        /// </summary>
        public bool IsOk
        {
            get { return isOk; }

            set
            {
                if (isOk == value) return;

                RaisePropertyChanging(IsOkPropertyName);
                isOk = value;
                RaisePropertyChanged(IsOkPropertyName);
            }
        }

        #endregion

        /// <summary>
        /// 商户信息
        /// </summary>
        /// <value>
        /// The merchant.
        /// </value>
        public ResponseBusinessmanInfo Merchant { get; set; }

        #endregion

        #region 公开命令

        #region AssignCommand

        private RelayCommand assignCommand;

        /// <summary>
        /// 分配命令
        /// </summary>
        public RelayCommand AssignCommand
        {
            get
            {
                return assignCommand ?? (assignCommand = new RelayCommand(ExecuteAssignCommand, CanExecuteAssignCommand));
            }
        }

        private void ExecuteAssignCommand()
        {
            IsBusy = true;

            Action action = () =>
            {
                CommunicateManager.Invoke<ITPosService>(service =>
                {
                    //获取Pos列表
                    service.AssignPos(Merchant.Id, cache.ToArray());
                    UIManager.ShowMessage("分配成功");
                    IsOk = true;

                }, UIManager.ShowErr);
            };

            Task.Factory.StartNew(action).ContinueWith((task) =>
            {
                Action setAction = () => { IsBusy = false; };
                DispatcherHelper.UIDispatcher.Invoke(setAction);
            });
        }

        private bool CanExecuteAssignCommand()
        {
            return !IsBusy;
        }

        #endregion

        #region CheckCommand

        private RelayCommand<POSCheckModel> checkCommand;

        /// <summary>
        /// Gets the CheckCommand.
        /// </summary>
        public RelayCommand<POSCheckModel> CheckCommand
        {
            get
            {
                return checkCommand ?? (checkCommand = new RelayCommand<POSCheckModel>(ExecuteCheckCommand, CanExecuteCheckCommand));
            }
        }

        private void ExecuteCheckCommand(POSCheckModel pos)
        {
            if (pos.IsChecked)
                cache.Add(pos.PosNo);
            else if (cache.Contains(pos.PosNo))
                cache.Remove(pos.PosNo);
        }

        private bool CanExecuteCheckCommand(POSCheckModel pos)
        {
            return pos != null;
        }

        #endregion

        #endregion
    }

    public class POSCheckModel : ObservableObject
    {
        #region 公开属性

        #region IsChecked

        /// <summary>
        /// The <see cref="IsChecked" /> property's name.
        /// </summary>
        public const string IsCheckedPropertyName = "IsChecked";

        private bool isChecked = false;

        /// <summary>
        /// 是否被选中
        /// </summary>
        public bool IsChecked
        {
            get { return isChecked; }

            set
            {
                if (isChecked == value) return;

                RaisePropertyChanging(IsCheckedPropertyName);
                isChecked = value;
                RaisePropertyChanged(IsCheckedPropertyName);
            }
        }

        #endregion

        #region PosName

        /// <summary>
        /// The <see cref="PosName" /> property's name.
        /// </summary>
        public const string PosNamePropertyName = "PosName";

        private string posName = null;

        /// <summary>
        /// pos机名称
        /// </summary>
        public string PosName
        {
            get { return posName; }

            set
            {
                if (posName == value) return;

                RaisePropertyChanging(PosNamePropertyName);
                posName = value;
                RaisePropertyChanged(PosNamePropertyName);
            }
        }

        #endregion

        #region PosNo

        /// <summary>
        /// The <see cref="PosNo" /> property's name.
        /// </summary>
        public const string PosNoPropertyName = "PosNo";

        private string posNo = null;

        /// <summary>
        /// Pos编号
        /// </summary>
        public string PosNo
        {
            get { return posNo; }

            set
            {
                if (posNo == value) return;

                RaisePropertyChanging(PosNoPropertyName);
                posNo = value;
                RaisePropertyChanged(PosNoPropertyName);
            }
        }

        #endregion

        #endregion
    }
}
