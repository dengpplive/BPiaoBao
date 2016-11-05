using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace PnrAnalysis.Model
{
    /// <summary>
    /// 指令类型
    /// </summary>
    public enum NewInsType : int
    {
        [Description("预定成人编码")]
        _YD_Adult,
        [Description("预定儿童编码")]
        _YD_Chd,
        [Description("RT编码")]
        _RT,
        [Description("PAT:A 获取成人运价")]
        _PAT_A,
        [Description("PAT:A*CH 获取儿童运价")]
        _PAT_A_CH,
        [Description("PAT:A*CH 获取婴儿运价")]
        _PAT_A_IN,
        [Description("HU特殊指令备注")]
        _HU_SpRmk,
        [Description("RT名字获取编码信息")]
        _RT_Name,
        [Description("儿童编码中备注成人编码")]
        _RT_ChdToAdult,
        [Description("提取大编码的RTR")]
        _RT_RTR,
        [Description("成人编码备注婴儿即添加婴儿")]
        _RT_XN_INF

    }

    /// <summary>
    /// 指令信息
    /// </summary>
    public class InsParam
    {
        public InsParam(string _ServerIP, int _ServerPort, string _Office)
        {
            this.ServerIP = _ServerIP;
            this.ServerPort = _ServerPort;
            this.Office = _Office;
            this.ID = Guid.NewGuid().ToString();
        }
        public string ID { get; set; }
        public string ServerIP { get; set; }
        public int ServerPort { get; set; }
        public string Office { get; set; }
        public NewInsType InsType { get; set; }
        public string SendTime { get; set; }
        public string SendCmd { get; set; }
        public string RecvTime { get; set; }
        public string RecvData { get; set; }

    }
}
