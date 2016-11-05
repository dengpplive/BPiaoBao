using System;
using BPiaoBao.Client.UIExt;
using BPiaoBao.Common.Enums;
using GalaSoft.MvvmLight;

namespace BPiaoBao.Client.DomesticTicket.Model
{
    /// <summary>
    /// 线下婴儿实体
    /// </summary>
    public class BabyModel : ObservableObject
    {
        #region 线下婴儿姓名

        public const string BabyNamePropertyName = "BabyName";
        private string _babyName;

        /// <summary>
        /// 线下婴儿姓名
        /// </summary>
        public string BabyName
        {
            get { return _babyName; }
            set
            {
                if (_babyName == value) return;
                RaisePropertyChanging(BabyNamePropertyName);
                _babyName = value;
                RaisePropertyChanged(BabyNamePropertyName);
            }
        }

        #endregion

        #region  线下婴儿性别

        public const string SexTypeProprtyName = "SexType";
        private EnumSexType _sexType;
        /// <summary>
        /// 被投保人性别
        /// </summary>
        public EnumSexType SexType
        {
            get { return _sexType; }
            set
            {
                if (_sexType == value) return;
                RaisePropertyChanging(SexTypeProprtyName);
                _sexType = value;
                RaisePropertyChanged(SexTypeProprtyName);
            }
        }

        #endregion

        #region  被投保人出生日期

        public const string BornDatePropertyName = "BornDate";
        private DateTime? _bornDate;
        /// <summary>
        /// 被投保人出生日期
        /// </summary>
        public DateTime? BornDate
        {
            get { return _bornDate; }
            set
            {
                if (_bornDate == value) return;
                RaisePropertyChanging(BornDatePropertyName);
                _bornDate = value;
                RaisePropertyChanged(BornDatePropertyName);
            }
        }
        #endregion

        #region DisplayDateEnd
        protected const string CDisplayDateEndPropertyName = "DisplayDateEnd";

        private DateTime _displayDateEnd;

        /// <summary>
        /// DisplayDateEnd
        /// </summary>        
        public DateTime DisplayDateEnd
        {
            get { return _displayDateEnd; }

            set
            {
                if (_displayDateEnd == value) return;
                RaisePropertyChanging(CDisplayDateEndPropertyName);
                _displayDateEnd = value;
                RaisePropertyChanged(CDisplayDateEndPropertyName);
            }
        }
        #endregion

        #region DisplayDateStart
        protected const string CDisplayDateStartPropertyName = "DisplayDateStart";

        private DateTime _displayDateStart;

        /// <summary>
        /// DisplayDateStart
        /// </summary>        
        public DateTime DisplayDateStart
        {
            get { return _displayDateStart; }

            set
            {
                if (_displayDateStart == value) return;
                RaisePropertyChanging(CDisplayDateStartPropertyName);
                _displayDateStart = value;
                RaisePropertyChanged(CDisplayDateStartPropertyName);
            }
        }
        #endregion

        /// <summary>
        /// 验证input
        /// </summary>
        public bool CheckInput()
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(BabyName))
                {
                    //1.全中文
                    //2.英文/英文
                    //3.中文英文
                    //4.全中文长度<15;英文<30;中文英文<30
                    if (SexType != EnumSexType.UnKnown)
                    {
                        if (BornDate != null && BornDate >= DisplayDateStart && BornDate <= DisplayDateEnd) return true;
                        UIManager.ShowMessage(string.Format("请核对出生日期范围{0:yyyy-MM-dd}",BornDate));
                        return false;
                    }
                    UIManager.ShowMessage(string.Format("请选择性别"));
                    return false;
                }
                UIManager.ShowMessage(string.Format("姓名不能为空"));
                return false;
            }
            catch (Exception ex)
            {
                UIManager.ShowErr(ex);
                return false;
            }
        }
    }
}
