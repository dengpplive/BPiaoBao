using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BPiaoBao.UnitTest
{
    [TestClass]
    public class Notify
    {
        [TestMethod]
        public void TestMethod1()
        {
            //DrawABillFlag 出票状态，0 表示出票，1 表示取消出票
            //517 取消出票通知-----------------------------------------------------------------------------------------------
            /*
            NotifyType =出票通知 
            OrderId =131129130938320112 
            DrawABillFlag =1 
            DrawABillRemark =运价不符480 
            TicketNos =781||NI|332623196404100730|颜兴华 
            Pnr =JS99BS 
            NewPnr = 
            Sign =709E3470E61FE3FB81DC8E6DCEE1E6E3 
            */
            /*
            NotifyType =出票通知 
            OrderId =131129110546750187 
            DrawABillFlag =1 
            DrawABillRemark =位置被NO 
            TicketNos =859||NI|460028197305090830|王福海,859||NI|460028195708031211|张以就 
            Pnr =HXZL0L 
            NewPnr = 
            Sign =9028297990DCD0D09F3476B965470799 
            */
            /*
            NotifyType =出票通知 
            OrderId =131129133036950132 
            DrawABillFlag =1 
            DrawABillRemark =换编码出票外放无舱位 
            TicketNos =876||NI|512901196603130053|李川东 
            Pnr =JFY9V9 
            NewPnr = 
            Sign =1E58EBA8EC28C4120FFD8CA94BAD0E57 
            */



            //51book 退票通知-----------------------------------------------------------------------------------------------
            //ticketCenterRefundNo          51book退票单号
            //ticketCenterorderNo           关联51book订单号
            //outerOrderNo                  外部订单号
            //outRefundOrderNo              外部退款批次号
            //refundStatus                  退票状态 S -退票成功 F -退票失败
            //statusMsg                     失败原因描述
            //refundParameters              退款参数   格式：姓名^航段^票号^退款类型^申请时间^退款时间^退款金额^退票手续费^支付宝交易号^行程单邮寄费用退款金额。多个之间以“|”分开
            //param1                        备用
            //param2                        备用
            //param3                        备用

            /* 申请退废票的通知
            sequenceNo：退款单号
            orderNo：订单号
            venderRefundTime：退款时间
            venderPayPrice：退款金额
            refundFee：退款手续费
            //退款失败时才会有
            venderRemark：退款失败的备注
            */





            //8000YI退票通知-----------------------------------------------------------------------------------------------
            //退票  格式:订单号^交易号^退款金额
            /*
                platform =8000YI 
                type =3 
                orderguid =I635228258082213126 
                orderstate =8 
                key =816035C1A2F1B244F64B864BB51B7673 
                notifymsg =I635228258082213126^2013121236520268^365.00^ 
            */

            //废票  格式:订单号^交易号^退款金额
            /*
                platform =8000YI 
                type =4 
                orderguid =I635230470535938750 
                orderstate =11 
                key =0007B2F64160029AED7D253BE50DC0AB 
                notifymsg =I635230470535938750^2013121923417886^4012.00^ 
             * 
             */
            //取消出票 格式:订单号^交易号^退款金额
            /*
                platform =8000YI 
                type =5 
                orderguid =I635230415749845000 
                orderstate =28 
                key =C82DBB4B40B64CAC7BD0F048CE1E05C1 
                notifymsg =I635230415749845000^2013121960717368^3252.00^航空公司系统故障          
             */



            //票盟-----------------------------------------------------------------------------------------------
            //支付完成
            /*
                Cmd=SUBMITORDERSTATUS 
                Status=10 
                orderId=131205183567156 //接口订单号
                neworderId=131205183567156 //申请退废票升舱生成的新订单号
                transNo=2013120514058268 //交易流水号
             */
            //出票完成
            /*
                Cmd=SUBMITORDERSTATUS 
                Status=13 
                orderId=131219001227143 
                neworderId=131219001227143 
                name=黄岩;李健;杜晶 
                ticketno=8802167810104;8802167810105;8802167810103              
             */

            //交易取消退款
            /*
              Cmd=SUBMITORDERSTATUS 
              Status=99 
              orderId=131219101543030968 
              neworderId=131219101543030968 //申请退废票升舱生成的新订单号
              Comment=低价格不享受此政策 
            */
            //完成改期升舱
            /*
             Cmd=SUBMITORDERSTATUS 
             Status=43 
             orderId=13121816530334609G 
             payfee=0  
             */
            //完成退款
            /*
                Cmd=SUBMITORDERSTATUS 
                Status=90 
                orderId=131215142435054346 
                neworderId=13121515100808781E //申请退废票升舱生成的新订单号
                payfee=576.00 
             */
            //无法退票
            /*
                Cmd=SUBMITORDERSTATUS 
                Status=22 
                orderId=131206113755031 
                neworderId=13121613380235406T 
                Comment=取消编码
            */

            //百拓------------------------------------------------------------------------------------------------
            //支付成功消息  messageType=1 
            /*
                orderID=f19051712 
                PaymentStatus=Y 
                PaymentMode=2 
                SystermId=2013121967204585 
                shouldPaid=1560.00 
                produceType=1 
                messageType=1 
                portorderid=0131219004759952032  
            */
            //出票成功消息messageType=2 
            /*
                forderformid=f19051712 
                produceType=1 
                messageType=2 
                portorderid=0131219004759952032 
                tickets=厚淑琴|784-2112715109|HZ4RLQ|HZ4RLQ,米林|784-2112715110|HZ4RLQ|HZ4RLQ,司俊祥|784-2112715111|HZ4RLQ|HZ4RLQ 
                pnr=HZ4RLQ|HZ4RLQ 
             */
            //退款成功消息 messageType=3
            /*
                orderID=f19033440 //百拓订单号
                ReturnStatus=Y //退款状态
                SystermId=2013121909171517390019033440 //交易流水号
                ReturnMoney=1164.4 //退款金额
                produceType=1 
                messageType=3 
                portorderid=0131218084655287206 //平台订单号
                RefundDeatils=赵鹏飞|999-2122455864|1164.4|10.00  多个用","分割
             */
            //拒绝出票消息 messageType=16
            /*
                forderformid	订单号
                produceType	产品类型
                messageType	消息类型 16
                remark	拒绝原因
             */
            //拒绝退/废票的消息 messageType=12
            /*
                orderID	订单号	f764500
                produceType	产品类型	1
                messageType	消息类型	12
                memo	拒绝说明	             
             */
            //改期的消息 messageType=7
            /*
                orderID	订单号	f764500	
                produceType	产品类型	1	
                messageType	消息类型	7	
                changeStatus	改期状态	Y	Y  成功，N失败
                portorderid	用户订单号	1208081105081	
                changeDeatils	改期详细说明	姓名|票号	多个乘机人用“,"隔开就可以了
             */

            //易行------------------------------------------------------------------------------------------------
            //出票成功通知 type=1
            /*
                payid =2013121923159486 
                totalPrice =1294.00 
                orderid =T2013121953823 
                type =2 
                payplatform =1 
                paytype =1 
                sign =7411ff101562e72ddc5a5a32c1dd035d 
             */
            //支付成功通知 type=2
            /*
                payid =2013121923159486 
                totalPrice =1294.00 
                orderid =T2013121953823 
                type =2 
                payplatform =1 
                paytype =1 
                sign =7411ff101562e72ddc5a5a32c1dd035d 
             */
            //取消成功通知  type=3
            /*
                orderid	订单号	50	易行天下订单号（易行天下系统中唯一）一次只能传一个易行订单
                passengerName	乘客姓名	500	(获取时使用urldecode解密)
                type	通知类型	1	3：取消成功通知
                sign	签名		所有参数经MD5加密算法后得出的结果
             */
            //退废票通知    type=4
            /*
                airId	机票票号	500	票号之间用 ^分隔
                refundPrice	实退金额	Double	支付方实际退款金额
                refundStatus	退票状态	1	1—成功 2—拒绝退废票
                refuseMemo	拒绝退票理由	100	(获取时使用urldecode解密)
                procedures	退款手续费	Double	退款时的手续费
                type	通知类型	1	4：退废票通知
                sign	签名	32	所有参数经MD5加密算法后得出的结果
             */
            //改期成功通知 type=5
            /*
                orderid	订单号	50	易行天下订单号（易行天下系统中唯一）一次只能传一个易行订单
                changeMemo	改期或改证件备注	255	
                changeStatus	改期状态	1	1—成功 2—拒绝
                refuseMemo	拒绝理由	100	
                type	通知类型	1	5：改期通知
                sign	签名	32	所有参数经MD5加密算法后得出的结果

             */
            //拒绝出票通知  type=6
            /*
                orderid	订单号	50	易行天下订单号（易行天下系统中唯一）一次只能传一个易行订单
                passengerName	乘客姓名	500	
                refuseMemo	拒绝理由	100	
                type	通知类型	1	6：拒绝出票通知
                sign	签名	32	所有参数经MD5加密算法后得出的结果
             */
            //用户签约通知    type=7
            /*
                appUserName	指定签约用户名	50	指定签约的易行天下用户名
                signStatus	签约状态	1	1—成功 2—失败
                type	通知类型	1	7：签约通知
                sign	签名	32	所有参数经MD5加密算法后得出的结果
             */

            //今日 无-----------------------------------------------------------------------------------------------
        }
    }
}
