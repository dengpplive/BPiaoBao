using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using AutoMapper;
using BPiaoBao.AppServices.Contracts.DomesticTicket;
using BPiaoBao.AppServices.DataContracts;
using BPiaoBao.AppServices.DataContracts.DomesticTicket;
using BPiaoBao.Common;
using BPiaoBao.Common.Enums;
using BPiaoBao.DomesticTicket.Domain.Models.Orders;
using BPiaoBao.DomesticTicket.Domain.Models.TravelPaper;
using BPiaoBao.DomesticTicket.Domain.Services;
using BPiaoBao.SystemSetting.Domain.Models.Businessmen;
using BPiaoBao.SystemSetting.Domain.Services.Auth;
using JoveZhao.Framework;
using JoveZhao.Framework.DDD;
using StructureMap;

namespace BPiaoBao.AppServices.DomesticTicket
{
    public class TravelPaperService : ITravelPaperService
    {
        IUnitOfWork unitOfWork = ObjectFactory.GetNamedInstance<IUnitOfWork>(EnumModule.DomesticTicket.ToString());
        IUnitOfWorkRepository unitOfWorkRepository = ObjectFactory.GetNamedInstance<IUnitOfWorkRepository>(EnumModule.DomesticTicket.ToString());

        ITravelGrantRecordRepository m_travelGrantRecordRepository;
        ITravelPaperRepository m_travelPaperRepository;
        IBusinessmanRepository m_businessmanRepository;
        IOrderRepository orderRepository;
        IAfterSaleOrderRepository AfterSaleorderRepository;
        CurrentUserInfo currentUser;
        public TravelPaperService(ITravelGrantRecordRepository travelGrantRecordRepository, ITravelPaperRepository travelPaperRepository, IOrderRepository orderRepository, IAfterSaleOrderRepository afterSaleorderRepository, IBusinessmanRepository businessmanRepository)
        {
            this.m_travelGrantRecordRepository = travelGrantRecordRepository;
            this.m_travelPaperRepository = travelPaperRepository;
            this.m_businessmanRepository = businessmanRepository;
            this.orderRepository = orderRepository;
            this.AfterSaleorderRepository = afterSaleorderRepository;
            currentUser = AuthManager.GetCurrentUser();
        }
        /// <summary>
        /// 分配行程单
        /// </summary>
        /// <param name="travelPaper"></param>
        [ExtOperationInterceptor("分配行程单")]
        public int AddTravelPaper(string buyerBusinessman,
            string startTripNumber, string endTripNumber, string useOffice, string iataCode,
            string ticketCompanyName, string tripRemark)
        {
            string BusinessmanCode = currentUser.Code;
            string BusinessmanName = currentUser.BusinessmanName;
            int dataCount = -1;
            string strMsg = string.Empty;
            string useBusinessmanCode = string.Empty;
            string useBusinessmanName = string.Empty;
            if (string.IsNullOrEmpty(buyerBusinessman))
            {
                strMsg = "分配商户名称或者商户号不能为空！";
            }
            else if (string.IsNullOrEmpty(BusinessmanCode))
            {
                strMsg = "供应商户号不能为空！";
            }
            else if (string.IsNullOrEmpty(BusinessmanName))
            {
                strMsg = "供应商户号不能为空！";
            }
            else if (string.IsNullOrEmpty(startTripNumber)
               || string.IsNullOrEmpty(endTripNumber)
                || startTripNumber.Trim().Length != 10
                || endTripNumber.Trim().Length != 10
               )
            {
                strMsg = "行程单号段数据不完整！";
            }
            else if (string.IsNullOrEmpty(useOffice))
            {
                strMsg = "行程单号终端号不能为空！";
            }
            else if (string.IsNullOrEmpty(iataCode))
            {
                strMsg = "航协号不能为空！";
            }
            else if (string.IsNullOrEmpty(ticketCompanyName))
            {
                strMsg = "填开单位不能为空！";
            }
            else
            {
                if (!string.IsNullOrEmpty(startTripNumber) && !string.IsNullOrEmpty(endTripNumber))
                {
                    int start = int.Parse(startTripNumber.Substring(6, 4));
                    int end = int.Parse(endTripNumber.Substring(6, 4));
                    if (start > end)
                    {
                        strMsg = "行程单号段范围有误！";
                    }
                }
            }
            //判断商户号是否存在
            Businessman businessman = this.m_businessmanRepository.FindAll(p => p.Code == buyerBusinessman || p.Name == buyerBusinessman).FirstOrDefault();
            if (businessman == null)
            {
                strMsg = "分配商户号或者商户名称不存在！";
            }
            else
            {
                useBusinessmanCode = businessman.Code;
                useBusinessmanName = businessman.Name;
            }
            //useBusinessmanCode = "caigou";//businessman.Code;
            //useBusinessmanName = "采购";//businessman.Name;
            if (string.IsNullOrEmpty(useBusinessmanCode))
            {
                strMsg = "分配商户号或者商户名称不存在！";
            }
            if (string.IsNullOrEmpty(strMsg))
            {
                string StartCode = startTripNumber.Substring(0, 6);
                string start = startTripNumber.Substring(6, 4);
                string end = endTripNumber.Substring(6, 4);
                tripRemark = tripRemark.Replace("'", "");
                //分配行程单
                dataCount = unitOfWorkRepository.ExecuteCommand(
                    "EXEC [dbo].[TravelPaperImport] @p0,@p1,@p2,@p3,@p4,@p5,@p6,@p7,@p8,@p9,@p10,@p11,@p12 ",
                    "0",
                    StartCode,
                    start,
                    end,
                    useOffice,
                    iataCode,
                    ticketCompanyName,
                    BusinessmanCode,
                    BusinessmanName,
                    useBusinessmanCode,
                    useBusinessmanName,
                    tripRemark,
                    ""
                    );
                if (dataCount > 0)
                {
                    TravelGrantRecord travelGrantRecord = new TravelGrantRecord();
                    travelGrantRecord.BusinessmanCode = BusinessmanCode;
                    travelGrantRecord.BusinessmanName = BusinessmanName;
                    travelGrantRecord.UseBusinessmanCode = useBusinessmanCode;
                    travelGrantRecord.UseBusinessmanName = useBusinessmanName;
                    travelGrantRecord.GrantTime = System.DateTime.Now;
                    travelGrantRecord.Office = useOffice;
                    travelGrantRecord.TripCount = int.Parse(end) - int.Parse(start) + 1;
                    travelGrantRecord.TripScope = startTripNumber + "-" + endTripNumber;
                    travelGrantRecord.TripRemark = tripRemark;
                    //添加发放记录
                    unitOfWorkRepository.PersistCreationOf(travelGrantRecord);
                    unitOfWork.Commit();
                }
            }
            if (!string.IsNullOrEmpty(strMsg))
            {
                throw new OrderCommException(strMsg);
            }
            return dataCount;
        }
        /// <summary>
        /// 查询可用行程单号
        /// </summary>
        /// <param name="buyBusinessmanCode"></param>
        /// <returns></returns>
        [ExtOperationInterceptor("查询可用行程单号")]
        public DataPack<TravelPaperDto> FindUseTravelPaperDto(string buyBusinessmanCode)
        {
            var result = m_travelPaperRepository.FindAll(p => p.UseBusinessmanCode.Trim() == buyBusinessmanCode.Trim());
            result = result.Where(p => p.TripStatus == EnumTripStatus.NoUsed || p.TripStatus == EnumTripStatus.BlankRecoveryUsed);
            List<TravelPaper> TravelPaperList = result.ToList();
            DataPack<TravelPaperDto> data = new DataPack<TravelPaperDto>();
            data.TotalCount = TravelPaperList.Count();
            data.List = Mapper.Map<List<TravelPaper>, List<TravelPaperDto>>(TravelPaperList);
            return data;
        }

