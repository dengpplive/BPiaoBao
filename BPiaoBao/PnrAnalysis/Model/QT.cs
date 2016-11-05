using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace PnrAnalysis.Model
{
    /// <summary>
    /// Q信箱
    /// </summary>
    [Serializable]
    public class QT_Queue
    {
        //种类        
        public enum QT_Type : int
        {
            /// <summary>
            /// 综合QUEUE，用于一些无法识别其种类或本部门没有建立的某种Q
            /// </summary>
            GQ = 0,
            /// <summary>
            /// 自由格式，用于代理之间的相互联系
            /// </summary>
            RP = 1,
            /// <summary>
            /// 座位证实回复电报QUEUE
            /// </summary>
            KK = 2,
            /// <summary>
            /// 再订座QUEUE
            /// </summary>
            RE = 3,
            /// <summary>
            /// 特殊服务电报(SSR)
            /// </summary>
            SR = 4,
            /// <summary>
            /// 票号更改QUEUE
            /// </summary>
            TC = 5,
            /// <summary>
            /// TL:出票时限(TK TL)QUEUE
            /// </summary>
            TL = 6,
            /// <summary>
            /// 航班更改通知QUEUE
            /// </summary>
            SC = 7,
            /// <summary>
            /// 店预定的信息QUEUE
            /// </summary>
            HT = 8,
            /// <summary>
            /// 国际订座PNR信箱
            /// </summary>
            IB = 9
        }
        [Serializable]
        public class QT_Item
        {
            public QT_Item(string _Qtype, string _Capacity, string _MaxCapacity)
            {
                this.Qtype = _Qtype;
                this.Capacity = _Capacity;
                this.MaxCapacity = _MaxCapacity;
                this.QT_Type = (QT_Type)Enum.Parse(typeof(QT_Type), _Qtype);
                int _CapacityNumber = 0, _MaxCapacityNumber = 0;
                int.TryParse(_Capacity, out _CapacityNumber);
                int.TryParse(_MaxCapacity, out _MaxCapacityNumber);
                CapacityNumber = _CapacityNumber;
                MaxCapacityNumber = _MaxCapacityNumber;
            }

            public string Qtype { get; set; }
            public string Capacity { get; set; }
            public string MaxCapacity { get; set; }
            public QT_Type QT_Type { get; set; }
            public int CapacityNumber { get; set; }
            public int MaxCapacityNumber { get; set; }
        }
        private string _Office = string.Empty;
        public string Office
        {
            get { return _Office; }
            set { _Office = value; }
        }
        private List<QT_Item> _QTList = new List<QT_Item>();
        public List<QT_Item> QTList
        {
            get { return _QTList; }
            set { _QTList = value; }
        }
    }

}
