using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.DomesticTicket.Domain.Services
{
    /// <summary>
    /// 计算金额类
    /// </summary>
    public class DataBill
    {
        /// <summary>
        /// //四舍五入
        /// </summary>
        /// <param name="number">数据</param>
        /// <param name="pointLength">保留小数位数</param>
        /// <returns></returns>
        public decimal Round(decimal number, int pointLength)
        {
            decimal _returnNumber = Math.Round(number + 0.0000001M, pointLength);
            return _returnNumber;
        }
        /// <summary>
        /// 单人支付价格
        /// </summary>
        /// <param name="SeatPrice"></param>
        /// <param name="TaxFee"></param>
        /// <param name="RQFee"></param>
        /// <param name="Point"></param>
        /// <param name="ReturnMoney"></param>
        /// <returns></returns>
        public decimal GetPayPrice(decimal seatPrice, decimal taxFee, decimal rQFee, decimal point, decimal returnMoney)
        {
            return seatPrice + taxFee + rQFee - GetCommission(point, seatPrice, returnMoney);
        }
        /// <summary>
        /// 出票方单人收款
        /// </summary>
        /// <param name="seatPrice"></param>
        /// <param name="taxFee"></param>
        /// <param name="rQFee"></param>
        /// <param name="point"></param>
        /// <param name="returnMoney"></param>
        /// <returns></returns>
        public decimal GetRecvPrice(decimal seatPrice, decimal taxFee, decimal rQFee, decimal point, decimal returnMoney)
        {
            return Round((seatPrice + taxFee + rQFee - point / 100 * seatPrice - returnMoney), 2);
        }
        /// <summary>
        /// 获取个人佣金 取整
        /// </summary>
        /// <param name="PolicyPoint"></param>
        /// <param name="seatPrice"></param>
        /// <returns></returns>
        public decimal GetCommission(decimal policyPoint, decimal seatPrice, decimal returnMoney)
        {
            //return Round(MinusCeilNum(policyPoint / 100 * seatPrice) - returnMoney, 2);
            return MinusCeilNum(policyPoint / 100 * seatPrice - returnMoney);
        }
        /// <summary>
        /// 获取佣金的小数部分 取小数
        /// </summary>
        /// <param name="policyPoint"></param>
        /// <param name="seatPrice"></param>
        /// <param name="returnMoney"></param>
        /// <returns></returns>
        public decimal GetCommissionPartNumber(decimal policyPoint, decimal seatPrice, decimal returnMoney)
        {
            decimal commission = policyPoint / 100 * seatPrice - returnMoney;
            int temp = (int)commission;
            decimal number = commission - temp;
            return number;
        }

        #region 数据处理
        /// <summary>
        /// 保留两位小数（只入不舍）
        /// </summary>
        /// <param name="del"></param>
        /// <returns></returns>
        public decimal AddCeilNum(decimal del)
        {
            decimal _del = Math.Round(del + 0.005M - 0.0001M, 2);
            return _del;
        }

        /// <summary>
        /// 保留一位小数（只舍不入）
        /// </summary>
        /// <param name="del"></param>
        /// <returns></returns>
        public decimal MinusCeilSmallNumOne(decimal del)
        {
            decimal _del = Math.Round(del - 0.05M + 0.001M, 1);
            return _del;
        }

        /// <summary>
        /// 保留两位小数（只舍不入）
        /// </summary>
        /// <param name="del"></param>
        /// <returns></returns>
        public decimal MinusCeilSmallNum(decimal del)
        {
            decimal _del = Math.Round(del - 0.005M + 0.0001M, 2);
            return _del;
        }

        /// <summary>  
        /// 实现数据的四舍五入法, 保留小数
        /// </summary>  
        /// <param name="v">要进行处理的数据</param>  
        /// <param name="x">保留的小数位数</param>   
        /// <returns>四舍五入后的结果</returns>   
        public decimal FourToFiveNum(decimal v, int x)
        {
            decimal _del = Math.Round(v + 0.0000001M, x);// //四舍五入
            return _del;
        }

        /// <summary>
        /// 保留整数（只入不舍）
        /// </summary>
        /// <param name="del"></param>
        /// <returns></returns>
        public decimal AddCeilNumInteger(decimal del)
        {
            bool _isF = del < 0 ? true : false;
            decimal result = Math.Ceiling(Math.Abs(del));
            return _isF ? result * -1 : result;

        }

        /// <summary>
        /// 保留整数（只舍不入）
        /// </summary>
        /// <param name="del"></param>
        /// <returns></returns>
        public decimal MinusCeilNum(decimal del)
        {
            bool _isF = del < 0 ? true : false;
            decimal result = Math.Floor(Math.Abs(del));
            return _isF ? result * -1 : result;
        }

        /// <summary>
        /// 四舍五入到十位
        /// </summary>
        /// <param name="del"></param>
        /// <returns></returns>
        public decimal MinusCeilTen(decimal del)
        {
            decimal temp = del / 10;
            decimal result = Math.Round(temp, 0, MidpointRounding.AwayFromZero) * 10;
            return result;
        }

        /// <summary>
        ///  保留到十位 只舍不进
        /// </summary>
        /// <param name="del"></param>
        /// <returns></returns>
        public decimal CeilTen(decimal del)
        {
            bool _isF = del < 0 ? true : false;
            decimal temp = Math.Abs(del / 10);
            decimal result = Math.Floor(temp) * 10;
            return _isF ? result * -1 : result;
        }

        /// <summary>
        /// 保留到十位 只进不舍
        /// </summary>
        /// <param name="del"></param>
        /// <returns></returns>
        public decimal CeilAddTen(decimal del)
        {
            bool _isF = del < 0 ? true : false;
            decimal temp = Math.Abs(del / 10);
            decimal result = Math.Ceiling(temp) * 10;
            return _isF ? result * -1 : result;
        }

        /// <summary>
        /// 保留到角 
        /// </summary>
        /// <param name="del"></param>
        /// <returns></returns>
        public decimal CeilAngle(decimal del)
        {
            bool _isF = del < 0 ? true : false;
            decimal d = Math.Abs(del * 10);
            del = Math.Ceiling(d) / 10;
            return _isF ? del * -1 : del;
        }

        /// <summary>
        /// 保留到分
        /// </summary>
        /// <param name="del"></param>
        /// <returns></returns>
        public decimal CeilFeng(decimal del)
        {
            bool _isF = del < 0 ? true : false;
            decimal d = Math.Abs(del * 100);
            del = Math.Ceiling(d) / 100;
            return _isF ? del * -1 : del;
        }

        /// <summary>
        /// 四舍五入
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="pointLength">保留小数位数</param>
        /// <returns></returns>
        public decimal NewRound(decimal data, int pointLength)
        {
            return Math.Round(data, pointLength, MidpointRounding.AwayFromZero);
        }
        #endregion

    }
}