        /// <summary>
        /// 查询行程单详情
        /// </summary>
        /// <returns></returns>
        [ExtOperationInterceptor("查询行程单详情")]
        public DataPack<TravelPaperDto> FindTravelPaper(string buyBusinessmanCode, string buyBusinessmanName, string useOffice,
            string startTripNumber, string endTripNumber,
            string startTicketNumber, string endTicketNumber,
            DateTime? startCreateTime, DateTime? endCreateTime,
            DateTime? startVoidTime, DateTime? endVoidTime,
            DateTime? startGrantTime, DateTime? endGrantTime,
            DateTime? startRecoveryTime, DateTime? endRecoveryTime,
            string PageSource,
            int? tripStatus, int pageIndex, int pageSize, bool isPager = true, int?[] tripStatuss = null, string OrderId = ""
            )
        {
            string BusinessmanCode = string.Empty;
            StringBuilder sbSqlWhere = new StringBuilder();
            if (tripStatuss == null)
            {
                BusinessmanCode = currentUser.Code;   //"111";//
                sbSqlWhere.AppendFormat(" and  BusinessmanCode='{0}' ", BusinessmanCode);
                //采购商户号
                if (!string.IsNullOrEmpty(buyBusinessmanCode))
                {
                    sbSqlWhere.AppendFormat(" and UseBusinessmanCode='{0}' ", buyBusinessmanCode.Trim());
                }
            }
            else
            {
                //采购商户号
                if (!string.IsNullOrEmpty(currentUser.Code))
                {
                    sbSqlWhere.AppendFormat(" and UseBusinessmanCode='{0}' ", currentUser.Code.Trim());
                }
            }
            //采购商户名
            if (!string.IsNullOrEmpty(buyBusinessmanName))
            {
                sbSqlWhere.AppendFormat(" and UseBusinessmanName='{0}' ", buyBusinessmanName.Trim());
            }
            //office
            if (!string.IsNullOrEmpty(useOffice))
            {
                sbSqlWhere.AppendFormat(" and UseOffice='{0}' ", useOffice.Trim());
            }
            ////票号段
            if (!string.IsNullOrEmpty(startTicketNumber) && !string.IsNullOrEmpty(endTicketNumber))
            {
                startTicketNumber = startTicketNumber.Replace("-", "").Replace("\'", "").Trim();
                endTicketNumber = endTicketNumber.Replace("-", "").Replace("\'", "").Trim();
                sbSqlWhere.AppendFormat(" and TicketNumber between '{0}' and '{1}' ", startTicketNumber.Replace("'", ""), endTicketNumber.Replace("'", ""));
            }
            if (!string.IsNullOrEmpty(startTripNumber) && !string.IsNullOrEmpty(endTripNumber))
            {
                //行程单号段                
                sbSqlWhere.AppendFormat(" and TripNumber between '{0}' and '{1}' ", startTripNumber.Replace("'", "").Trim(), endTripNumber.Replace("'", "").Trim());
            }
            //行程单状态
            if (PageSource == "black")//来源空白行程单
            {
                if (tripStatus != null && tripStatus.HasValue && tripStatus.Value != -1)
                {
                    sbSqlWhere.AppendFormat(" and TripStatus ={0} ", tripStatus.Value);
                }
                else
                {
                    sbSqlWhere.AppendFormat(" and TripStatus in(3,4) ");
                }
            }
            else
            {
                if (tripStatus != null && tripStatus.HasValue && tripStatus.Value != -1)
                {
                    sbSqlWhere.AppendFormat(" and TripStatus ={0} ", tripStatus.Value);
                }
            }
            if (startCreateTime != null && startCreateTime.HasValue)
            {
                sbSqlWhere.AppendFormat(" and PrintTime >='{0}' ", startCreateTime.Value.ToString("yyyy-MM-dd"));
            }
            if (endCreateTime != null && endCreateTime.HasValue)
            {
                sbSqlWhere.AppendFormat(" and PrintTime <='{0} 23:59:59' ", endCreateTime.Value.ToString("yyyy-MM-dd"));
            }
            if (startVoidTime != null && startVoidTime.HasValue)
            {
                sbSqlWhere.AppendFormat(" and InvalidTime >='{0}' ", startVoidTime.Value.ToString("yyyy-MM-dd"));
            }
            if (endVoidTime != null && endVoidTime.HasValue)
            {
                sbSqlWhere.AppendFormat(" and InvalidTime <='{0} 23:59:59' ", endVoidTime.Value.ToString("yyyy-MM-dd"));
            }
            if (startGrantTime != null && startGrantTime.HasValue)
            {
                sbSqlWhere.AppendFormat(" and GrantTime >='{0}' ", startGrantTime.Value.ToString("yyyy-MM-dd"));
            }
            if (endGrantTime != null && endGrantTime.HasValue)
            {
                sbSqlWhere.AppendFormat(" and GrantTime <='{0} 23:59:59' ", endGrantTime.Value.ToString("yyyy-MM-dd"));
            }
            if (startRecoveryTime != null && startRecoveryTime.HasValue)
            {
                sbSqlWhere.AppendFormat(" and BlankRecoveryTime >='{0}' ", startRecoveryTime.Value.ToString("yyyy-MM-dd"));
            }
            if (endRecoveryTime != null && endRecoveryTime.HasValue)
            {
                sbSqlWhere.AppendFormat(" and BlankRecoveryTime <='{0} 23:59:59' ", endRecoveryTime.Value.ToString("yyyy-MM-dd"));
            }
            if (tripStatuss != null && tripStatuss.Count() > 0)//行程单号状态
            {
                sbSqlWhere.AppendFormat(" and TripStatus in({0}) ", string.Join(",", tripStatuss.ToArray()));
            }
            if (!string.IsNullOrEmpty(OrderId))//订单号
            {
                sbSqlWhere.AppendFormat(" and OrderId = '{0}' ", OrderId);
            }
            string sql = "";
            if (isPager)
            {
                sql = "SELECT TOP " + pageSize + " * " +
                 "FROM TravelPaper " +
                 " WHERE " +
                              "(ID >(SELECT isnull(MAX(ID),0) " +
                              " FROM (SELECT TOP " + ((pageIndex - 1) * pageSize) + " ID " +
                              " FROM TravelPaper  where 1=1 " + sbSqlWhere.ToString() + " " +
                              " ORDER BY ID) AS T)) " + sbSqlWhere.ToString() + " " +

                 " ORDER BY ID ";
            }
            else
            {
                sql = "select * from TravelPaper where 1=1 " + sbSqlWhere.ToString() + " Order By ID";
            }
            DataPack<TravelPaperDto> data = new DataPack<TravelPaperDto>();
            List<TravelPaper> TravelPaperList = new List<TravelPaper>();
            DbRawSqlQuery dbRawSqlQuery = unitOfWorkRepository.SqlQuery(sql, typeof(TravelPaper));
            TravelPaperList = dbRawSqlQuery.Cast<TravelPaper>().ToList();

            sql = "select count(*) from TravelPaper where 1=1 " + sbSqlWhere.ToString();
            dbRawSqlQuery = unitOfWorkRepository.SqlQuery(sql, typeof(int));
            data.TotalCount = dbRawSqlQuery.Cast<int>().FirstOrDefault();
            data.List = Mapper.Map<List<TravelPaper>, List<TravelPaperDto>>(TravelPaperList);
            return data;
        }


