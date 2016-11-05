using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.Client.UIExt
{   /// <summary>
    /// 带分页的基础视图模型
    /// </summary>
    public abstract class PageBaseViewModel : BaseVM
    {
        #region 构造函数

        /// <summary>
        /// Initializes a new instance of the <see cref="PageBaseViewModel"/> class.
        /// </summary>
        public PageBaseViewModel()
        {
            //EndTime = DateTime.Now;
            //StartTime = DateTime.Today.AddMonths(-1);
        }

        #endregion

        #region 公开属性

        #region StartTime

        /// <summary>
        /// The <see cref="StartTime" /> property's name.
        /// </summary>
        public const string StartTimePropertyName = "StartTime";

        private DateTime? startTime;

        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime? StartTime
        {
            get { return startTime; }

            set
            {
                if (startTime == value) return;

                RaisePropertyChanging(StartTimePropertyName);
                startTime = value;
                RaisePropertyChanged(StartTimePropertyName);
            }
        }

        #endregion

        #region EndTime

        /// <summary>
        /// The <see cref="EndTime" /> property's name.
        /// </summary>
        public const string EndTimePropertyName = "EndTime";

        private DateTime? endTime = null;

        /// <summary>
        /// 查询结束时间,该属性会自动修改到当天23：59：59
        /// </summary>
        public DateTime? EndTime
        {
            get { return endTime; }

            set
            {
                if (value != null)
                {
                    value = new DateTime(value.Value.Year, value.Value.Month, value.Value.Day);
                    value = value.Value.Add(TimeSpan.FromDays(1)).Add(TimeSpan.FromMilliseconds(-1));
                }

                if (endTime == value) return;

                RaisePropertyChanging(EndTimePropertyName);
                endTime = value;
                RaisePropertyChanged(EndTimePropertyName);
            }
        }

        #endregion

        #region CurrentPageIndex

        /// <summary>
        /// The <see cref="CurrentPageIndex" /> property's name.
        /// </summary>
        public const string CurrentPageIndexPropertyName = "CurrentPageIndex";

        private int currentPageIndex = 1;

        /// <summary>
        /// 当前页
        /// </summary>
        public int CurrentPageIndex
        {
            get { return currentPageIndex; }

            set
            {
                if (currentPageIndex == value) return;

                RaisePropertyChanging(CurrentPageIndexPropertyName);
                currentPageIndex = value;
                RaisePropertyChanged(CurrentPageIndexPropertyName);
            }
        }

        #endregion

        #region TotalCount

        /// <summary>
        /// The <see cref="TotalCount" /> property's name.
        /// </summary>
        public const string TotalCountPropertyName = "TotalCount";

        private int totalCount = 0;

        /// <summary>
        /// 总页数
        /// </summary>
        public int TotalCount
        {
            get { return totalCount; }

            set
            {
                if (totalCount == value) return;

                RaisePropertyChanging(TotalCountPropertyName);
                totalCount = value;
                RaisePropertyChanged(TotalCountPropertyName);
            }
        }

        #endregion

        #region PageSize

        /// <summary>
        /// The <see cref="PageSize" /> property's name.
        /// </summary>
        public const string PageSizePropertyName = "PageSize";

        private int pageSize = 20;

        /// <summary>
        /// 页面大小
        /// </summary>
        public int PageSize
        {
            get { return pageSize; }

            set
            {
                if (pageSize == value) return;

                RaisePropertyChanging(PageSizePropertyName);
                pageSize = value;
                RaisePropertyChanged(PageSizePropertyName);
            }
        }

        #endregion

        #region IsBusy

        ///// <summary>
        ///// The <see cref="IsBusy" /> property's name.
        ///// </summary>
        //public const string IsBusyPropertyName = "IsBusy";

        //private bool isBusy = false;

        /// <summary>
        /// 是否在繁忙
        /// </summary>
        public new bool IsBusy
        {
            get { return isBusy; }

            set
            {
                if (isBusy == value) return;

                RaisePropertyChanging(IsBusyPropertyName);
                isBusy = value;
                RaisePropertyChanged(IsBusyPropertyName);

                if (queryCommand != null)
                    queryCommand.RaiseCanExecuteChanged();
            }
        }

        #endregion

        #endregion

        #region 公开命令

        #region QueryCommand

        private RelayCommand queryCommand;

        /// <summary>
        /// 查询命令
        /// </summary>
        public RelayCommand QueryCommand
        {
            get
            {
                return queryCommand ?? (queryCommand = new RelayCommand(ExecuteQueryCommand, CanExecuteQueryCommand));
            }
        }

        /// <summary>
        /// 执行查询命令
        /// </summary>
        protected virtual void ExecuteQueryCommand()
        {

        }

        /// <summary>
        /// 检查是否可以执行命令
        /// </summary>
        /// <returns></returns>
        protected virtual bool CanExecuteQueryCommand()
        {
            return !isBusy;
        }

        #endregion

        #endregion
    }
}
