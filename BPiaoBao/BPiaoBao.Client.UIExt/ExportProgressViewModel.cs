using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.Client.UIExt
{
    /// <summary>
    /// 导出进度视图模型
    /// </summary>
    public class ExportProgressViewModel : BaseVM
    {
        Action abortAction;
        public ExportProgressViewModel(Action action)
        {
            abortAction = action;
        }

        #region Maximum

        /// <summary>
        /// The <see cref="Maximum" /> property's name.
        /// </summary>
        public const string MaximumPropertyName = "Maximum";

        private decimal maximum = 100;

        /// <summary>
        /// 最大数量
        /// </summary>
        public decimal Maximum
        {
            get { return maximum; }

            set
            {
                if (maximum == value) return;

                RaisePropertyChanging(MaximumPropertyName);
                maximum = value;
                RaisePropertyChanged(MaximumPropertyName);
            }
        }

        #endregion

        #region Value

        /// <summary>
        /// The <see cref="Value" /> property's name.
        /// </summary>
        public const string ValuePropertyName = "Value";

        private decimal val = 0;

        /// <summary>
        /// 当前进度
        /// </summary>
        public decimal Value
        {
            get { return val; }

            set
            {
                if (val == value) return;

                RaisePropertyChanging(ValuePropertyName);
                val = value;
                RaisePropertyChanged(ValuePropertyName);
            }
        }

        #endregion

        #region Message

        /// <summary>
        /// The <see cref="Message" /> property's name.
        /// </summary>
        public const string MessagePropertyName = "Message";

        private string message = null;

        /// <summary>
        /// 显示消息
        /// </summary>
        public string Message
        {
            get { return message; }

            set
            {
                if (message == value) return;

                RaisePropertyChanging(MessagePropertyName);
                message = value;
                RaisePropertyChanged(MessagePropertyName);
            }
        }

        #endregion

        #region IsClose

        /// <summary>
        /// The <see cref="IsClose" /> property's name.
        /// </summary>
        public const string IsClosePropertyName = "IsClose";

        private bool isClose = false;

        /// <summary>
        /// 是否关闭
        /// </summary>
        public bool IsClose
        {
            get { return isClose; }

            set
            {
                if (isClose == value) return;

                RaisePropertyChanging(IsClosePropertyName);
                isClose = value;
                RaisePropertyChanged(IsClosePropertyName);
            }
        }

        #endregion

        #region AbortText

        /// <summary>
        /// The <see cref="AbortText" /> property's name.
        /// </summary>
        public const string AbortTextPropertyName = "AbortText";

        private string abortText = "停止";

        /// <summary>
        /// desc
        /// </summary>
        public string AbortText
        {
            get { return abortText; }

            set
            {
                if (abortText == value) return;

                RaisePropertyChanging(AbortTextPropertyName);
                abortText = value;
                RaisePropertyChanged(AbortTextPropertyName);
            }
        }

        #endregion

        #region AbortCommand

        private RelayCommand abortCommand;

        /// <summary>
        /// 终止命令
        /// </summary>
        public RelayCommand AbortCommand
        {
            get
            {
                return abortCommand ?? (abortCommand = new RelayCommand(ExecuteAbortCommand, CanExecuteAbortCommand));
            }
        }

        private void ExecuteAbortCommand()
        {
            var result = UIManager.ShowMessageDialog("确认终止导出？");
            if (result != null && result.Value)
            {
                AbortText = "停止中...";
                abortAction();
            }
        }

        private bool CanExecuteAbortCommand()
        {
            return abortText == "停止";
        }

        #endregion

    }
}
