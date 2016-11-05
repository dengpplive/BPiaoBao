using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Norm;

namespace BPiaoBao.DomesticTicket.Domain.Models
{
    /// <summary>
    /// 舱位
    /// </summary>
    /// 
    [Serializable]
    public class CabinSeat
    {
        public CabinSeat()
        {
            _id = Guid.NewGuid().ToString();
        }
        public string _id { get; set; }
        /// <summary>
        /// 承运人二字码
        /// </summary>
        public string CarrierCode { get; set; }//33
        /// <summary>
        /// 航线
        /// </summary>
        public Airline Airline { get; set; }//4069
        /// <summary>
        /// 燃油
        /// </summary>
        public Fuel Fuel { get; set; }
        /// <summary>
        /// 舱位列表
        /// </summary>
        public List<Seat> SeatList { get; set; }//10       
    }
    [Serializable]
    public class Airline
    {
        //public Airline()
        //{
        //    _id = ObjectId.NewObjectId();
        //}
        //public ObjectId _id { get; set; }
        /// <summary>
        /// Y舱舱位价
        /// </summary>
        public decimal SeatPrice { get; set; }
        /// <summary>
        /// 里程
        /// </summary>
        public int Mileage { get; set; }
        /// <summary>
        /// 出发三字码
        /// </summary>
        public string FromCode { get; set; }

        /// <summary>
        /// 到达三字码
        /// </summary>
        public string ToCode { get; set; }
        /// <summary>
        /// 航线
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return FromCode + "-" + ToCode;
        }
    }
    [Serializable]
    public class Seat
    {
        //public Seat()
        //{
        //    _id = ObjectId.NewObjectId();
        //}
        //public ObjectId _id { get; set; }
        /// <summary>
        /// 基本舱位代码
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 基本折扣
        /// </summary>
        public decimal Rebate { get; set; }
        /// <summary>
        /// 舱位折扣
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Code + "-" + Rebate;
        }
    }
    [Serializable]
    public class Fuel
    {
        //public Fuel()
        //{
        //    _id = ObjectId.NewObjectId();
        //}
        //public ObjectId _id { get; set; }
        /// <summary>
        /// 儿童燃油
        /// </summary>
        public decimal ChildFuelFee { get; set; }
        /// <summary>
        /// 成人燃油
        /// </summary>
        public decimal AdultFuelFee { get; set; }
    }
}
