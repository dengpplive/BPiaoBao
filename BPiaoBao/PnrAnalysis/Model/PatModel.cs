using System;
using System.Collections.Generic;
using System.Text;

namespace PnrAnalysis
{
    /// <summary>
    /// PAT价格实体集合
    /// </summary>
    [Serializable]
    public class PatModel
    {
        private string _PatType = "1";

        /// <summary>
        /// PAT价格类型 1成人 2儿童 3婴儿
        /// </summary>
        public string PatType
        {
            get { return _PatType; }
            set { _PatType = value; }
        }

        private int _PatPriceCount = 0;
        /// <summary>
        /// 有效价格数目 去掉重复的舱位价
        /// </summary>
        public int PatPriceCount
        {
            get
            {
                return UninuePatList.Count;
            }
        }

        private List<PatInfo> _UninuePatList = new List<PatInfo>();
        /// <summary>
        /// 去掉重复后的价格数据列表
        /// </summary>
        public List<PatInfo> UninuePatList
        {
            get
            {
                System.Collections.Hashtable table = new System.Collections.Hashtable();
                List<PatInfo> patList = new List<PatInfo>();
                string key = "";
                foreach (PatInfo item in _PatList)
                {
                    key = item.Fare + item.TAX + item.RQFare + item.SeatGroup;
                    if (!table.ContainsKey(key))
                    {
                        table.Add(key, item);
                        patList.Add(item);
                    }
                }
                return patList;
            }
        }
        private List<PatInfo> _PatList = new List<PatInfo>();

        /// <summary>
        /// Pat列表信息 按照价格Fare 由低到高排序
        /// </summary>
        public List<PatInfo> PatList
        {
            get { return _PatList; }
            set { _PatList = value; }
        }

        private PatInfo childPat;

        /// <summary>
        /// 子舱位实体
        /// </summary>
        public PatInfo ChildPat
        {
            get { return childPat; }
            set { childPat = value; }
        }


        private PatInfo childYPat;

        /// <summary>
        /// 儿童Y舱位价格实体
        /// </summary>
        public PatInfo ChildYPat
        {
            get { return childYPat; }
            set { childYPat = value; }
        }

        private decimal _MaxPrice = 10000000m;
        /// <summary>
        /// 输入价格上限 默认1000万（10000000）
        /// </summary>
        public decimal MaxPrice
        {
            get
            {
                return _MaxPrice;
            }
            set
            {
                _MaxPrice = value;
            }
        }
        private bool _IsOverMaxPrice = false;

        /// <summary>
        /// PAT总价格是否超出设置最大价格上限 true超过（溢出） false 没有超过（没有溢出）
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
        private string _PatCon = string.Empty;
        /// <summary>
        /// Pat内容
        /// </summary>
        public string PatCon
        {
            get
            {
                return _PatCon;
            }
            set
            {
                _PatCon = value;
            }
        }
        /// <summary>
        /// 高低价格字符串显示 格式: 低舱位价|机建|燃油|总价##高舱位价|机建|燃油|总价
        /// </summary>
        private string GDPriceShow
        {
            get
            {
                string strPrice = "0.00|0.00|0.00|0.00##0.00|0.00|0.00|0.00";
                if (PatList != null && PatList.Count > 0)
                {
                    //排序从低到高排序
                    PatList.Sort(delegate(PatInfo pat1, PatInfo pat2)
                    {
                        decimal d1 = 0m;
                        decimal d2 = 0m;
                        decimal.TryParse(pat1.Fare, out d1);
                        decimal.TryParse(pat2.Fare, out d2);
                        return decimal.Compare(d1, d2);
                    });
                    PatInfo First = PatList[0];
                    PatInfo Last = PatList[PatList.Count - 1];
                    strPrice = First.Fare + "|" + First.TAX + "|" + First.RQFare + "|" + First.Price + "##" + Last.Fare + "|" + Last.TAX + "|" + Last.RQFare + "|" + Last.Price;
                }
                return strPrice;
            }
        }
        /// <summary>
        /// 修改字符串价格 基建和燃油
        /// </summary>
        /// <param name="strFare"></param>
        /// <param name="strTAX"></param>
        /// <param name="strRQFare"></param>
        /// <param name="strPrice"></param>
        /// <returns></returns>
        public string UpdatePrice(string strTAX, string strRQFare)
        {
            StringBuilder sbPat = new StringBuilder();
            if (this.PatType == "1")
            {
                sbPat.Append(">PAT:A                                                                         \r");
            }
            else if (this.PatType == "2")
            {
                sbPat.Append(">PAT:A*CH                                                                       \r");
            }
            else if (this.PatType == "3")
            {
                sbPat.Append(">PAT:A*IN                                                                       \r");
            }
            for (int i = 0; i < this.PatList.Count; i++)
            {
                sbPat.Append(this.PatList[i].ReplacePriceToString(strTAX, strRQFare));
            }
            return sbPat.ToString();
        }
    }
}
