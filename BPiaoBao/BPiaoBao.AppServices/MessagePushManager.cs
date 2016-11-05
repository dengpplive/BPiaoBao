using BPiaoBao.AppServices.Contracts.DomesticTicket;
using BPiaoBao.AppServices.Contracts.ServerMessages;
using BPiaoBao.AppServices.DataContracts.DomesticTicket;
using BPiaoBao.AppServices.DomesticTicket;
using BPiaoBao.Common;
using BPiaoBao.Common.Enums;
using BPiaoBao.DomesticTicket.Domain.Models.Orders;
using BPiaoBao.DomesticTicket.EFRepository;
using BPiaoBao.SystemSetting.Domain.Models.Businessmen;
using BPiaoBao.SystemSetting.Domain.Services.Auth;
using JoveZhao.Framework;
using JoveZhao.Framework.DDD;
using JoveZhao.Framework.Excel;
using JoveZhao.Framework.Helper;
using StructureMap;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Threading.Tasks;

namespace BPiaoBao.AppServices
{
    /// <summary>
    /// 服务消息推送管理
    /// </summary>
    public class MessagePushManager
    {
        /// <summary>
        /// 订阅列表
        /// </summary>
        public static ConcurrentDictionary<Guid, PushInfo> PushList = new ConcurrentDictionary<Guid, PushInfo>();

        public static ConcurrentDictionary<Guid, OffLineMessage> OffLineMessages = new ConcurrentDictionary<Guid, OffLineMessage>();

        /// <summary>
        /// 订阅
        /// </summary>
        /// <param name="codeAccount"></param>
        /// <param name="callback"></param>
        public static void Subscriber(Guid guid, string code, string account, IPublisherEvents callback)
        {
            PushInfo pushInfo = new PushInfo(code, account, callback);
            PushList.AddOrUpdate(guid, pushInfo, (a, b) => { return pushInfo; });
        }

        /// <summary>
        /// 取消订阅
        /// </summary>
        /// <param name="codeAccount"></param>
        public static void UnSubscriber(Guid guid)
        {
            PushInfo pushInfo = null;
            if (PushList.ContainsKey(guid))
                PushList.TryRemove(guid, out pushInfo);
        }

        /// <summary>
        /// 发送所有客户端[Buyer]
        /// </summary>
        public static void SendAll(EnumPushCommands command, string content, bool isRepeatSend = false, params object[] param)
        {
            IBusinessmanRepository _repository = StructureMap.ObjectFactory.GetInstance<IBusinessmanRepository>();
            //获取所有商户信息
            var sendArray = _repository.FindAll().OfType<Buyer>().Select(p => p.Code).ToArray();
            foreach (var item in sendArray)
            {
                Send(item, command, content, isRepeatSend, param);
            }
        }

        /// <summary>
        /// 发送给指定的运营商
        /// </summary>
        /// <param name="carrierCodes"></param>
        /// <param name="command"></param>
        /// <param name="content"></param>
        /// <param name="isRepeatSend"></param>
        /// <param name="param"></param>
        public static void SendByCarrier(string[] carrierCodes, EnumPushCommands command, string content, bool isRepeatSend = false, params object[] param)
        {
            IBusinessmanRepository _repository = StructureMap.ObjectFactory.GetInstance<IBusinessmanRepository>();
            //获取发送商户
            var sendArray = _repository.FindAll().OfType<Buyer>().Where(p => carrierCodes.Contains(p.CarrierCode)).Select(p => p.Code).ToArray();
            SendMsgByBuyerCodes(sendArray, command, content, isRepeatSend, param);
        }

        /// <summary>
        /// 发送给指定商户
        /// </summary>
        /// <param name="businessmanCodes">商户号组</param>
        public static void SendMsgByBuyerCodes(string[] businessmanCodes, EnumPushCommands command, string content, bool isRepeatSend = false, params object[] param)
        {
            foreach (var item in businessmanCodes)
            {
                Send(item, command, content, isRepeatSend, param);
            }
        }

        /// <summary>
        /// 发送方法
        /// </summary>
        /// <param name="pushInfo"></param>
        /// <param name="title">标题</param>
        /// <param name="command">命令</param>
        /// <param name="IsRepeatSend">发送失败是否保存消息下次发送</param>
        /// <param name="content">内容</param>
        /// <param name="param">附加参数</param>
        public static void Send(KeyValuePair<Guid, PushInfo> keyValuePair, EnumPushCommands command, string content, bool IsRepeatSend = false, params object[] param)
        {
            try
            {
                keyValuePair.Value.Callback.Notify(command, content, param);
            }
            catch (CommunicationException)
            {
                UnSubscriber(keyValuePair.Key);
                if (IsRepeatSend)
                    AddOffLine(new OffLineMessage
                    {
                        Code = keyValuePair.Value.Code,
                        Account = keyValuePair.Value.Account,
                        Command = command,
                        MessageContent = content,
                        SendDate = DateTime.Now
                    });
            }
            catch (Exception e)
            {
                JoveZhao.Framework.Logger.WriteLog(JoveZhao.Framework.LogType.ERROR, "发送消息出现异常", e);
            }
        }