        [ExtOperationInterceptor("查询行程单发放记录")]
        public DataPack<TravelGrantRecordDto> FindTravelRecord(string useBusinessmanCode, string office, DateTime? startTime, DateTime? endTime, int pageIndex, int pageSize, bool isPager = true)
        {
            string BusinessmanCode = currentUser.Code;
            var result = m_travelGrantRecordRepository.FindAll(p => p.BusinessmanCode.Trim() == BusinessmanCode.Trim());
            if (startTime != null && startTime.HasValue)
            {
                result = result.Where(p => p.GrantTime >= startTime.Value);
            }
            if (endTime != null && endTime.HasValue)
            {
                DateTime entime = DateTime.Parse(endTime.Value.ToString("yyyy-MM-dd") + " 23:59:59");
                result = result.Where(p => p.GrantTime <= entime);
            }
            if (!string.IsNullOrEmpty(useBusinessmanCode))
            {
                result = result.Where(p => (p.UseBusinessmanCode.Trim() == useBusinessmanCode.Trim()
                       || p.UseBusinessmanName.Trim() == useBusinessmanCode.Trim()));
            }
            if (!string.IsNullOrEmpty(office))
            {
                result = result.Where(p => p.Office.ToLower().Trim() == office.ToLower().Trim());
            }
            List<TravelGrantRecord> TravelGrantRecordList = new List<TravelGrantRecord>();
            DataPack<TravelGrantRecordDto> data = new DataPack<TravelGrantRecordDto>();
            if (isPager)
            {
                TravelGrantRecordList = result.OrderByDescending(p => p.ID).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            }
            else
            {
                TravelGrantRecordList = result.ToList();
            }
            List<TravelGrantRecordDto> travelGrantRecordDtoList = Mapper.Map<List<TravelGrantRecord>, List<TravelGrantRecordDto>>(TravelGrantRecordList);
            //合计
            if (travelGrantRecordDtoList.Count > 0)
            {
                travelGrantRecordDtoList.Add(new TravelGrantRecordDto()
                {
                    TripCount = travelGrantRecordDtoList.Sum(p => p.TripCount),
                    UseBusinessmanName = "合计"
                });
            }
            data.TotalCount = result.Count() + 1;
            data.List = travelGrantRecordDtoList;
            return data;
        }
        /// <summary>
        /// 查询行程单统计数据
        /// </summary>
        /// <returns></returns>
        [ExtOperationInterceptor("查询行程单统计数据")]
        public TravelPaperStaticsDto FindTravelPaperStatistics(string buyBusinessmanCode, string buyBusinessmanName)
        {
            string BusinessmanCode = currentUser.Code;
            TravelPaperStaticsDto travelPaperStaticsDto = new TravelPaperStaticsDto();
            var result = m_travelPaperRepository.FindAll(p => p.BusinessmanCode.Trim() != "" && p.BusinessmanCode.Trim() == BusinessmanCode.Trim());
            result.GroupBy(p => p.UseBusinessmanCode.Trim()).Each(p =>
            {
                TravelPaper travelPaper = null;
                if (string.IsNullOrEmpty(buyBusinessmanCode) && string.IsNullOrEmpty(buyBusinessmanName))
                {
                    travelPaper = result.FirstOrDefault(p1 => p1.UseBusinessmanCode.Trim() == p.Key.Trim());
                }
                else
                {
                    if (!string.IsNullOrEmpty(buyBusinessmanCode))
                    {
                        result = result.Where(p1 => p1.UseBusinessmanCode.Trim() == buyBusinessmanCode.Trim());
                    }
                    if (!string.IsNullOrEmpty(buyBusinessmanName))
                    {
                        result = result.Where(p1 => p1.UseBusinessmanName.Trim() == buyBusinessmanName.Trim());
                    }
                    travelPaper = result.FirstOrDefault(p1 => p1.UseBusinessmanCode.Trim() == p.Key.Trim());
                }
                if (travelPaper != null && !string.IsNullOrEmpty(travelPaper.UseBusinessmanCode))
                {
                    travelPaperStaticsDto.ItemStaticsList.Add(
                     new TravelPaperItem()
                     {
                         BusinessmanCode = travelPaper.BusinessmanCode,
                         BusinessmanName = travelPaper.BusinessmanName,
                         UseBusinessmanCode = travelPaper.UseBusinessmanCode,
                         UseBusinessmanName = travelPaper.UseBusinessmanName,

                         TotalCount = result.Where(p2 => p2.UseBusinessmanCode == p.Key && (int)p2.TripStatus != 3).Count(),
                         TotalNoUse = result.Where(p2 => p2.UseBusinessmanCode == p.Key && ((int)p2.TripStatus == 0)).Count(),
                         TotalUse = result.Where(p2 => p2.UseBusinessmanCode == p.Key && ((int)p2.TripStatus == 1)).Count(),
                         TotalVoid = result.Where(p2 => p2.UseBusinessmanCode == p.Key && ((int)p2.TripStatus == 2)).Count(),
                         TotalBlankRecovery = result.Where(p2 => p2.UseBusinessmanCode == p.Key && (int)p2.TripStatus == 3).Count(),
                         TotalValidateUse = result.Where(p2 => p2.UseBusinessmanCode == p.Key && (int)p2.TripStatus == 4).Count() + result.Where(p2 => p2.UseBusinessmanCode == p.Key && ((int)p2.TripStatus == 0)).Count()
                     });
                }
            });
            travelPaperStaticsDto.Total.UseBusinessmanName = "合计";
            travelPaperStaticsDto.Total.TotalCount = travelPaperStaticsDto.ItemStaticsList.Sum(p => p.TotalCount);
            travelPaperStaticsDto.Total.TotalUse = travelPaperStaticsDto.ItemStaticsList.Sum(p => p.TotalUse);
            travelPaperStaticsDto.Total.TotalNoUse = travelPaperStaticsDto.ItemStaticsList.Sum(p => p.TotalNoUse);
            travelPaperStaticsDto.Total.TotalBlankRecovery = travelPaperStaticsDto.ItemStaticsList.Sum(p => p.TotalBlankRecovery);
            travelPaperStaticsDto.Total.TotalVoid = travelPaperStaticsDto.ItemStaticsList.Sum(p => p.TotalVoid);
            travelPaperStaticsDto.Total.TotalValidateUse = travelPaperStaticsDto.ItemStaticsList.Sum(p => p.TotalValidateUse);
            return travelPaperStaticsDto;
        }


