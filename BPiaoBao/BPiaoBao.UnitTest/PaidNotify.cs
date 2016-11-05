using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BPiaoBao.UnitTest
{
    [TestClass]
    public class PaidNotify
    {
        [TestMethod]
        public void TestMethod1()
        {

            //517 8000YI  易行需要签名

            //票盟
            if ("Status" == "10")//票盟
            {
                /*
                Cmd=SUBMITORDERSTATUS 
                Status=10 
                orderId=131205183567156 //接口订单号
                neworderId=131205183567156 //申请退废票升舱生成的新订单号
                transNo=2013120514058268 //交易流水号
                */
            }
            if ("NotifyType" == "支付宝支付通知")//517
            {
                /*
                 *  NotifyType =支付宝支付通知 
                    OrderId =131205182706490191 
                    TradeID =2013120515407946 
                    CreateTime =2013-12-05 18:27 
                    PNR =HXB6E9 
                    Passenger =CHENG/ZHUO 
                    Voyage =北京-兰州 
                    State =已经付款，等待出票 
                    TotalPay =440.00 
                    FlightNo =CA1273 
                    PayTime =2013-12-05 18:27 
                    BigPNR =MZY3B7 
                    OfficeCode =BJS182 
                    Sign =A753F9E724369386BFD6DCDB9B714109 
                 * */
            }
            //  8000YI 支付通知
            if ("type" == "1")
            {
                //notifymsg  =订单号^交易号^PNR^支付日期^支付金额(支付通知)
                //platform =8000YI 
                //type =1 
                //orderguid =I635220850525394286 
                //orderstate =2 
                //key =3502B775FA57B04ED584F6BCA19B86FD 
                //notifymsg =I635220850525394286^2013120829050843^JQZH2R^2013-12-08 07:33:25^702.00 
            }
            //百拓
            if ("messageType" == "1")
            {
                /*
                 *  orderID=f18826171 //接口订单号
                    PaymentStatus=Y 
                    PaymentMode=2  //1 快钱  2 支付宝
                    SystermId=2013120514039368 //交易号
                    shouldPaid=1023.00  //支付金额
                    produceType=1 
                    messageType=1 
                    portorderid=0131205182756413251 //用户订单号
                 */
            }
            //易行
            if ("type" == "2")//2：支付成功通知
            {
                //orderid  易行订单号
                //payid   交易号
                //totalPrice 支付金额
                //payplatform  支付平台 1—支付宝 2—汇付  7—财付通
                //payType  1：自动扣款，2：收银台支付

                //payid =2013120556623886 
                //totalPrice =737.00 
                //orderid =T2013120513195 
                //type =2 
                //payplatform =1 
                //paytype =1 
                //sign =575fb9747b8f6ef4196a18f8fc3eb588 
            }

            //51Book暂时没有代扣通知
            //今日暂时不知


        }
    }
}