        public static void Send(string code, string account, EnumPushCommands command, string content, params object[] param)
        {
            try
            {
                PushList.Where(p => p.Value.Code == code && p.Value.Account == account).ToList().ForEach(p =>
                {
                    ICommunicationObject obj = (ICommunicationObject)p.Value.Callback;
                    if (obj.State == CommunicationState.Opened)
                        p.Value.Callback.Notify(command, content, param);
                    else
                    {
                        PushInfo info = null;
                        PushList.TryRemove(p.Key, out info);
                    }
                });
            }
            catch (Exception e)
            {
                JoveZhao.Framework.Logger.WriteLog(JoveZhao.Framework.LogType.ERROR, "发送消息错误", e);
            }
        }

        /// <summary>
        /// 给商户发送消息
        /// </summary>
        /// <param name="code">商户号</param>
        /// <param name="command">命令</param>
        /// <param name="content">消息内容</param>
        public static void Send(string code, EnumPushCommands command, string content, bool isRepeatSend = false, params object[] param)
        {
            var list = PushList.Where(p => string.Equals(p.Value.Code, code, StringComparison.InvariantCultureIgnoreCase)).ToList();
            bool isSend = false;
            //当前商户是否有用户在线
            if (list != null && list.Count > 0)
            {
                list.ForEach(p =>
                {
                    ICommunicationObject obj = (ICommunicationObject)p.Value.Callback;
                    if (obj.State == CommunicationState.Opened)
                    {
                        p.Value.Callback.Notify(command, content, param);
                        isSend = true;
                    }
                });
            }
            //保存离线消息
            if (!isSend && isRepeatSend)
            {
                AddOffLine(new OffLineMessage
                {
                    Code = code,
                    Account = string.Empty,
                    Command = command,
                    MessageContent = content,
                    SendDate = DateTime.Now
                });
            }
        }

        /// <summary>
        /// 添加离线消息
        /// </summary>
        public static void AddOffLine(OffLineMessage offLineMessage)
        {
            try
            {
                if (offLineMessage != null)
                {
                    KeyValuePair<Guid, OffLineMessage> sendModel = OffLineMessages.Where(p => p.Value.Code == offLineMessage.Code && p.Value.Command == offLineMessage.Command).FirstOrDefault();
                    if (sendModel.Key == Guid.Empty || sendModel.Key == null)
                    {
                        OffLineMessages.TryAdd(Guid.NewGuid(), offLineMessage);
                    }
                    else
                    {
                        sendModel.Value.MessageContent = offLineMessage.MessageContent;
                        sendModel.Value.SendDate = DateTime.Now;
                    }
                }
            }
            catch (Exception e)
            {
                JoveZhao.Framework.Logger.WriteLog(JoveZhao.Framework.LogType.ERROR, "添加离线消息异常", e);
            }
        }
    }

    /// <summary>
    /// 登录消息保存
    /// </summary>
    public class PushInfo
    {
        public PushInfo(string code, string account, IPublisherEvents callback)
        {
            this.Code = code;
            this.Account = account;
            this.Callback = callback;
        }

        /// <summary>
        /// 商户号
        /// </summary>
        public string Code { get; private set; }

        /// <summary>
        /// 登录帐号
        /// </summary>
        public string Account { get; private set; }

        /// <summary>
        /// 登录唯一标识
        /// </summary>
        public IPublisherEvents Callback { get; private set; }
    }

    /// <summary>
    /// 离线消息保存
    /// </summary>
    public class OffLineMessage
    {
        public string Code { get; set; }

        public string Account { get; set; }

        public EnumPushCommands Command { get; set; }

        public string MessageContent { get; set; }

        public DateTime SendDate { get; set; }
    }

    public class TimeSendMessage : JoveZhao.Framework.ScheduleTask.ITask
    {
        public string TaskName
        {
            get { return "服务推送消息定时清空"; }
        }

        public bool Execute()
        {
            MessagePushManager.OffLineMessages.Where(p => p.Value.SendDate.AddDays(1) > DateTime.Now).ToList().ForEach(p =>
            {
                OffLineMessage of = null;
                MessagePushManager.OffLineMessages.TryRemove(p.Key, out of);
            });
            return true;
        }
    }

