
using JoveZhao.Framework;
using StructureMap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BPiaoBao.Common;
using PnrAnalysis;
namespace BPiaoBao.DomesticTicket.Platform.Plugin
{
    public class PlatformFactory
    {
        public static List<PlatformPolicy> GetPoliciesByPnrContent(string pnrContent, bool IsLowPrice, PnrData pnrData)
        {
            List<PlatformPolicy> lst = new List<PlatformPolicy>();
            var plst = ObjectFactory.GetAllInstances<IPlatform>().Where(p => p.IsClosed == false);
            StringBuilder sbLog = new StringBuilder();
            sbLog.Append("查询政策开始:=============================\r\n");
            sbLog.AppendFormat("时间:{0}", System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            if (pnrData.PnrMode != null && pnrData.PnrMode._LegList.Count > 0)
            {
                Parallel.ForEach(plst, p =>
                {
                    System.Diagnostics.Stopwatch swch = new System.Diagnostics.Stopwatch();
                    List<PlatformPolicy> listPlatformPolicy = new List<PlatformPolicy>();
                    try
                    {
                        swch.Start();
                        listPlatformPolicy = p.GetPoliciesByPnrContent(pnrContent, IsLowPrice, pnrData);
                        lst.AddRange(listPlatformPolicy);
                    }
                    catch
                    {
                        Logger.WriteLog(LogType.WARN, "取" + p.GetPlatformName() + "接口政策失败");
                    }
                    finally
                    {
                        swch.Stop();
                        string strTime = swch.Elapsed.ToString();
                        sbLog.AppendFormat("[{0}]平台:{1} 查询政策用时:{2} \r\n", listPlatformPolicy.Count, p.Code, strTime);
                    }
                });
            }
            else
            {
                sbLog.AppendFormat("编码内容解析失败:PnrContent={0}\r\n", pnrContent);
            }
            sbLog.Append("查询政策结束:=============================\r\n");
            Logger.WriteLog(LogType.INFO, sbLog.ToString());
            return lst;
        }
        public static PlatformOrder CreateOrder(string platformName, bool IsLowPrice, string areaCity, string pnrContent, string policyId, string RateId, string localOrderId, decimal policyPoint, decimal ReturnMoney, BPiaoBao.Common.PnrData pnrData)
        {
            var plateform = ObjectFactory.GetNamedInstance<IPlatform>(platformName);
            return plateform.CreateOrder(pnrContent, IsLowPrice, areaCity, policyId,RateId, localOrderId, policyPoint, ReturnMoney, pnrData);
        }

        public static void Pay(string platformName, string areaCity, PlatformOrder order)
        {
            var plateform = ObjectFactory.GetNamedInstance<IPlatform>(platformName);
            plateform.Pay(areaCity, order);
        }
        public static IPlatform GetPlatformByCode(string code)
        {
            return ObjectFactory.GetNamedInstance<IPlatform>(code);
        }
        
    }
}
