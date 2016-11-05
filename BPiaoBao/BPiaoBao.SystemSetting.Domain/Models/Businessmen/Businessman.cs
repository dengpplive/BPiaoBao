using JoveZhao.Framework.DDD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JoveZhao.Framework.Expand;
using BPiaoBao.SystemSetting.Domain.Models.SMS;
using JoveZhao.Framework;
using BPiaoBao.Common.Enums;

namespace BPiaoBao.SystemSetting.Domain.Models.Businessmen
{
    //商户基类
    public abstract partial class Businessman : EntityBase, IAggregationRoot
    {
        #region 属性
        /// <summary>
        /// 商户号
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 商户名
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 商户创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 联系方式
        /// </summary>
        public ContactWay ContactWay { get; set; }
        /// <summary>
        /// 所属业务员
        /// </summary>
        public string ContactName { get; set; }
        /// <summary>
        /// 联系电话
        /// </summary>
        public string Phone { get; set; }
        /// <summary>
        /// 是否启用【默认启用】
        /// </summary>
        public bool IsEnable { get; set; }
        /// <summary>
        /// 附件
        /// </summary>
        public virtual IList<Attachment> Attachments { get; set; }
        /// <summary>
        /// 操作员
        /// </summary>
        public virtual IList<Operator> Operators { get; set; }
        /// <summary>
        /// 短信购买记录
        /// </summary>
        public virtual IList<BuyDetail> BuyDetails { get; set; }
        /// <summary>
        /// 短信发送记录
        /// </summary>
        public virtual IList<SendDetail> SendDetails { get; set; }
        /// <summary>
        /// 与钱袋子对接用的Key
        /// </summary>
        public string CashbagKey { get; set; }
        /// <summary>
        /// 与钱袋子对接用的商户Code
        /// </summary>
        public string CashbagCode { get; set; }
        /// <summary>
        /// 短信
        /// </summary>
        public virtual SMS.SMS SMS { get; set; }

        /// <summary>
        /// 出票速度
        /// </summary>
        public int IssueSpeed { get; set; }
        /// <summary>
        /// 赠送短信记录
        /// </summary>
        public virtual IList<GiveDetail> GiveDetails { get; set; }
        #endregion
        protected override string GetIdentity()
        {
            return Code;
        }
        public virtual void CheckRule()
        {
            if (string.IsNullOrEmpty(Code) || string.IsNullOrEmpty(Code.Trim()))
                throw new CustomException(400, "请输入商户号!");
            if (string.IsNullOrEmpty(Name) || string.IsNullOrEmpty(Name.Trim()))
                throw new CustomException(400, "请输入商户名称!");

            this.Code = Code.Trim();
            this.Name = Name.Trim();
        }
    }
    /// <summary>
    /// 采购商
    /// </summary>
    public class Buyer : Businessman
    {

        /// <summary>
        /// 座机号
        /// </summary>
        public string Plane { get; set; }
        /// <summary>
        /// 标签|
        /// </summary>
        public string Label { get; set; }
        /// <summary>
        /// 扣点组ID
        /// </summary>
        public int? DeductionGroupID { get; set; }
        /// <summary>
        /// 运营商号
        /// </summary>
        public string CarrierCode { get; set; }
        /// <summary>
        /// 控台分销分组ID
        /// </summary>
        public string StationBuyGroupID{get;set;}