        /// <summary>
        /// 发放空白行程单
        /// </summary>
        /// <param name="travelPaper"></param>
        [ExtOperationInterceptor("发放空白行程单")]
        public int GrantBlankRecoveryTravelPaper(
            string useBusinessman, string useOffice, string iataCode,
            string ticketCompanyName, string TripRemark, List<int> selectIds)
        {
            int dataCount = -1;
            string strMsg = string.Empty;
            string BusinessmanCode = currentUser.Code;
            string BusinessmanName = currentUser.BusinessmanName;
            string useBusinessmanCode = string.Empty;
            string useBusinessmanName = string.Empty;
            var result = m_travelPaperRepository.FindAll(p => p.BusinessmanCode.Trim() == BusinessmanCode.Trim());

            Businessman businessman = this.m_businessmanRepository.FindAll(p => p.Code.Trim() == useBusinessman.Trim() || p.Name.Trim() == useBusinessman.Trim()).FirstOrDefault();
            if (businessman == null)
            {
                strMsg = "分配采购商户号或者采购商户名称不存在！";
            }
            else
            {
                useBusinessmanCode = businessman.Code;
                useBusinessmanName = businessman.Name;
            }
            //useBusinessmanCode = "caigou";//businessman.Code;
            //useBusinessmanName = "采购";//businessman.Name;
            if (string.IsNullOrEmpty(strMsg))
            {
                List<int> Ids = new List<int>();
                List<string> tripNumber = new List<string>();
                result.Where(p => selectIds.Contains(p.ID)).Each(p =>
                {
                    Ids.Add(p.ID);
                    tripNumber.Add(p.TripNumber);
                });

                //发放空白行程单
                dataCount = unitOfWorkRepository.ExecuteCommand(
                    "EXEC [dbo].[TravelPaperImport] @p0,@p1,@p2,@p3,@p4,@p5,@p6,@p7,@p8,@p9,@p10,@p11,@p12 ",
                     "2",
                     "",
                     "0",
                     "0",
                     useOffice,
                     iataCode,
                     ticketCompanyName,
                     BusinessmanCode,
                     BusinessmanName,
                     useBusinessmanCode,
                     useBusinessmanName,
                     TripRemark,
                      string.Join(",", Ids.ToArray())
                     );
                if (dataCount <= 0)
                {
                    strMsg = "发放空白行程单失败！";
                }
                else
                {
                    if (dataCount > 0)
                    {
                        TravelGrantRecord travelGrantRecord = new TravelGrantRecord();
                        travelGrantRecord.BusinessmanCode = BusinessmanCode;
                        travelGrantRecord.BusinessmanName = BusinessmanName;
                        travelGrantRecord.UseBusinessmanCode = useBusinessmanCode;
                        travelGrantRecord.UseBusinessmanName = useBusinessmanName;
                        travelGrantRecord.GrantTime = System.DateTime.Now;
                        travelGrantRecord.Office = useOffice;
                        travelGrantRecord.TripCount = tripNumber.Count();
                        travelGrantRecord.TripScope = string.Join(",", tripNumber.ToArray());
                        travelGrantRecord.TripRemark = TripRemark;
                        //添加发放记录
                        unitOfWorkRepository.PersistCreationOf(travelGrantRecord);
                        unitOfWork.Commit();
                    }
                }
            }
            if (!string.IsNullOrEmpty(strMsg))
            {
                throw new OrderCommException(strMsg);
            }
            return dataCount;
        }





