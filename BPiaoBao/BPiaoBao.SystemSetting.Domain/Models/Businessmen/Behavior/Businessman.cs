using BPiaoBao.SystemSetting.Domain.Models.SMS;
using JoveZhao.Framework.SMS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JoveZhao.Framework.Expand;
using BPiaoBao.Common.Enums;
using JoveZhao.Framework;
using BPiaoBao.Common;

namespace BPiaoBao.SystemSetting.Domain.Models.Businessmen
{
    /// <summary>
    /// 商户员工行为
    /// </summary>
    public partial class Businessman
    {
        /// <summary>
        /// 查找员工
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public Operator FindByAccount(string account)
        {
            var vm = this.Operators.SingleOrDefault(p => p.Account == account);
            if (vm == null)
                throw new CustomException(404, "账户不存在");
            return vm;

        }
        /// <summary>
        /// 创建新操作员
        /// </summary>
        /// <param name="oper"></param>
        public void NewOperator(Operator oper)
        {
            if (!this.CheckAccountExists(oper.Account))
            {
                oper.Password = oper.Password.Md5();
                oper.CreateDate = DateTime.Now;
                this.Operators.Add(oper);
            }
            else
            {
                throw new CustomException(500, "账户已存在!");
            }

        }
        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="account">账户</param>
        /// <param name="password">密码</param>
        /// <returns></returns>
        public Operator GetOperatorByPasswordAndAccount(string account, string password)
        {
            string md5Password = password.Md5();
            var ope = this.Operators.Where(p => string.Equals(p.Account, account, StringComparison.InvariantCultureIgnoreCase) && p.Password == md5Password).FirstOrDefault();
            return ope;
        }
        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="account"></param>
        /// <param name="newPassword"></param>
        public void ChangePassword(string account, string oldPassword, string newPassword)
        {
            var vm = this.FindByAccount(account);
            if (vm != null)
            {
                if (vm.Password != oldPassword.Md5())
                    throw new CustomException(404, "原始密码输入错误!");
                vm.Password = newPassword.Md5();
            }
        }
        /// <summary>
        /// 搜索员工
        /// </summary>
        /// <param name="realName"></param>
        /// <param name="account"></param>
        /// <param name="operatorState"></param>
        /// <returns></returns>
        public IQueryable<Operator> GetOperatorsBySearch(string realName, string account, EnumOperatorState? operatorState)
        {
            var query = this.Operators.AsQueryable();
            if (!string.IsNullOrEmpty(realName) && !string.IsNullOrEmpty(realName.Trim()))
                query = query.Where(p => p.Realname.Contains(realName.Trim()));
            if (!string.IsNullOrEmpty(account) && !string.IsNullOrEmpty(account.Trim()))
                query = query.Where(p => p.Account.ToLower().Contains(account.Trim().ToLower()));
            if (operatorState != null)
                query = query.Where(p => p.OperatorState == operatorState.Value);
            return query;
        }
        /// <summary>
        /// 当前账户是否重复
        /// </summary>
        /// <param name="account"></param>
        /// <returns>存在True</returns>
        public bool IsExistAccount(string account)
        {
            return this.Operators.Where(p => p.Account == account).Count() == 0;
        }
        /// <summary>
        /// 移除员工
        /// </summary>
        /// <param name="account"></param>
        public void RemoveOperator(string account)
        {
            var currentAccount = FindByAccount(account);
            if (currentAccount != null)
                this.Operators.Remove(currentAccount);
        }
        /// <summary>
        /// 验证账户是否存在
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public bool CheckAccountExists(string account)
        {
            return this.Operators.SingleOrDefault(p => p.Account == account) != null ? true : false;
        }
        /// <summary>
        /// 修改员工信息
        /// </summary>
        /// <param name="op"></param>
        public void UpdateOperator(Operator op)
        {
            Operator vm = this.FindByAccount(op.Account);
            vm.Realname = op.Realname;
            vm.Phone = op.Phone;
            //  vm.Password = op.Password.Md5();
        }
        /// <summary>
        /// 启用，禁用
        /// </summary>
        /// <param name="account"></param>
        public void CanExecute(string account)
        {
            var vm = this.FindByAccount(account);
            vm.OperatorState = vm.OperatorState == EnumOperatorState.Normal ? EnumOperatorState.Frozen : EnumOperatorState.Normal;
        }

    }
    /// <summary>
    /// 商户短信
    /// </summary>
    public partial class Businessman
    {
        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="sendDetail"></param>
        public void SendMessage(string oName, string receiveName, string receiveNum, string content)
        {
            Tuple<int, bool> result = SMSServiceFactory.GetEmailService().SMS(receiveNum, content);
            SendDetail detail = new SendDetail
                {
                    Content = content,
                    Name = oName,
                    ReceiveName = receiveName,
                    ReceiveNum = receiveNum,
                    SendTime = DateTime.Now,
                    SendState = result.Item2,
                    SendCount = result.Item1
                };
            if (result.Item2)
            {
                this.SMS.Send(result.Item1);
            }
            this.SendDetails.Add(detail);
        }
        /// <summary>
        /// 消息发送记录列表
        /// </summary>
        /// <param name="currentPageIndex">当前页</param>
        /// <param name="pageSize">分页大小【默认10】</param>
        /// <param name="startTime">查询开始时间</param>
        /// <param name="endTime">截至时间</param>
        /// <returns></returns>
        public Tuple<int, IList<SendDetail>> GetSendRecordByPage(int currentPageIndex, int? pageSize, DateTime? startTime, DateTime? endTime)
        {
            int pSize = pageSize ?? 10;
            int count = ((currentPageIndex == default(int) ? 1 : currentPageIndex) - 1) * pSize;
            var query = this.SendDetails.AsQueryable();
            if (startTime.HasValue)
                query = query.Where(p => p.SendTime >= startTime.Value);
            if (endTime.HasValue)
                query = query.Where(p => p.SendTime <= endTime.Value);

            var totalCount = query.Count();
            var list = query.OrderByDescending(x => x.SendTime).Skip(count).Take(pSize).ToList();

            return Tuple.Create<int, IList<SendDetail>>(totalCount, list);
        }
        public void BuySms(BuyDetail buyDetail)
        {
            this.BuyDetails.Add(buyDetail);
        }
        public string GetPayNo()
        {
            return string.Format("{0}{1}", DateTime.Now.ToString("yyyyMMddHHmmssfff"), new Random().Next(1000, 9999).ToString());
        }
        /// <summary>
        /// 购买短信
        /// </summary>
        /// <param name="oName">操作员姓名</param>
        /// <param name="count">购买条数</param>
        /// <param name="payWay">支付方式</param>
        /// <param name="payAmount">支付金额</param>
        /// <returns>支付URL地址信息</returns>
        public string BuySms(string oName, int count, EnumPayMethod payWay, decimal payAmount)
        {
            string payNo = string.Format("{0}{1}", DateTime.Now.ToString("yyyyMMddHHmmssfff"), new Random().Next(1000, 9999).ToString());
            BuyDetail buyDetail = new BuyDetail
            {
                BuyState = EnumPayStatus.NoPay,
                PayNo = payNo,
                BuyTime = DateTime.Now,
                Count = count,
                Name = oName,
                PayAmount = payAmount,
                PayWay = payWay,
                PayFee = Math.Round(payAmount * SystemConsoSwitch.Rate, 2)
            };
            this.BuyDetails.Add(buyDetail);
            return payNo;
        }
        /// <summary>
        /// 异步通知更新
        /// </summary>
        public void SMSNotify(string payno, string outPayno,EnumPayMethod payMethod)
        {
            var model = this.BuyDetails.Where(x => x.PayNo == payno).SingleOrDefault();
            model.OutPayNo = outPayno;
            model.PayTime = DateTime.Now;
            model.BuyState = EnumPayStatus.OK;
            model.PayWay = payMethod;
            this.SMS.Buy(model.Count);
        }
        /// <summary>
        /// 消息发送记录列表
        /// </summary>
        /// <param name="currentPageIndex">当前页</param>
        /// <param name="pageSize">分页大小【默认10】</param>
        /// <param name="startTime">查询开始时间</param>
        /// <param name="endTime">截至时间</param>
        /// <param name="outTradeNo"></param>
        /// <returns></returns>
        public Tuple<int, IList<BuyDetail>> GetBuyRecordByPage(int currentPageIndex, int? pageSize, DateTime? startTime, DateTime? endTime, string outTradeNo = null)
        {
            int pSize = pageSize ?? 10;
            int count = ((currentPageIndex == default(int) ? 1 : currentPageIndex) - 1) * pSize;

            var query = this.BuyDetails.AsQueryable();
            if (startTime.HasValue)
                query = query.Where(p => p.BuyTime >= startTime.Value);
            if (endTime.HasValue)
                query = query.Where(p => p.BuyTime <= endTime.Value);
            if (!string.IsNullOrWhiteSpace(outTradeNo))
            {
                query = query.Where(p => p.OutPayNo == outTradeNo.Trim());
            }
            var totalCount = query.Count();
            var list = query.OrderByDescending(x => x.BuyTime).Skip(count).Take(pSize).ToList();

            return Tuple.Create<int, IList<BuyDetail>>(totalCount, list);
        }
    }
}
