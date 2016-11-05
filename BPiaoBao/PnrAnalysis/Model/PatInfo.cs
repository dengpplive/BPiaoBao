using System;
using System.Collections.Generic;
using System.Text;

namespace PnrAnalysis
{
    /// <summary>
    /// PAT实体信息
    /// </summary>
    /// 
    [Serializable]
    public class PatInfo
    {
        private string _SerialNum = string.Empty;
        /// <summary>
        /// 序号
        /// </summary>
        public string SerialNum
        {
            get { return _SerialNum; }
            set { _SerialNum = value; }
        }

        private string _SeatGroup = string.Empty;
        /// <summary>
        /// PAT舱位部分字符串 如 Y E+V YCH A/TNA13A044  WAP15315+WAP15315  YCH+YCH  PPRHAP02   Y/CA2T132254 E/CA4T132118 等等
        /// </summary>
        public string SeatGroup
        {
            get { return _SeatGroup; }
            set { _SeatGroup = value; }
        }

        private string _Fare = string.Empty;
        /// <summary>
        /// 舱位价
        /// </summary>
        public string Fare
        {
            get { return _Fare; }
            set { _Fare = value; }
        }
        private string _TAX = "0";
        /// <summary>
        /// 基建
        /// </summary>
        public string TAX
        {
            get { return _TAX; }
            set { _TAX = value; }
        }
        private string _RQFare = "0";
        /// <summary>
        /// 燃油
        /// </summary>
        public string RQFare
        {
            get { return _RQFare; }
            set { _RQFare = value; }
        }
        private string _SFC = string.Empty;

        /// <summary>
        /// SFC
        /// </summary>
        public string SFC
        {
            get { return _SFC; }
            set { _SFC = value; }
        }

        private string _SFN = string.Empty;
        /// <summary>
        /// SFN
        /// </summary>
        public string SFN
        {
            get { return _SFN; }
            set { _SFN = value; }
        }

        private string _SFP = string.Empty;
        /// <summary>
        /// SFP
        /// </summary>
        public string SFP
        {
            get { return _SFP; }
            set { _SFP = value; }
        }

        private string _Price = "0";

        /// <summary>
        /// 价格： 舱位价+基建+燃油
        /// </summary>
        public string Price
        {
            get { return _Price; }
            set
            {
                _Price = value;
            }
        }
        private string _PriceType = "1";
        /// <summary>
        /// 价格类型 1成人 2儿童 3婴儿
        /// </summary>
        public string PriceType
        {
            get { return _PriceType; }
            set { _PriceType = value; }
        }
        /// <summary>
        /// 字符串显示
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            StringBuilder sbPat = new StringBuilder();
            if (this.PriceType == "1")
            {
                sbPat.Append(">PAT:A  \r");
            }
            else if (this.PriceType == "2")
            {
                sbPat.Append(">PAT:A*CH  \r");
            }
            else if (this.PriceType == "3")
            {
                sbPat.Append(">PAT:A*IN  \r");
            }
            sbPat.AppendFormat("{0} {1} FARE:CNY{2} TAX:CNY{3} YQ:CNY{4}  TOTAL:{5} \r",
                this.SerialNum.PadLeft(2, '0'),
                this.SeatGroup,
                this.Fare.Contains(".") ? this.Fare : this.Fare + ".00",
                this.TAX.Contains(".") ? this.TAX : this.TAX + ".00",
                this.RQFare.Contains(".") ? this.RQFare : this.RQFare + ".00",
                this.Price.Contains(".") ? this.Price : this.Price + ".00"
                );
            sbPat.Append(string.IsNullOrEmpty(this.SFC) ? "SFC:01" : this.SFC);
            return sbPat.ToString();
        }
        /// <summary>
        /// 替换价格 返回替换后的字符串
        /// </summary>
        /// <param name="strFare"></param>
        /// <param name="strTAX"></param>
        /// <param name="strRQFare"></param>
        /// <param name="strPrice"></param>
        /// <returns></returns>
        public string ReplacePriceToString(string strTAX, string strRQFare)
        {
            decimal m_Fare = 0m;
            decimal m_TAX = 0m;
            decimal m_RQFare = 0m;
            decimal m_Price = 0m;
            this.TAX = strTAX;
            this.RQFare = strRQFare;
            decimal.TryParse(this.Fare, out m_Fare);
            decimal.TryParse(this.TAX, out  m_TAX);
            decimal.TryParse(this.RQFare, out m_RQFare);
            m_Price = m_Fare + m_TAX + m_RQFare;
            this.Price = m_Price.ToString();

            StringBuilder sbPat = new StringBuilder();
            sbPat.AppendFormat("{0} {1} FARE:CNY{2} TAX:CNY{3} YQ:CNY{4}  TOTAL:{5} \r",
               this.SerialNum.PadLeft(2, '0'),
               this.SeatGroup,
               this.Fare.Contains(".") ? this.Fare : this.Fare + ".00",
               this.TAX.Contains(".") ? this.TAX : this.TAX + ".00",
               this.RQFare.Contains(".") ? this.RQFare : this.RQFare + ".00",
               this.Price.Contains(".") ? this.Price : this.Price + ".00"
               );
            sbPat.Append(string.IsNullOrEmpty(this.SFC) ? "SFC:01" : this.SFC);
            return sbPat.ToString();
        }


        private bool _IsOverMaxPrice = false;

        /// <summary>
        /// PAT总价格是否超出设置最大价格上限
        /// </summary>
        public bool IsOverMaxPrice
        {
            get
            {
                return _IsOverMaxPrice;
            }
            set
            {
                _IsOverMaxPrice = value;
            }
        }
    }
}
