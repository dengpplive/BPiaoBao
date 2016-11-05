using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BPiaoBao.AppServices.DataContracts.Cashbag;

namespace BPiaoBao.Web.SupplierManager.Models
{
    public class BankCardModel : BankCardDto
    { 
         
        /// <summary>
        /// 带*号显示的银行卡号
        /// </summary>
        public string CardNoShow { get; set; }

        /// <summary>
        /// 用于显示在下拉列表中
        /// </summary>
        public string NameCardNoShow { get; set; }


         
    }
}