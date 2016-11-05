using AutoMapper;
using BPiaoBao.AppServices.Contracts.TPos;
using BPiaoBao.AppServices.DataContracts;
using BPiaoBaoTPos.Domain.Models;
using BPiaoBaoTPos.Domain.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.AppServices.TPos
{
    public class TPosService : ITPosService
    {
        IBusinessmanClientProxy businessmanClientProxy;
        IPosClientProxy posClientProxy;
        IPosStatClientProxy posStatClientProxy;
        string code, key;
        public TPosService(IBusinessmanClientProxy businessmanClientProxy, IPosClientProxy posClientProxy, IPosStatClientProxy posStatClientProxy)
        {
            this.businessmanClientProxy = businessmanClientProxy;
            this.posClientProxy = posClientProxy;
            this.posStatClientProxy = posStatClientProxy;
            code = AuthManager.GetCurrentUser().CashbagCode;
            key = AuthManager.GetCurrentUser().CashbagKey;
        }


        public void AddBusinessman(DataContracts.TPos.RequestBusinessmanInfo businessmanInfo)
        {
            this.businessmanClientProxy.AddBusinessman(code, key,AuthManager.GetCurrentUser().OperatorName, Mapper.Map<BusinessmanInfo>(businessmanInfo));
        }

        public void UpdateBusinessman(DataContracts.TPos.RequestBusinessmanInfo businessmanInfo)
        {
            this.businessmanClientProxy.UpdateBusinessman(code, key, AuthManager.GetCurrentUser().OperatorName, Mapper.Map<BusinessmanInfo>(businessmanInfo));
        }
        public void DeleteBusinessman(string Id)
        {
            this.businessmanClientProxy.DeleteBusinessman(code, key, AuthManager.GetCurrentUser().OperatorName, Id);
        }
        public DataContracts.TPos.ResponseBusinessmanInfo GetBusinessmanInfo(string Id)
        {
            var model = this.businessmanClientProxy.GetBusinessmanInfo(code, key, Id);
            return Mapper.Map<BPiaoBao.AppServices.DataContracts.TPos.ResponseBusinessmanInfo>(model);
        }

        public void AssignPos(string Id, string[] posNoList)
        {
            this.businessmanClientProxy.AssignPos(code, key, AuthManager.GetCurrentUser().OperatorName, Id, posNoList);
        }

        public DataContracts.DataPack<DataContracts.TPos.ResponseBusinessmanInfo> GetPosBusinessman(string businessmanName, string posNo, int startIndex, int count)
        {
            Tuple<IEnumerable<BusinessmanInfo>, int> tuple = this.businessmanClientProxy.GetPosBusinessman(code, key, businessmanName, posNo, startIndex, count);
            DataPack<BPiaoBao.AppServices.DataContracts.TPos.ResponseBusinessmanInfo> dataPack = new DataPack<DataContracts.TPos.ResponseBusinessmanInfo>();
            dataPack.TotalCount = tuple.Item2;
            dataPack.List = Mapper.Map<List<BusinessmanInfo>, List<BPiaoBao.AppServices.DataContracts.TPos.ResponseBusinessmanInfo>>(tuple.Item1.ToList());
            return dataPack;
        }

        public DataContracts.DataPack<DataContracts.TPos.PosInfoDataObject> GetPosList(string posNo, string businessmanName, bool? isAssign, int startIndex, int count)
        {
            Tuple<IEnumerable<PosInfo>, int> tuple = this.posClientProxy.GetPosList(code, key, posNo, businessmanName, isAssign, startIndex, count);
            DataPack<BPiaoBao.AppServices.DataContracts.TPos.PosInfoDataObject> dataPack = new DataPack<DataContracts.TPos.PosInfoDataObject>();
            dataPack.TotalCount = tuple.Item2;
            dataPack.List = Mapper.Map<List<PosInfo>, List<BPiaoBao.AppServices.DataContracts.TPos.PosInfoDataObject>>(tuple.Item1.ToList());
            return dataPack;
        }

        public IEnumerable<DataContracts.TPos.PosAssignLogDataObject> GetPosAssignLogs(string posNo)
        {
            IEnumerable<PosAssignLog> plogs = this.posClientProxy.GetPosAssignLogs(code, key, posNo);
            return Mapper.Map<IEnumerable<PosAssignLog>, IEnumerable<BPiaoBao.AppServices.DataContracts.TPos.PosAssignLogDataObject>>(plogs);
        }

        public DataContracts.TPos.AccountStatDataObject GetAccountStat()
        {
            var model = this.posStatClientProxy.GetAccountStat(code, key);
            return Mapper.Map<BPiaoBao.AppServices.DataContracts.TPos.AccountStatDataObject>(model);
        }

        public IEnumerable<DataContracts.TPos.TradeStatDataObject> GainStat(DateTime startTime, DateTime endTime)
        {
            IEnumerable<TradeStat> tradeStats = this.posStatClientProxy.GainStat(code, key, startTime, endTime);
            return Mapper.Map<IEnumerable<TradeStat>, IEnumerable<BPiaoBao.AppServices.DataContracts.TPos.TradeStatDataObject>>(tradeStats);
        }

        public DataContracts.DataPack<DataContracts.TPos.TradeDetailDataObject> GetTradeDetail(DateTime? startTime, DateTime? endTime, string posNo, int startIndex, int count)
        {
            Tuple<IEnumerable<TradeDetail>, int> tuple = this.posStatClientProxy.GetTradeDetail(code, key, startTime, endTime, posNo, startIndex, count);
            DataPack<BPiaoBao.AppServices.DataContracts.TPos.TradeDetailDataObject> dataPack = new DataPack<DataContracts.TPos.TradeDetailDataObject>();
            dataPack.TotalCount = tuple.Item2;
            dataPack.List = Mapper.Map<List<TradeDetail>, List<BPiaoBao.AppServices.DataContracts.TPos.TradeDetailDataObject>>(tuple.Item1.ToList());
            return dataPack;
        }

        public DataContracts.TPos.BusinessmanReportDataObject GetBusinessmanReport(DateTime startTime, DateTime endTime)
        {
            var model = this.posStatClientProxy.GetBusinessmanReport(code, key, startTime, endTime);
            return Mapper.Map<BPiaoBao.AppServices.DataContracts.TPos.BusinessmanReportDataObject>(model);
        }


        public void RetrievePos(string Id, string PosNo)
        {
            this.businessmanClientProxy.RetrievePos(code, key, Id, PosNo, AuthManager.GetCurrentUser().OperatorName);
        }
    }
}
