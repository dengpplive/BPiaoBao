using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BPiaoBao.AppServices;
using BPiaoBao.Common;
using BPiaoBao.DomesticTicket.Domain.Services;
using BPiaoBao.DomesticTicket.Domain.Services.B2BParam;
using BPiaoBao.UnitTest.DomesticTicket;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StructureMap;

namespace BPiaoBao.UnitTest
{
    [TestClass]
    public class BSPAutoDZ
    {
        [TestMethod]
        public void Test1()
        {           
        }
        [TestMethod]
        public void Test()
        {
            /*
             * BSP出票步骤
             * 1.发送RT指令检查编码是否已出票，已出票解析出票号和对应的乘客姓名
             * 2.编码没有出票,发送RT指令解析编码中的多余项Xe掉,这个需要多次翻页Xe,最后再次发送RT解析航段序号和出票时限序号记录
             * 3.判断编码是成人还是儿童，分别发送PAT获取成人或者儿童的PAT价格信息,对比PAT中是否存在要出票的价格，不存在结束出票，出票失败.存在获取出票价格的PAT序号
             * 4.将出票的价格填进编码,发送指令:RT编码|PAT价格|SFC:序号|@ 
             * 5.检查价格是否填入编码,发送出票指令:RT编码|XE出票时限序号|出票航段序号RR|ETDZ 打票机号
             * 6.检查返回数据中是否含有票号和该编码，有出票成功解析出票号 但是这个没有关联对应的乘客，按顺序获取。 没有票号再次发送RT编码指令 从返回结果中获取票号，没有则失败            
             */

            //string strIP = "210.14.139.29";
            //string strPort = "2232";
            //string strCpOffice = "CTU186";
            //string strPrintNo = "10";
            //string strPnr = "abcdef";
            //string strCpPrice = "100";

            PnrAnalysis.FormatPNR format = new PnrAnalysis.FormatPNR();
            //            string strData = @"1.邵文龙 HVH5JJ   2.  SC1194 L   FR11JUL  CKGTNA HK1   0810 1005          E   
            //3.CTU/T CTU/T 028-5566222/CTU QI MING INDUSTRY CO.,LTD/TONG LILI ABCDEFG   
            //4.TL/0540/11JUL/CTU324   
            //5.SSR FOID SC HK1 NI08-70019/P1   
            //6.OSI SC CTCM13640550770/P1   
            //7.OSI SC CTCT13981780660   
            //8.RMK CA/NCVTX6   
            //9.CTU324        
            //>PAT:A  
            //01 L FARE:CNY770.00 TAX:CNY50.00 YQ:CNY120.00  TOTAL:940.00   
            //SFC:01  ";

            //string msg = "";
            //PnrAnalysis.PnrModel pnrmodel = format.GetPNRInfo("HVH5JJ", strData, false, out msg);
            // string RegPasSSRPattern = @"\s*(?<=SSR FOID)\s*(?<carry>\w{2})\s*(?<state>[A-Za-z]{2}\d{0,3})\s*(\/)?NI\s*(?<SSRNum>[\w|\(|\)|\-]+)/P(?<Num>\d+)\s*";
            //string ssr = "SSR FOID SC HK1 NI08-70019/P1";

            string strdata = @"  **ELECTRONIC TICKET PNR** 
 1.宋欣芮 2.于佳呈 HF4485   
 3.  CA4153 A   SU06JUL14CTUURC RR2   1615 1950          E T2T2 
 4.URC/T URC/T0991-3677114/WULUMUQITITONGBAITONGJIPIAODAILIYOUXIANGONGSI/   
    /HUANGZHENPING ABCDEFG  
 5.0993-2833333 
 6.TL/1515/06JUL14/URC302   
 7.SSR FOID CA HK1 NI653127199207132212/P2  
 8.SSR FOID CA HK1 NI653101199306232020/P1  
 9.SSR OTHS 1E 1 PNR RR AND PRINTED 
10.SSR OTHS 1E 1 CAAIRLINES ET PNR  
11.SSR ADTK 1E BY URC05JUL14/1601 OR CXL CA ALL SEGS                           +
12.SSR TKNE CA HK1 CTUURC 4153 A06JUL 9992342455793/1/P1                       -
13.SSR TKNE CA HK1 CTUURC 4153 A06JUL 9992342455794/1/P2
14.OSI CA CTCM18721531432/P1
15.OSI CA CTCM15199832222/P2
16.OSI CA CTCT13070099773   
17.OSI 1E CAET TN/9992342455793-9992342455794   
18.RMK CA/MK8MNS
19.RMK TJ AUTH URC221   
20.URC302   
";
            // string errMsg="";
            // PnrAnalysis.PnrModel pnrmodel = format.GetPNRInfo("HF4485", strdata, false, out errMsg);



        }
    }
}