        /// <summary>
        /// 批量修改Office
        /// </summary>
        /// <param name="useOffice"></param>
        /// <param name="BusinessmanCode"></param>
        /// <param name="SelectIds"></param>
        [ExtOperationInterceptor("批量修改Office")]
        public int UpdateOffice(string useOffice, List<int> selectIds)
        {
            int dataCount = -1;
            string BusinessmanCode = currentUser.Code;
            try
            {
                List<TravelPaper> travelPaperList = m_travelPaperRepository.FindAll(p => p.BusinessmanCode == BusinessmanCode && selectIds.Contains(p.ID)).ToList();
                if (travelPaperList != null && travelPaperList.Count > 0)
                {
                    for (int i = 0; i < travelPaperList.Count; i++)
                    {
                        travelPaperList[i].WriteLog(new TravelPaperLog()
                        {
                            OperationContent = "原Office：" + travelPaperList[i].UseOffice + " 新Office：" + useOffice,
                            OperationType = "修改Office",
                            OperationDatetime = System.DateTime.Now,
                            OperationPerson = currentUser.OperatorAccount
                        });
                        travelPaperList[i].UseOffice = useOffice;
                        unitOfWorkRepository.PersistUpdateOf(travelPaperList[i]);
                    }
                    unitOfWork.Commit();
                    dataCount = travelPaperList.Count;
                }
                else
                {
                    throw new OrderCommException("批量修改Office失败！");
                }
            }
            catch (Exception ex)
            {
                throw new OrderCommException(ex.Message);
            }
            return dataCount;
        }

