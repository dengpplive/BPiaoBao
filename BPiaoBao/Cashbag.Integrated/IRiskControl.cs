using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;

namespace Cashbag.Integrated
{
    [ServiceContract]
    public interface IRiskControl
    {
        /// <summary>
        /// 取最近三个月的流水信息
        /// </summary>
        /// <param name="cashbagCode"></param>
        /// <param name="startIndex"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        [OperationContract]
        DataList<SellSerial> GetSerial(string cashbagCode, int startIndex, int count);
        /// <summary>
        /// 取库存信息
        /// </summary>
        /// <param name="cashbagCode"></param>
        /// <returns></returns>
        [OperationContract]
        DataInventory GetInventory(string cashbagCode);
    }
}
