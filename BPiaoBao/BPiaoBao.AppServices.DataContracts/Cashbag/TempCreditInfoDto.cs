namespace BPiaoBao.AppServices.DataContracts.Cashbag
{
    /// <summary>
    ///  临时额度实体接收对象
    /// </summary>
    public class TempCreditInfoDto
    {
        /// <summary>
        /// 最近未逾期周期（天）
        /// </summary>
        public int Day { get; set; }
        /// <summary>
        /// 周期内申请次数
        /// </summary>
        public int Number { get; set; }
    }
}
