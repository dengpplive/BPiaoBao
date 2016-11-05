using BPiaoBao.AppServices.Contracts.SystemSetting;
using BPiaoBao.AppServices.DataContracts.SystemSetting;
using BPiaoBao.Client.UIExt;
using BPiaoBao.Common.Enums;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Threading;
using JoveZhao.Framework;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BPiaoBao.Client.SystemSetting.ViewModel
{
    public class OperatorViewModel : BaseVM
    {
        //private bool isWait;
        //public bool IsWait
        //{
        //    get { return isWait; }
        //    set
        //    {
        //        if (isWait != value)
        //        {
        //            isWait = value;
        //            RaisePropertyChanged("IsWait");
        //        }
        //    }
        //}
        private bool _showHidden;

        public bool ShowHidden
        {
            get { return _showHidden; }
            set
            {
                if (_showHidden == value) return;
                _showHidden = value;
                RaisePropertyChanged("ShowHidden");
            }
        }

        public string SearchRealName { get; set; }
        public string SearchAccount { get; set; }
        public ObservableCollection<OperatorDto> OpList { get; private set; }
        public ObservableCollection<KeyValuePair<EnumOperatorState?, String>> OperatorStateList { get; private set; }
        private EnumOperatorState? _selectedOperatorStatus;

        public EnumOperatorState? SelectedOperatorStatus
        {
            get { return _selectedOperatorStatus; }
            set
            {
                if (_selectedOperatorStatus == value) return;
                _selectedOperatorStatus = value;
                RaisePropertyChanged("SelectedOperatorStatus");
            }
        }

        public OperatorDto CurrentOperator { get; set; }
        public OperatorViewModel()
        {
            if (OperatorStateList == null)
            {
                OperatorStateList = new ObservableCollection<KeyValuePair<EnumOperatorState?, string>>
                {
                    new KeyValuePair<EnumOperatorState?, string>(null, "所有")
                };
            }

            EnumItemManager.GetItemList<EnumOperatorState>().ForEach(x =>
            {
                var kv = new KeyValuePair<EnumOperatorState?, string>(x.Key, x.Value);
                OperatorStateList.Add(kv);
            });
            if (OpList == null)
                OpList = new ObservableCollection<OperatorDto>();

            if (IsInDesignMode)
                return;

            Initialize();

        }

        /// <summary>
        /// 初始化数据
        /// </summary>
        public override void Initialize()
        {
            ShowHidden = string.Equals(UIExt.Communicate.LoginInfo.Account, "admin", StringComparison.CurrentCultureIgnoreCase);
            Query();
        }

        /// <summary>
        /// 查询命令
        /// </summary>
        public ICommand QueryCommand
        {
            get
            {
                return new RelayCommand(Query, () => !IsBusy);
            }
        }
        /// <summary>
        /// 删除命令
        /// </summary>
        public ICommand DeleteCommand
        {
            get
            {
                return new RelayCommand<OperatorDto>(param =>
                {
                    var flag = MessageBoxExt.Show("操作提示", "你确定要删除吗?", MessageImageType.Question, MessageBoxButtonType.OKCancel);
                    if (flag == null || !flag.Value)
                        return;
                    IsBusy = true;
                    Action action = () => CommunicateManager.Invoke<IBusinessmanService>(p =>
                    {
                        p.DeleteOperator(param.Account);

                        DispatcherHelper.UIDispatcher.BeginInvoke(new Action(() =>
                        {
                            if (!OpList.Contains(param)) return;
                            OpList.Remove(param);
                        }));

                    }, UIManager.ShowErr);

                    Task.Factory.StartNew(action).ContinueWith(task =>
                    {
                        DispatcherHelper.UIDispatcher.BeginInvoke(new Action(() =>
                        {
                            IsBusy = false;
                        }));
                    });
                }, model =>
                {
                    //不能操作当前用户
                    var can = !IsBusy && model != null && UIExt.Communicate.LoginInfo.Account != model.Account;
                    return can;
                });
            }
        }

        #region UpdateCommand

        private RelayCommand<OperatorDto> _updateCommand;

        /// <summary>
        /// Gets the UpdateCommand.
        /// </summary>
        public RelayCommand<OperatorDto> UpdateCommand
        {
            get
            {
                return _updateCommand ?? (_updateCommand = new RelayCommand<OperatorDto>(ExecuteUpdateCommand, CanExecuteUpdateCommand));
            }
        }

        private void ExecuteUpdateCommand(OperatorDto operatorDto)
        {
            LocalUIManager.ShowOperatorDialog(operatorDto, result =>
            {
                if (result == null || !result.Value) return;
                Refresh(operatorDto);
            });
        }

        private bool CanExecuteUpdateCommand(OperatorDto operatorDto)
        {
            //不能操作当前用户
            var can = operatorDto != null && !IsBusy;
            return can;
        }

        #endregion


        #region ResetPswCommand 重置

        private RelayCommand<OperatorDto> _resetPswCommand;

        /// <summary>
        /// 重置
        /// </summary>
        public RelayCommand<OperatorDto> ResetPswCommand
        {
            get
            {
                return _resetPswCommand ?? (_resetPswCommand = new RelayCommand<OperatorDto>(ExecuteResetPswCommand, CanExecuteResetPswCommand));
            }
        }

        private void ExecuteResetPswCommand(OperatorDto obj)
        {
            CommunicateManager.Invoke<IBusinessmanService>(p =>
            {
                p.ResetPassword(obj.Account);
                UIManager.ShowMessage("修改成功");
            }, UIManager.ShowErr);
        }

        private bool CanExecuteResetPswCommand(OperatorDto obj)
        {
            return true;
        }
        #endregion

        #region ActiveCommand

        private RelayCommand<OperatorDto> _activeCommand;

        /// <summary>
        /// 启用，禁用命令
        /// </summary>
        public RelayCommand<OperatorDto> ActiveCommand
        {
            get
            {
                return _activeCommand ?? (_activeCommand = new RelayCommand<OperatorDto>(ExecuteActiveCommand, CanExecuteActiveCommand));
            }
        }

        private void ExecuteActiveCommand(OperatorDto param)
        {
            IsBusy = true;
            Action action = () => CommunicateManager.Invoke<IBusinessmanService>(p =>
            {
                p.ModifyOperatorState(param.Account);

                Refresh(param);
            }, UIManager.ShowErr);

            Task.Factory.StartNew(action).ContinueWith(task =>
            {
                DispatcherHelper.UIDispatcher.Invoke(new Action(() =>
                {
                    IsBusy = false;
                    ActiveCommand.RaiseCanExecuteChanged();
                }));
            });
        }

        private bool CanExecuteActiveCommand(OperatorDto param)
        {
            //不能操作当前用户
            var can = !IsBusy && param != null && UIExt.Communicate.LoginInfo.Account != param.Account;
            return can;
        }

        #endregion

        /// <summary>
        /// 刷新一条数据
        /// </summary>
        private void Refresh(OperatorDto model)
        {
            var oldData = OpList.FirstOrDefault(m => m.Id == model.Id);
            if (oldData == null)
                return;
            CommunicateManager.Invoke<IBusinessmanService>(p =>
            {
                var temp = p.GetAllOperators(model.Realname, model.Account, null);
                //分开写方便调试
                if (temp == null || temp.Count <= 0) return;
                var index = OpList.IndexOf(oldData);
                if (index < 0)
                    return;

                var newData = temp.FirstOrDefault(m => m.Id == model.Id);
                if (newData == null)
                    return;

                Action setAction = () =>
                {
                    OpList[index] = newData;
                };

                DispatcherHelper.UIDispatcher.Invoke(setAction);
            }, UIManager.ShowErr);
        }
        /// <summary>
        /// 打开新窗体
        /// </summary>
        public ICommand AddOperatorCommand
        {
            get
            {
                return new RelayCommand(() => LocalUIManager.ShowOperatorDialog(null, result =>
                {
                    if (result == null || !result.Value) return;
                    Query();
                }));
            }
        }
        /// <summary>
        /// 查询
        /// </summary>
        private bool _result = true;
        private void Query()
        {
            if (!_result) return;
            _result = false;
            IsBusy = true;
            OpList.Clear();
            Action action = () => CommunicateManager.Invoke<IBusinessmanService>(p =>
            {
                var temp = p.GetAllOperators(SearchRealName, SearchAccount, SelectedOperatorStatus);
                //分开写方便调试
                if (temp != null)
                    temp.ForEach(x => DispatcherHelper.UIDispatcher.Invoke(new Action<OperatorDto>(OpList.Add), x));
            }, UIManager.ShowErr);
            Task.Factory.StartNew(action).ContinueWith(p =>
            {
                Action setIsWait = () =>
                {
                    IsBusy = false;
                    _result = true;
                };
                DispatcherHelper.UIDispatcher.Invoke(setIsWait);
            });
        }
    }
}