        public override void CheckRule()
        {
            if (string.IsNullOrEmpty(ContactName))
                throw new CustomException(400, "请输入业务员!");
            if (string.IsNullOrEmpty(Phone))
                throw new CustomException(400, "请输入业务员电话!");
            if (string.IsNullOrEmpty(ContactWay.Province))
                throw new CustomException(400, "请选择省份");
            if (string.IsNullOrEmpty(ContactWay.City))
                throw new CustomException(400, "请选择城市");
            this.Phone = Phone.Trim();
            base.CheckRule();
        }
    }
    /// <summary>
    /// 供应商
    /// </summary>
    public class Supplier : Businessman
    {
        /// <summary>
        /// 运营商号
        /// </summary>
        public string CarrierCode { get; set; }
        /// <summary>
        /// 本地费率
        /// </summary>
        public decimal SupRate { get; set; }
        /// <summary>
        /// 异地费率
        /// </summary>
        public decimal SupRemoteRate { get; set; }
        /// <summary>
        /// 正常工作日 业务处理时间
        /// </summary>
        public WorkBusinessman SupNormalWork { get; set; }
        /// <summary>
        /// 休息日 业务处理时间
        /// </summary>
        public WorkBusinessman SupRestWork { get; set; }
        /// <summary>
        /// 本地政策开关 true开 false关
        /// </summary>
        public bool SupLocalPolicySwitch { get; set; }
        /// <summary>
        /// 异地政策开关 true开 false关
        /// </summary>
        public bool SupRemotePolicySwitch { get; set; }
        /// <summary>
        /// PID配置 
        /// </summary>
        public virtual ICollection<PID> SupPids { get; set; }
        /// <summary>
        /// 航空公司使用Office的设置
        /// </summary>
        public virtual ICollection<CarrierSetting> CarrierSettings { get; set; }


        /// <summary>
        /// 验证是否在工作日时间
        /// </summary>
        /// <returns></returns>
        public bool CheckNormalWork()
        {
            bool defaultCheck = false;
            var dayOfWeek = (int)DateTime.Now.DayOfWeek;
            string currentWeekDay = dayOfWeek == 0 ? "7" : dayOfWeek.ToString();
            if (this.SupNormalWork.WeekDay.Contains(currentWeekDay))
            {
                if (DateTime.Compare(DateTime.Now, DateTime.Parse(this.SupNormalWork.WorkOnLineTime)) > 0 && DateTime.Compare(DateTime.Now, DateTime.Parse(this.SupNormalWork.WorkUnLineTime)) < 0)
                    defaultCheck = true;
            }
            else
            {
                if (DateTime.Compare(DateTime.Now, DateTime.Parse(this.SupRestWork.WorkOnLineTime)) > 0 && DateTime.Compare(DateTime.Now, DateTime.Parse(this.SupRestWork.WorkUnLineTime)) < 0)
                    defaultCheck = true;
            }
            return defaultCheck;
        }

    }
    /// <summary>
    /// 运营商
    /// </summary>
    public class Carrier : Businessman
    {
        /// <summary>
        /// 标签
        /// </summary>
        public string Label { get; set; }
        /// <summary>
        /// 费率
        /// </summary>
        public decimal Rate { get; set; }
        /// <summary>
        /// 异地费率
        /// </summary>
        public decimal RemoteRate { get; set; }
        /// <summary>
        /// 正常工作日 业务处理时间
        /// </summary>
        public WorkBusinessman NormalWork { get; set; }
        /// <summary>
        /// 休息日 业务处理时间
        /// </summary>
        public WorkBusinessman RestWork { get; set; }
        /// <summary>
        /// 本地政策开关 true开 false关
        /// </summary>
        public bool LocalPolicySwitch { get; set; }
        /// <summary>
        /// 接口政策开关 true开 false关
        /// </summary>
        public bool InterfacePolicySwitch { get; set; }
        /// <summary>
        /// 对外异地政策开关 true开 false关
        /// </summary>
        public bool ForeignRemotePolicySwich { get; set; }
        /// <summary>
        /// 采购异地政策开关 true开 false关
        /// </summary>
        public bool BuyerRemotoPolicySwich { get; set; }
        /// <summary>
        /// 显示本地客户中心开关 true开 false关
        /// </summary>
        public bool ShowLocalCSCSwich { get; set; }
        /// <summary>
        /// 运营扣点组ID
        /// </summary>
        public Guid? PointGroupID { get; set; }
        /// <summary>
        /// PID配置
        /// </summary>
        public virtual ICollection<PID> Pids { get; set; }