        /// <summary>
        ///  回收空白行程单
        /// </summary>
        /// <param name="travelPaper"></param>
        [ExtOperationInterceptor("回收空白行程单")]
        public int RecoveryBlackTravelPaper(List<int> travelIdList)
        {
            int dataCount = -1;
            string BusinessmanCode = currentUser.Code;
            try
            {
                List<TravelPaper> travelPaperList = this.m_travelPaperRepository.FindAll(p =>
                    p.BusinessmanCode == BusinessmanCode
                    && travelIdList.Contains(p.ID)
                    && (p.TripStatus == EnumTripStatus.NoUsed || p.TripStatus == EnumTripStatus.BlankRecoveryUsed)
                    ).ToList();
                if (travelPaperList != null && travelPaperList.Count > 0)
                {
                    for (int i = 0; i < travelPaperList.Count; i++)
                    {
                        travelPaperList[i].WriteLog(new TravelPaperLog()
                        {
                            OperationContent = "回收空白行程单",
                            OperationType = "空白回收",
                            OperationDatetime = System.DateTime.Now,
                            OperationPerson = currentUser.OperatorAccount
                        });
                        travelPaperList[i].TripStatus = EnumTripStatus.BlankRecoveryNoUsed;
                        travelPaperList[i].PrintTime = DateTime.Parse("1900-01-01");
                        travelPaperList[i].InvalidTime = DateTime.Parse("1900-01-01");
                        travelPaperList[i].BlankRecoveryTime = System.DateTime.Now;
                        travelPaperList[i].UseBusinessmanCode = "";
                        travelPaperList[i].UseBusinessmanName = "";
                        travelPaperList[i].UseOffice = "";
                        unitOfWorkRepository.PersistUpdateOf(travelPaperList[i]);
                    }
                    unitOfWork.Commit();
                    dataCount = travelPaperList.Count;
                }
                else
                {
                    throw new OrderCommException("请选择空白回收的行程单！");
                }
            }
            catch (Exception ex)
            {
                throw new OrderCommException(ex.Message);
            }
            return dataCount;
        }
        /// <summary>
        ///  回收作废行程单
        /// </summary>
        /// <param name="travelPaper"></param>
        [ExtOperationInterceptor("回收作废行程单")]
        public int RecoveryVoidTravelPaper(List<int> travelIdList)
        {
            int dataCount = -1;
            string BusinessmanCode = currentUser.Code;
            try
            {
                List<TravelPaper> travelPaperList = this.m_travelPaperRepository.FindAll(p => p.BusinessmanCode == BusinessmanCode && travelIdList.Contains(p.ID)).ToList();
                if (travelPaperList != null && travelPaperList.Count > 0)
                {
                    for (int i = 0; i < travelPaperList.Count; i++)
                    {
                        travelPaperList[i].WriteLog(new TravelPaperLog()
                        {
                            OperationContent = "回收作废行程单",
                            OperationType = "回收作废",
                            OperationDatetime = System.DateTime.Now,
                            OperationPerson = currentUser.OperatorAccount
                        });
                        travelPaperList[i].TripStatus = EnumTripStatus.VoidRecoveryUsed;
                        travelPaperList[i].BlankRecoveryTime = System.DateTime.Now;
                        unitOfWorkRepository.PersistUpdateOf(travelPaperList[i]);
                    }
                    unitOfWork.Commit();
                    dataCount = travelPaperList.Count;
                }
                else
                {
                    throw new OrderCommException("请选择回收作废的行程单！");
                }
            }
            catch (Exception ex)
            {
                throw new OrderCommException(ex.Message);
            }
            return dataCount;
        }




        /// <summary>
        /// 查询行程单号详情
        /// </summary>
        /// <param name="TripNumber"></param>
        /// <returns></returns>
        [ExtOperationInterceptor("查询行程单号详情")]
        public TravelPaperDto QueryTripNumberDetail(string tripNumber)
        {
            TravelPaperDto TravelPaperDto = null;
            var result = m_travelPaperRepository.FindAll(p => p.TripNumber == tripNumber);
            TravelPaper travelPaper = result.FirstOrDefault();
            if (travelPaper != null)
            {
                TravelPaperDto = Mapper.Map<TravelPaper, TravelPaperDto>(travelPaper);
                TravelPaperDto.TravelPaperLogDtos = Mapper.Map<List<TravelPaperLog>, List<TravelPaperLogDto>>(travelPaper.TravelPaperLogs);
            }
            else
            {
                throw new OrderCommException("未能查到该【" + tripNumber + "】行程单号的信息");
            }
            return TravelPaperDto;
        }

