using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BPiaoBao.AppServices.Hosting;
using BPiaoBao.DomesticTicket.Platforms._BaiTuo;
using System.Collections.Specialized;
using BPiaoBao.DomesticTicket.Platforms._517;
using System.Text;
using BPiaoBao.DomesticTicket.Platforms._8000YI;
using BPiaoBao.DomesticTicket.Platform.Plugin;
using BPiaoBao.AppServices.Hosting.Notify;

namespace BPiaoBao.UnitTest
{
    [TestClass]
    public class notiyfyTest
    {


        [TestMethod]
        public void TestMethod1()
        {
            BootStrapper.ConfigureDependencies();
            PlatformHttpHandler p = new PlatformHttpHandler();
            p.Process(null, null);
            return;

            _8000YIPlatformNotify _8000YI = new _8000YIPlatformNotify();
            //_517PlatformNotify baiTuoPlatformNotify = new _517PlatformNotify();
            //NotifyType=出票通知&OrderId=121130234135710108&DrawABillFlag=0&DrawABillRem
            //ark=&TicketNos=999|2192820521|NI|500225198608150772|艾庄博,999|2192820520|N
            //I|510230196401200318|艾勇&Pnr=JGS4DW&NewPnr= &Sign=B899D1455447E0777EFFDBF3
            //B7C4B7FE
            NameValueCollection nv = new NameValueCollection();
            /*
            platform=8000YI
type=5
orderguid=I635247870605937500
orderstate=28
key=BD42F2D8D6D4203FC47135BCDDE9EFBE
notifymsg=I635247870605937500%5e2014010846896956%5e1712.00%5e%e7%bc%96%e7%a0%81%e5%b7%b2%e5%8f%96%e6%b6%88
areaCity=ctu
    */
            nv.Add("platform", "8000YI");
            nv.Add("type", "5");
            nv.Add("orderguid", "I635247870605937500");
            nv.Add("orderstate", "28");
            nv.Add("key", "BD42F2D8D6D4203FC47135BCDDE9EFBE");
            nv.Add("notifymsg", "I635247870605937500%5e2014010846896956%5e1712.00%5e%e7%bc%96%e7%a0%81%e5%b7%b2%e5%8f%96%e6%b6%88");
            nv.Add("areaCity", "ctu");



            bool b = _8000YI.CanProcess(nv);
            bool c = _8000YI.CanProcess(nv);

            //NotifyType =出票通知 
            //OrderId =140102231401680175 
            //DrawABillFlag =0 
            //DrawABillRemark = 
            //TicketNos =999|2123356533|NI|130421197209255712|杨保民,999|2123356534|NI|511121197212100020|曾英 
            //Pnr =HQ8VYH 
            //NewPnr = 
            //Sign =88EB8681CD34E0E2D1962F94F79C560D 

        }
    }
}
