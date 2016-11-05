using BPiaoBao.AppServices.ConsoContracts.DomesticTicket.DataObjects;
using BPiaoBao.Common.Enums;
using PnrAnalysis;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Web;

namespace BPiaoBao.Web.SupplierManager
{
    /// <summary>
    /// EnumTypeHandler 的摘要说明
    /// </summary>
    public class EnumTypeHandler : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "application/x-javascript";
            List<Type> list = new List<Type>()
            {
                typeof(EnumTravelType),
                typeof(EnumReleaseType),
                typeof(EnumIssueTicketWay),
                typeof(EnumApply),
                typeof(EnumPayStatus),
                typeof(EnumPayMethod),
                typeof(EnumOrderStatus),
                typeof(EnumTfgProcessStatus),
                typeof(EnumPolicySourceType),
                typeof(EnumSmsTemplateType),
                typeof(EnumSkyWayType),
                typeof(FixedOnSaleType),
                typeof(EnumAriChangNotifications),
                typeof(EnumRefuse)
            };
            StringBuilder sb = new StringBuilder();
            foreach (Type item in list)
            {
                string arrayName = item.Name;
                sb.AppendFormat("var {0} = new Array();", arrayName);
                foreach (var enumItem in Enum.GetNames(item))
                {
                    var obj = Enum.Parse(item, enumItem, false);
                    string descStr = enumItem;
                    var descModel = obj.GetType().GetField(enumItem).GetCustomAttributes(typeof(DescriptionAttribute), false).OfType<DescriptionAttribute>().FirstOrDefault();
                    if (descModel != null)
                        descStr = descModel.Description;
                    sb.AppendFormat("{0}[{1}]={{Value:{1},Text:'{3}',Description:'{2}'}};", arrayName, (int)obj, descStr, enumItem);
                }
            }
            //航空公司
            sb.Append("var CarrayList=new Array();");
          var carrayList=  new PnrResource().CarrayDictionary.CarrayList;
            for (int i = 0; i < carrayList.Count; i++)
			{
                sb.AppendFormat("CarrayList[{0}]={{Value:'{1}',Text:'{2}'}};", i, carrayList[i].AirCode, carrayList[i].AirCode + "-" + carrayList[i].Carry.AirName);
			}
         
            context.Response.Write(sb.ToString());
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}