        /// <summary>
        /// 批量修改行程单数据
        /// </summary>
        /// <param name="tripNumber"></param>
        /// <param name="ticketNumber"></param>
        /// <param name="tripStatus"></param>
        /// <param name="useOffice"></param>
        /// <param name="iataCode"></param>
        /// <param name="ticketCompanyName"></param>
        /// <returns></returns>
        [ExtOperationInterceptor("批量修改行程单数据")]
        public bool UpdateTripNumberInfo(List<string> tripNumberList, string ticketNumber, EnumTripStatus tripStatus, string useOffice, string iataCode, string ticketCompanyName)
        {
            bool IsUpdate = false;
            if (tripNumberList == null || tripNumberList.Count == 0)
                throw new OrderCommException("需要修改的行程单数据不能为空!");
            var result = m_travelPaperRepository.FindAll(p => tripNumberList.Contains(p.TripNumber)).ToList();
            if (result.Count > 0)
            {
                result.ForEach(p =>
                {
                    TravelPaper travelPaper = p;
                    if (travelPaper != null)
                    {
                        string tripNumber = p.TripNumber;
                        StringBuilder sbCon = new StringBuilder();
                        if (!string.IsNullOrEmpty(ticketNumber) && ticketNumber != travelPaper.TicketNumber)
                        {
                            travelPaper.TicketNumber = ticketNumber;
                            sbCon.AppendFormat("原票号:{0} 新票号:{1},", travelPaper.TicketNumber, tripNumber);
                        }
                        if (tripStatus != travelPaper.TripStatus)
                        {
                            travelPaper.TripStatus = tripStatus;
                            sbCon.AppendFormat("原状态:{0} 新状态:{1},", EnumItemManager.GetDesc(travelPaper.TripStatus), EnumItemManager.GetDesc(tripStatus));
                        }
                        if (!string.IsNullOrEmpty(useOffice) && useOffice != travelPaper.UseOffice)
                        {
                            travelPaper.UseOffice = useOffice;
                            sbCon.AppendFormat("原Office号:{0} 新Office号:{1},", travelPaper.UseOffice, useOffice);
                        }
                        if (!string.IsNullOrEmpty(iataCode) && iataCode != travelPaper.IataCode)
                        {
                            travelPaper.IataCode = iataCode;
                            sbCon.AppendFormat("原航协号:{0} 新航协号:{1},", travelPaper.IataCode, iataCode);
                        }
                        if (!string.IsNullOrEmpty(ticketCompanyName) && ticketCompanyName != travelPaper.TicketCompanyName)
                        {
                            travelPaper.TicketCompanyName = ticketCompanyName;
                            sbCon.AppendFormat("原填开单位:{0} 新填开单位:{1},", travelPaper.TicketCompanyName, ticketCompanyName);
                        }
                        travelPaper.WriteLog(new TravelPaperLog()
                        {
                            OperationType = "修改",
                            OperationPerson = currentUser.OperatorAccount,
                            OperationDatetime = System.DateTime.Now,
                            OperationContent = sbCon.ToString()
                        });
                        unitOfWorkRepository.PersistUpdateOf(travelPaper);
                    }
                });
                //提交
                unitOfWork.Commit();
                IsUpdate = true;
            }
            else
            {
                throw new OrderCommException("未能查到行程单数据！");
            }
            return IsUpdate;
        }



