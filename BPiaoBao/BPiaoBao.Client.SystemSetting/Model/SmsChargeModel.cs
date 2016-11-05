using BPiaoBao.AppServices.DataContracts.SystemSetting;
using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.Client.SystemSetting.Model
{
    public class SmsChargeModel : ObservableObject
    {
        #region IsChecked

        /// <summary>
        /// The <see cref="IsChecked" /> property's name.
        /// </summary>
        public const string IsCheckedPropertyName = "IsChecked";

        private bool isChecked = false;

        /// <summary>
        /// 是否选择
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


        public int ID { get; set; }
        /// <summary>
        /// 商户Code
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 单价
        /// </summary>
        public decimal Price { get; set; }
        /// <summary>
        /// 总价
        /// </summary>
        public decimal TotalPrice { get; set; }
        /// <summary>
        /// 条数
        /// </summary>
        public int Count { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public bool State { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; } 
    }
}
