using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/*
* CLR版本: 4.0.30319.34014
* 文件名：SendPNRInfo
* 命名空间：BPiaoBao.Client.DomesticTicket.Model
* 类名：SendPNRInfo
* 用户名：duanwei
* 创建日期：2014/5/15 15:03:45
* 描述：
*/
namespace BPiaoBao.Client.DomesticTicket.Model
{
  public  class SendPNRInfo
    {
      public string PnrCode { get; set; }
      public decimal Discount { get; set; }
      public string FlightModel { get; set; }
    }
}
