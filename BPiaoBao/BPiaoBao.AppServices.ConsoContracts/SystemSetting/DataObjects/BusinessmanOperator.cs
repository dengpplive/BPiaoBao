using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.AppServices.ConsoContracts.SystemSetting.DataObjects
{
    public class BusinessmanOperator
    {
        /// <summary>
        /// 商户号
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 商户名
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 联系方式
        /// </summary>
        public ContactWayDataObject ContactWay { get; set; }
        /// <summary>
        /// 订票联系人
        /// </summary>
        public OperatorDataObject Operator { get; set; }
    }
    public class OperatorDataObject
    {
        public string Realname { get; set; }
        public string Phone { get; set; }
        public string Tel { get; set; }
    }
}