        /// <summary>
        /// 创建行程单
        /// </summary>
        /// <param name="req"></param>
        [ExtOperationInterceptor("创建行程单")]
        public TravelAppResponse CreateTrip(BPiaoBao.AppServices.DataContracts.DomesticTicket.TravelAppRequrst req)
        {
            TravelAppResponse response = new TravelAppResponse();
            try
            {
                FlightService flightDestineService = new FlightService(this.m_businessmanRepository, currentUser);
                response = flightDestineService.CreateTrip(req);
                if (response.IsSuc)
                {
                    if (response.PnrAnalysisTripNumber.Trim() != req.TripNumber.Trim())
                    {
                        response.IsSuc = false;
                    }
                    else
                    {
                        if (req.Flag == 0)//如果是正常订单
                        {
                            //更改状态
                            Order order = this.orderRepository.FindAll(p => p.OrderId == req.OrderId).FirstOrDefault();
                            TravelPaper travelPaper = this.m_travelPaperRepository.FindAll(p => p.TripNumber == req.TripNumber).FirstOrDefault();
                            if (order != null && travelPaper != null)
                            {
                                order.Passengers.Each(p =>
                                {
                                    if (p.Id == req.PassengerId)
                                    {
                                        p.PassengerTripStatus = EnumPassengerTripStatus.HasCreate;
                                        p.TravelNumber = req.TripNumber;
                                    }
                                });
                                travelPaper.OrderId = req.OrderId;
                                travelPaper.PassengerId = req.PassengerId;
                                travelPaper.TripStatus = EnumTripStatus.HasCreatedUsed;
                                travelPaper.PrintTime = System.DateTime.Now;
                                travelPaper.TicketNumber = req.TicketNumber;
                                string strLog = "行程单号:" + response.TripNumber + "票号:" + response.TicketNumber + " Office：" + response.CreateOffice + " " + response.ShowMsg;
                                travelPaper.WriteLog(new TravelPaperLog()
                                {
                                    OperationContent = strLog,
                                    OperationType = "创建行程单",
                                    OperationDatetime = System.DateTime.Now,
                                    OperationPerson = currentUser.OperatorAccount
                                });
                                order.OrderLogs.Add(new OrderLog()
                                {
                                    OperationContent = strLog,
                                    OperationDatetime = System.DateTime.Now,
                                    OperationPerson = currentUser.OperatorAccount,
                                    IsShowLog = true
                                });
                            }
                            unitOfWorkRepository.PersistUpdateOf(order);
                            unitOfWorkRepository.PersistUpdateOf(travelPaper);
                            unitOfWork.Commit();
                        }
                        else//如果是售后订单
                        {
                            //更改状态
                            int id = int.Parse(req.OrderId);
                            AfterSaleOrder order = this.AfterSaleorderRepository.FindAll(p => p.Id == id).FirstOrDefault();
                            TravelPaper travelPaper = this.m_travelPaperRepository.FindAll(p => p.TripNumber == req.TripNumber).FirstOrDefault();
                            if (order != null && travelPaper != null)
                            {
                                order.Passenger.Each(p =>
                                {
                                    if (p.Id == req.PassengerId)
                                    {
                                        p.PassengerTripStatus = EnumPassengerTripStatus.HasCreate;
                                        p.AfterSaleTravelNum = req.TripNumber;
                                        p.AfterSaleTravelTicketNum = req.TicketNumber;
                                    }
                                });
                                travelPaper.OrderId = req.OrderId;
                                travelPaper.PassengerId = req.PassengerId;
                                travelPaper.TripStatus = EnumTripStatus.HasCreatedUsed;
                                travelPaper.PrintTime = System.DateTime.Now;
                                travelPaper.TicketNumber = req.TicketNumber;
                                string strLog = "改签行程单号:" + response.TripNumber + "改签票号:" + response.TicketNumber + " 改签Office：" + response.CreateOffice + " " + response.ShowMsg;
                                travelPaper.WriteLog(new TravelPaperLog()
                                {
                                    OperationContent = strLog,
                                    OperationType = "创建改签行程单",
                                    OperationDatetime = System.DateTime.Now,
                                    OperationPerson = currentUser.OperatorAccount
                                });
                                order.Order.OrderLogs.Add(new OrderLog()
                                {
                                    OperationContent = strLog,
                                    OperationDatetime = System.DateTime.Now,
                                    OperationPerson = currentUser.OperatorAccount,
                                    IsShowLog = true
                                });
                            }
                            unitOfWorkRepository.PersistUpdateOf(order);
                            unitOfWorkRepository.PersistUpdateOf(travelPaper);
                            unitOfWork.Commit();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new CustomException(111, ex.Message);
            }
            return response;
        }

        /// <summary>
        /// 作废行程单
        /// </summary>
        /// <param name="req"></param>
        [ExtOperationInterceptor("作废行程单")]
        public TravelAppResponse VoidTrip(BPiaoBao.AppServices.DataContracts.DomesticTicket.TravelAppRequrst req)
        {
            TravelAppResponse response = new TravelAppResponse();
            try
            {
                FlightService flightDestineService = new FlightService(this.m_businessmanRepository, currentUser);
                response = flightDestineService.VoidTrip(req);
                if (response.IsSuc)
                {
                    if (response.PnrAnalysisTripNumber.Trim() != req.TripNumber.Trim())
                    {
                        response.IsSuc = false;
                    }
                    else
                    {
                        if (req.Flag == 0)
                        {
                            //更改状态
                            Order order = this.orderRepository.FindAll(p => p.OrderId == req.OrderId).FirstOrDefault();
                            TravelPaper travelPaper = this.m_travelPaperRepository.FindAll(p => p.TripNumber == req.TripNumber).FirstOrDefault();
                            if (order != null && travelPaper != null)
                            {
                                order.Passengers.Each(p =>
                                {
                                    if (p.Id == req.PassengerId)
                                    {
                                        p.PassengerTripStatus = EnumPassengerTripStatus.HasVoid;
                                    }
                                });
                                travelPaper.OrderId = req.OrderId;
                                travelPaper.PassengerId = req.PassengerId;
                                travelPaper.TripStatus = EnumTripStatus.HasObsoleteUsed;
                                travelPaper.InvalidTime = System.DateTime.Now;
                                string strLog = "行程单号:" + response.TripNumber + "票号:" + response.TicketNumber + " Office：" + response.CreateOffice + " " + response.ShowMsg;
                                travelPaper.WriteLog(new TravelPaperLog()
                                {
                                    OperationContent = strLog,
                                    OperationType = "作废行程单",
                                    OperationDatetime = System.DateTime.Now,
                                    OperationPerson = currentUser.OperatorAccount
                                });
                                order.OrderLogs.Add(new OrderLog()
                                {
                                    OperationContent = strLog,
                                    OperationDatetime = System.DateTime.Now,
                                    OperationPerson = currentUser.OperatorAccount,
                                    IsShowLog = true
                                });
                                unitOfWorkRepository.PersistUpdateOf(order);
                                unitOfWorkRepository.PersistUpdateOf(travelPaper);
                                unitOfWork.Commit();
                            }
                        }
                        else
                        {
                            //更改状态
                            int id = int.Parse(req.OrderId);
                            AfterSaleOrder order = this.AfterSaleorderRepository.FindAll(p => p.Id == id).FirstOrDefault();
                            TravelPaper travelPaper = this.m_travelPaperRepository.FindAll(p => p.TripNumber == req.TripNumber).FirstOrDefault();
                            if (order != null && travelPaper != null)
                            {
                                order.Passenger.Each(p =>
                                {
                                    if (p.Id == req.PassengerId)
                                    {
                                        p.PassengerTripStatus = EnumPassengerTripStatus.HasVoid;
                                    }
                                });
                                travelPaper.OrderId = req.OrderId;
                                travelPaper.PassengerId = req.PassengerId;
                                travelPaper.TripStatus = EnumTripStatus.HasObsoleteUsed;
                                travelPaper.InvalidTime = System.DateTime.Now;
                                string strLog = "改签行程单号:" + response.TripNumber + "改签票号:" + response.TicketNumber + " 改签Office：" + response.CreateOffice + " " + response.ShowMsg;
                                travelPaper.WriteLog(new TravelPaperLog()
                                {
                                    OperationContent = strLog,
                                    OperationType = "作废改签行程单",
                                    OperationDatetime = System.DateTime.Now,
                                    OperationPerson = currentUser.OperatorAccount
                                });
                                order.Order.OrderLogs.Add(new OrderLog()
                                {
                                    OperationContent = strLog,
                                    OperationDatetime = System.DateTime.Now,
                                    OperationPerson = currentUser.OperatorAccount,
                                    IsShowLog = true
                                });
                                unitOfWorkRepository.PersistUpdateOf(order);
                                unitOfWorkRepository.PersistUpdateOf(travelPaper);
                                unitOfWork.Commit();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new CustomException(111, ex.Message);
            }
            return response;
        }
    }
}
