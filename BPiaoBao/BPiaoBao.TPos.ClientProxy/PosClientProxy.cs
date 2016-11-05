using BPiaoBao.Common;
using BPiaoBaoTPos.Domain.Models;
using BPiaoBaoTPos.Domain.Services;
using JoveZhao.Framework;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.TPos.ClientProxy
{
    public class PosClientProxy : IPosClientProxy
    {
        private string WebUrlTPos = @"" + SettingSection.GetInstances().Cashbag.BaseUrl + "TPos/";
        public Tuple<IEnumerable<BPiaoBaoTPos.Domain.Models.PosInfo>, int> GetPosList(string code, string key, string posNo, string businessmanName, bool? isAssign, int startIndex, int count)
        {
            CashbagHelper ch = new CashbagHelper(WebUrlTPos + "QueryPOS", "GET");
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            dictionary.Add("code", code);
            dictionary.Add("key", key);
            dictionary.Add("CompanyName",businessmanName);
            if (isAssign.HasValue)
            {
                dictionary.Add("Status", isAssign.Value == true ? "1" : "0");
            }
            dictionary.Add("POSNumber", posNo);
            var page = Math.Ceiling((double)startIndex / count) + 1;
            dictionary.Add("CurrentPage", page.ToString());
            dictionary.Add("PageSize", count.ToString());
            dictionary.Add("currentTime", DateTime.Now.ToString("yyyyMMddHHmmss"));
            string data = ch.ParamsURLEncode(dictionary);
            var result = ch.GetBackJsonData(data);
            if (result.status == false)
                throw new CustomException(500, result.message.ToString());
            List<PosInfo> list = new List<PosInfo>();
            var rows = JArray.FromObject(result.result.rows);
            int totalcount = result.result.total;
            foreach (var item in rows)
            {
                PosInfo posinfo = new PosInfo()
                {
                    BusinessmanName = item.PosCompany,
                    PosNo = item.PosNumber,
                    PosRate = item.posRate,
                    Status = item.Status == 1 ? true : false,
                    StatusStr = item.StatusStr,
                    CompanyID = item.CompanyID
                };
                list.Add(posinfo);
            }
            Tuple<IEnumerable<PosInfo>, int> tuple = new Tuple<IEnumerable<PosInfo>, int>(list,totalcount);
            return tuple;
        }

        public IEnumerable<BPiaoBaoTPos.Domain.Models.PosAssignLog> GetPosAssignLogs(string code, string key, string posNo)
        {
            CashbagHelper ch = new CashbagHelper(WebUrlTPos + "DistributionLog", "GET");
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            dictionary.Add("code", code);
            dictionary.Add("key", key);
            dictionary.Add("POSNumber", posNo);
            dictionary.Add("currentTime", DateTime.Now.ToString("yyyyMMddHHmmss"));
            string data = ch.ParamsURLEncode(dictionary);
            var result = ch.GetBackJsonData(data);
            if (result.status == false)
                throw new CustomException(500, result.message.ToString());
            List<PosAssignLog> list = new List<PosAssignLog>();
            var rows = JArray.FromObject(result.result.rows);
            foreach (var item in rows)
            {
                PosAssignLog poslog = new PosAssignLog()
                {
                    Content = item.Content,
                    Operater = item.OperationUser,
                    OperateTime = item.CreateDate
                };
                list.Add(poslog);
            }
            return list;
        }
    }
}
