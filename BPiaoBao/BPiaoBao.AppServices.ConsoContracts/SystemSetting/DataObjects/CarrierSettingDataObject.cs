using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.AppServices.ConsoContracts.SystemSetting.DataObjects
{
    public class CarrierSettingDataObject
    {
        public int ID { get; set; }

        /// <summary>
        /// 航空公司二字码
        /// </summary>
        public string CarrayCode
        {
            get;
            set;
        }

        /// <summary>
        /// 预定编码Office
        /// </summary>
        public string YDOffice
        {
            get;
            set;
        }
        /// <summary>
        /// 出票Office
        /// </summary>
        public string CPOffice
        {
            get;
            set;
        }
        /// <summary>
        /// 打票机号
        /// </summary>
        public string PrintNo
        {
            get;
            set;
        }
    }
}