        /// <summary>
        /// 航空公司使用Office的设置
        /// </summary>
        public virtual ICollection<CarrierSetting> CarrierSettings { get; set; }
        /// <summary>
        /// 默认政策
        /// </summary>
        public virtual ICollection<DefaultPolicy> DefaultPolicys { get; set; }
        public bool CheckNormalWork()
        {
            bool defaultCheck = false;
            var dayOfWeek = (int)DateTime.Now.DayOfWeek;
            string currentWeekDay = dayOfWeek == 0 ? "7" : dayOfWeek.ToString();
            if (this.NormalWork.WeekDay.Contains(currentWeekDay))
            {
                if (DateTime.Compare(DateTime.Now, DateTime.Parse(this.NormalWork.WorkOnLineTime)) > 0 && DateTime.Compare(DateTime.Now, DateTime.Parse(this.NormalWork.WorkUnLineTime)) < 0)
                    defaultCheck = true;
            }
            else
            {
                if (DateTime.Compare(DateTime.Now, DateTime.Parse(this.RestWork.WorkOnLineTime)) > 0 && DateTime.Compare(DateTime.Now, DateTime.Parse(this.RestWork.WorkUnLineTime)) < 0)
                    defaultCheck = true;
            }
            return defaultCheck;
        }
    }
    public class WorkBusinessman : ValueObjectBase
    {
        /// <summary>
        /// 星期
        /// </summary>
        public string WeekDay { get; set; }
        /// <summary>
        /// 上班时间
        /// </summary>
        public string WorkOnLineTime { get; set; }
        /// <summary>
        /// 下班时间
        /// </summary>
        public string WorkUnLineTime { get; set; }
        /// <summary>
        /// 业务处理上线时间
        /// </summary>
        public string ServiceOnLineTime { get; set; }
        /// <summary>
        /// 业务处理下线世间
        /// </summary>
        public string ServiceUnLineTime { get; set; }
    }
    /// <summary>
    /// PID信息
    /// </summary>
    public class PID : EntityBase
    {
        public int ID { get; set; }
        /// <summary>
        /// IP地址
        /// </summary>
        public string IP { get; set; }
        /// <summary>
        /// 端口号
        /// </summary>
        public int Port { get; set; }
        /// <summary>
        /// Office号
        /// </summary>
        public string Office { get; set; }

        protected override string GetIdentity()
        {
            return ID.ToString();
        }
    }
    /// <summary>
    /// 航空公司使用Office
    /// </summary>
    public class CarrierSetting : EntityBase
    {
        public int ID { get; set; }
        /// <summary>
        /// 外健
        /// </summary>
        public string CarrierCode { get; set; }
        /// <summary>
        /// 航空公司二字码
        /// </summary>
        public string CarrayCode
        {
            get;
            set;
        }
        /// <summary>
        /// 预定编码Office
        /// </summary>
        public string YDOffice
        {
            get;
            set;
        }
        /// <summary>
        /// 出票Office
        /// </summary>
        public string CPOffice
        {
            get;
            set;
        }
        /// <summary>
        /// 打票机号
        /// </summary>
        public string PrintNo
        {
            get;
            set;
        }
        protected override string GetIdentity()
        {
            return ID.ToString();
        }
    }
    public class DefaultPolicy : EntityBase
    {
        public int ID { get; set; }
        /// <summary>
        /// 航空公司二字码
        /// </summary>
        public string CarrayCode { get; set; }
        /// <summary>
        /// 默认政策
        /// </summary>
        public decimal DefaultPolicyPoint { get; set; }
        /// <summary>
        /// 儿童政策
        /// </summary>
        public decimal ChildrenPolicyPoint { get; set; }
        /// <summary>
        /// 出票类型[B2B,BSP]
        /// </summary>
        public string IssueTicketType { get; set; }
        /// <summary>
        /// Office号
        /// </summary>
        public string Office { get; set; }
        /// <summary>
        /// 出票方式
        /// </summary>
        public EnumIssueTicketWay IssueTicketWay { get; set; }
        protected override string GetIdentity()
        {
            return ID.ToString();
        }
    }


}