    /// <summary>
    /// 超时后的订单失效任务
    /// </summary>
    public class OrderTimeSendMessage : JoveZhao.Framework.ScheduleTask.ITask
    {
        public string TaskName
        {
            get { return "让超时的订单失效"; }
        }

        public bool Execute()
        {
            AuthManager.SaveUser(new CurrentUserInfo
            {
                Code = "系统",
                OperatorAccount = "系统",
                OperatorName = "系统"
            });
            Logger.WriteLog(LogType.INFO, string.Format("{0}:{1}", DateTime.Now, this.TaskName));
            var repository = ObjectFactory.GetInstance<IOrderRepository>();
            var orderService = ObjectFactory.GetInstance<IOrderService>();
            var unitOfWork = ObjectFactory.GetNamedInstance<IUnitOfWork>(EnumModule.DomesticTicket.ToString());
            var unitOfWorkRepository = ObjectFactory.GetNamedInstance<IUnitOfWorkRepository>(EnumModule.DomesticTicket.ToString());
            var pidService = ObjectFactory.GetInstance<BPiaoBao.AppServices.DomesticTicket.PidService>();
            var currentTime = DateTime.Now.AddMinutes(-30);
            var query = repository.FindAll(p => p.CreateTime < currentTime && (p.OrderStatus == EnumOrderStatus.NewOrder || p.OrderStatus == EnumOrderStatus.WaitChoosePolicy || p.OrderStatus == EnumOrderStatus.PaymentInWaiting) && p.OrderType != 2).Take(100).ToList();
            foreach (var item in query)
            {
                if (!string.IsNullOrEmpty(item.OldOrderId) && item.OrderType == 1)
                {
                    var oldOrder = repository.FindAll(p => p.OrderId.Equals(item.OldOrderId)).FirstOrDefault();
                    if (oldOrder != null)
                    {
                        oldOrder.AssocChdCount -= item.Passengers.Count();
                        unitOfWorkRepository.PersistUpdateOf(oldOrder);
                    }
                }
                //接口订单调用取消接口订单或者白屏预定取消订单取消编码
                if (item.Policy == null)
                {
                    item.OrderStatus = EnumOrderStatus.Invalid;
                    bool isSuc = false;
                    if (item.PnrSource == EnumPnrSource.CreatePnr && pidService.CanCancel(item.BusinessmanCode, item.YdOffice, item.PnrCode))
                    {
                        isSuc = pidService.CancelPnr(item.BusinessmanCode, item.YdOffice, item.PnrCode);
                    }
                    item.WriteLog(new OrderLog
                    {
                        IsShowLog = true,
                        OperationContent = string.Format("取消编码{0}{1}", item.PnrCode, (isSuc ? "成功" : "失败")),
                        OperationDatetime = DateTime.Now,
                        OperationPerson = "系统"
                    });
                    continue;
                }
                orderService.CancelOrder(item.OrderId, item.PnrSource == EnumPnrSource.CreatePnr);
                unitOfWorkRepository.PersistUpdateOf(item);
            }
            unitOfWork.Commit();
            return true;
        }
    }

    /// <summary>
    /// 获取QT信息
    /// </summary>
    public class GetQTInfo : JoveZhao.Framework.ScheduleTask.ITask
    {
        public GetQTInfo(QTSetting info)
        {
            this.qtSet = info;
        }

        public QTSetting qtSet { get; set; }

        public string TaskName
        {
            get { return "定时获取QT信息"; }
        }

        public bool Execute()
        {
            QTSetting p = this.qtSet;
            if (!string.IsNullOrEmpty(p.Code) && p.IsOpen)
            {
                DateTime now = DateTime.Now;
                DateTime startTime = DateTime.Parse(p.QTStartTime);
                DateTime endTime = DateTime.Parse(p.QTEndTime);
                if (now.Hour >= startTime.Hour && now.Hour <= endTime.Hour)
                {
                    var pidService = ObjectFactory.GetInstance<BPiaoBao.AppServices.DomesticTicket.PidService>();
                    var repository = ObjectFactory.GetInstance<AriChangeRepository>();
                    var orderSrrvice = ObjectFactory.GetInstance<OrderService>();
                    //var orderStationSrrvice = ObjectFactory.GetInstance<OrderService>();
                    var unitOfWork = ObjectFactory.GetNamedInstance<IUnitOfWork>(EnumModule.DomesticTicket.ToString());
                    string _content = "您好！接到航空公司通知，您预订的此编码航班有变动，请及时通知旅客。若有疑问，请咨询航空公司客服（航司电话请在右上角“客规”里查看），谢谢！";
                    var unitOfWorkRepository = ObjectFactory.GetNamedInstance<IUnitOfWorkRepository>(EnumModule.DomesticTicket.ToString());
                    var pidResult = pidService.GetPids().Where(p1 => p1.CarrierCode == p.Code || p1.SupplierCode == p.Code).ToList();
                    //循环不同的Office的配置
                    pidResult.ForEach(pidInfo =>
                    {
                        var r = pidService.SendQT(pidInfo);
                        var result = orderSrrvice.CreateAirChangeInfo(r);
                        result.ForEach(y =>
                        {
                            unitOfWorkRepository.PersistCreationOf(y);
                        });
                        unitOfWork.Commit();
                        result.ForEach(t => {
                            orderSrrvice.CreateAirChangeCoordion(EnumAriChangNotifications.AutoPopMessage, true, _content, t.Id);
                        });
                    });
                }
            }
            return true;
        }
    }

