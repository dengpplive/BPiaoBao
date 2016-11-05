/***********************************************************************   
* Copyright(c)    
* CLR 版本: 4.0.30319.34014   
* 命名空间: BPiaoBao.Client.DomesticTicket.View.Converters
* 文 件 名：PayInfoConverter.cs   
* 创 建 人：duanwei   
* 创建日期：2014/11/28 11:43:54       
* 备注描述：           
************************************************************************/
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Data;
using BPiaoBao.AppServices.DataContracts.DomesticTicket;
using BPiaoBao.Client.UIExt.Model;

namespace BPiaoBao.Client.DomesticTicket.View.Converters
{
    public class PayInfoConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is OrderPayDto)) return "";
            var pay = (OrderPayDto)value;
            if (pay.PayMethod != null && pay.PayMethod.ToLower().Equals("银行卡"))
            {
                var bank = BankData.GetBankInfoByCode(pay.PayMethodCode);
                return "财付通(" + bank == null ? "" : bank.Name + ")";

            }
            return pay.PayMethod;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
