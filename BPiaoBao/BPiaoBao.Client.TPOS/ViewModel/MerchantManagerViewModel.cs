using BPiaoBao.AppServices.Contracts.TPos;
using BPiaoBao.AppServices.DataContracts.TPos;
using BPiaoBao.Client.UIExt;
using BPiaoBao.Client.UIExt.Communicate;
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
    /// <summary>
    /// 商户管理视图模型
    /// </summary>
    public class MerchantManagerViewModel : PageBaseViewModel
    {
        #region 构造函数

        public MerchantManagerViewModel()
        {
            if (IsInDesignMode)
                return;

            Initialize();
        }

        /// <summary>
        /// 页面呈现后触发
        /// </summary>
        protected override void ExecutePageLoadCommand()
        {
            Initialize();
        }

        /// <summary>
        /// 初始化数据
        /// </summary>
        public override void Initialize()
        {
            if (CanExecuteQueryCommand())
                ExecuteQueryCommand();
        }

        #endregion

        #region 公开属性

        #region InputName

        /// <summary>
        /// The <see cref="InputName" /> property's name.
        /// </summary>
        public const string InputNamePropertyName = "InputName";

        private string inputName = null;

        /// <summary>
        /// 输入的内容
        /// </summary>
        public string InputName
        {
            get { return inputName; }

            set
            {
                if (inputName == value) return;

                RaisePropertyChanging(InputNamePropertyName);
                inputName = value;
                RaisePropertyChanged(InputNamePropertyName);
            }
        }

        #endregion

        #region InputPosNo

        /// <summary>
        /// The <see cref="InputPosNo" /> property's name.
        /// </summary>
        public const string InputPosNoPropertyName = "InputPosNo";

        private string inputPosNo = null;

        /// <summary>
        /// 输入的pos编码
        /// </summary>
        public string InputPosNo
        {
            get { return inputPosNo; }

            set
            {
                if (inputPosNo == value) return;

                RaisePropertyChanging(InputPosNoPropertyName);
                inputPosNo = value;
                RaisePropertyChanged(InputPosNoPropertyName);
            }
        }

        #endregion

        #region Merchants

        /// <summary>
        /// The <see cref="Merchants" /> property's name.
        /// </summary>
        public const string MerchantsPropertyName = "Merchants";

        private ObservableCollection<ResponseBusinessmanInfo> merchants = new ObservableCollection<ResponseBusinessmanInfo>();

        /// <summary>
        /// 商户列表
        /// </summary>
        public ObservableCollection<ResponseBusinessmanInfo> Merchants
        {
            get { return merchants; }

            set
            {
                if (merchants == value) return;

                RaisePropertyChanging(MerchantsPropertyName);
                merchants = value;
                RaisePropertyChanged(MerchantsPropertyName);
            }
        }

        #endregion

        #endregion

        #region 公开命令

        #region AddCommand

        private RelayCommand addCommand;

        /// <summary>
        /// 添加商户命令
        /// </summary>
        public RelayCommand AddCommand
        {
            get
            {
                return addCommand ?? (addCommand = new RelayCommand(ExecuteAddCommand, CanExecuteAddCommand));
            }
        }

        private void ExecuteAddCommand()
        {
            LocalUIManager.AddMerchant(new Action<bool?>((isOk) =>
            {
                if (isOk != null && isOk.Value)
                    if (CanExecuteAddCommand())
                        ExecuteQueryCommand();
            }));
        }

        private bool CanExecuteAddCommand()
        {
            return true;
        }

        #endregion

        #region DetailCommand

        private RelayCommand<ResponseBusinessmanInfo> detailCommand;

        /// <summary>
        /// Gets the DetailCommand.
        /// </summary>
        public RelayCommand<ResponseBusinessmanInfo> DetailCommand
        {
            get
            {
                return detailCommand ?? (detailCommand = new RelayCommand<ResponseBusinessmanInfo>(ExecuteDetailCommand, CanExecuteDetailCommand));
            }
        }

        private void ExecuteDetailCommand(ResponseBusinessmanInfo merchant)
        {
            LocalUIManager.ShowMerchantInfo(merchant);
        }

        private bool CanExecuteDetailCommand(ResponseBusinessmanInfo merchant)
        {
            return true;
        }

        #endregion

        #region EditCommand

        private RelayCommand<ResponseBusinessmanInfo> editCommand;

        /// <summary>
        /// 修改命令
        /// </summary>
        public RelayCommand<ResponseBusinessmanInfo> EditCommand
        {
            get
            {
                return editCommand ?? (editCommand = new RelayCommand<ResponseBusinessmanInfo>(ExecuteEditCommand, CanExecuteEditCommand));
            }
        }

        private void ExecuteEditCommand(ResponseBusinessmanInfo merchant)
        {
            LocalUIManager.EditMerchant(merchant, new Action<bool?>((isOk) =>
            {
                if (isOk != null && isOk.Value)
                {
                    Refresh(merchant);
                }
            }));
        }

        private void Refresh(ResponseBusinessmanInfo merchant)
        {
            IsBusy = true;
            Action action = () =>
            {
                CommunicateManager.Invoke<ITPosService>(service =>
                {
                    var result = service.GetBusinessmanInfo(merchant.Id);
                    var index = Merchants.IndexOf(merchant);
                    if (index >= 0)
                    {
                        DispatcherHelper.UIDispatcher.Invoke(new Action(() =>
                        {
                            Merchants[index] = result;
                        }));
                    }

                }, UIManager.ShowErr);
            };

            Task.Factory.StartNew(action).ContinueWith((task) =>
            {
                Action setAction = () => { IsBusy = false; };
                DispatcherHelper.UIDispatcher.Invoke(setAction);
            });
        }

        private bool CanExecuteEditCommand(ResponseBusinessmanInfo merchant)
        {
            return true;
        }

        #endregion

        #region AssignCommand

        private RelayCommand<ResponseBusinessmanInfo> assignCommand;

        /// <summary>
        /// 分配POS机命令
        /// </summary>
        public RelayCommand<ResponseBusinessmanInfo> AssignCommand
        {
            get
            {
                return assignCommand ?? (assignCommand = new RelayCommand<ResponseBusinessmanInfo>(ExecuteAssignCommand, CanExecuteAssignCommand));
            }
        }

        private void ExecuteAssignCommand(ResponseBusinessmanInfo merchant)
        {
            LocalUIManager.AssignPos(merchant, (result) =>
            {
                if (result != null && result.Value)
                    Refresh(merchant);
            });
        }

        private bool CanExecuteAssignCommand(ResponseBusinessmanInfo merchant)
        {
            return true;
        }

        #endregion

        #region DeleteCommand

        private RelayCommand<ResponseBusinessmanInfo> deleteCommand;

        /// <summary>
        /// 删除命令
        /// </summary>
        public RelayCommand<ResponseBusinessmanInfo> DeleteCommand
        {
            get
            {
                return deleteCommand ?? (deleteCommand = new RelayCommand<ResponseBusinessmanInfo>(ExecuteDeleteCommand, CanExecuteDeleteCommand));
            }
        }

        private void ExecuteDeleteCommand(ResponseBusinessmanInfo info)
        {
            var result = UIManager.ShowMessageDialog("确认删除商户？");
            if (result == null || result.Value == false)
                return;

            IsBusy = true;

            Action action = () =>
            {
                CommunicateManager.Invoke<ITPosService>(service =>
                {
                    service.DeleteBusinessman(info.Id);
                    UIManager.ShowMessage("删除成功");
                    if (Merchants.Contains(info))
                    {
                        DispatcherHelper.UIDispatcher.Invoke(new Action(() =>
                        {
                            Merchants.Remove(info);
                        }));
                    }
                }, UIManager.ShowErr);
            };

            Task.Factory.StartNew(action).ContinueWith((task) =>
            {
                Action setAction = () => { IsBusy = false; };
                DispatcherHelper.UIDispatcher.Invoke(setAction);
            });
        }

        private bool CanExecuteDeleteCommand(ResponseBusinessmanInfo info)
        {
            return true;
        }

        #endregion

        #region 查询

        protected override void ExecuteQueryCommand()
        {
            IsBusy = true;
            Merchants.Clear();

            Action action = () =>
            {
                CommunicateManager.Invoke<ITPosService>(service =>
                {
                    //获取商户列表
                    var temp = service.GetPosBusinessman(InputName, InputPosNo, (CurrentPageIndex - 1) * PageSize, PageSize);
                    TotalCount = temp.TotalCount;
                    if (temp.List != null)
                        foreach (var item in temp.List)
                            DispatcherHelper.UIDispatcher.Invoke(new Action<ResponseBusinessmanInfo>(Merchants.Add), item);

                }, UIManager.ShowErr);
            };

            Task.Factory.StartNew(action).ContinueWith((task) =>
            {
                Action setAction = () => { IsBusy = false; };
                DispatcherHelper.UIDispatcher.Invoke(setAction);
            });
        }

        protected override bool CanExecuteQueryCommand()
        {
            return !IsBusy;
        }

        #endregion

        #endregion
    }
}