    /// <summary>
    /// 用户行为写入任务
    /// </summary>
    public class UserBehavior : JoveZhao.Framework.ScheduleTask.ITask
    {
        public string TaskName
        {
            get { return "用户行为"; }
        }

        public bool Execute()
        {
            var unitOfWork = ObjectFactory.GetNamedInstance<IUnitOfWork>(EnumModule.SystemSetting.ToString());
            var unitOfWorkRepository = ObjectFactory.GetNamedInstance<IUnitOfWorkRepository>(EnumModule.SystemSetting.ToString());

            for (int i = 0; i < 100; i++)
            {
                if (UserBehaviorManage.list.Count > 0)
                {
                    BPiaoBao.SystemSetting.Domain.Models.Behavior.BehaviorStat behavior = null;
                    if (UserBehaviorManage.list.TryTake(out behavior))
                        unitOfWorkRepository.PersistCreationOf(behavior);
                }
            }
            unitOfWork.Commit();

            return true;
        }
    }

    public class OpenTicketTask : JoveZhao.Framework.ScheduleTask.ITask
    {
        private const string _taskName = "Open票扫描";

        public string TaskName
        {
            get { return _taskName; }
        }

        public bool Execute()
        {
            var unitOfWork = ObjectFactory.GetNamedInstance<IUnitOfWork>(EnumModule.SystemSetting.ToString());
            var unitOfWorkRepository = ObjectFactory.GetNamedInstance<IUnitOfWorkRepository>(EnumModule.SystemSetting.ToString());
            var openScanRepository = ObjectFactory.GetInstance<IOPENScanRepository>();
            var pidService = ObjectFactory.GetInstance<BPiaoBao.AppServices.DomesticTicket.PidService>();

            var scanList = openScanRepository.FindAll(p => p.State == EnumOPEN.NoScan).ToList();
            if (scanList.Count == 0)
                return false;
            scanList.ForEach(p =>
            {
                p.State = EnumOPEN.Scanning;
                unitOfWorkRepository.PersistUpdateOf(p);
            });
            unitOfWork.Commit();

            Parallel.ForEach(scanList, (item) =>
            {
                string result = HttpHelper.Get(item.TemplateName);
                var ticketNumList = result.Split(new string[] { "\r", "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries).ToList();
                OpenTicketResponse openTicketResponse = pidService.ScanOpenTicket(item.IP, item.Port.ToString(), item.OfficeNum, ticketNumList);
                item.ScanCount = openTicketResponse.OpenTKList.Count;
                item.OPENCount = openTicketResponse.OpenTKList.Where(p => string.Equals(p.TKStatus, "OPEN FOR USE", StringComparison.OrdinalIgnoreCase)).Count();
                item.State = EnumOPEN.Scaned;

                unitOfWorkRepository.PersistUpdateOf(item);
                DataTable dt = new DataTable(item.TemplateName);
                List<KeyValuePair<string, Type>> headArray = new List<KeyValuePair<string, Type>>
            {
                 new KeyValuePair<string,Type>("票号",typeof(string)),
                 new KeyValuePair<string,Type>("扫描状态",typeof(string)),
                 new KeyValuePair<string,Type>("扫描OFFICE",typeof(string)),
                 new KeyValuePair<string,Type>("备注",typeof(string))
            };
                headArray.ForEach(p => dt.Columns.Add(p.Key, p.Value));
                openTicketResponse.OpenTKList.ForEach(p => dt.Rows.Add(p.TKNumber, p.TKStatus, item.OfficeNum, p.DetrData));
                ExportExcelContext export = new ExportExcelContext("Excel2003");
                item.TemplateUrl = export.Write(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Export"), dt);
            });
            unitOfWork.Commit();
            return true;
        }
    }
}
