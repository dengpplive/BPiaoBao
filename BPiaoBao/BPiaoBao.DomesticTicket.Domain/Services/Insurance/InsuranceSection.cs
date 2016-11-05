using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JoveZhao.Framework;
using StructureMap.Interceptors;

namespace BPiaoBao.DomesticTicket.Domain.Services.Insurance
{
    /// <summary>
    /// 保险配置文件管理类
    /// </summary>
    public class InsuranceSection
    {
        /// <summary>
        /// 获取全部
        /// </summary>
        /// <returns></returns>
        public static InsuranceConfigurationSection GetInsuranceConfigurationSection()
        {
            return SectionManager.GetConfigurationSection<InsuranceConfigurationSection>("insuranceSection");
        } 

         

        /// <summary>
        /// 保存节点
        /// </summary>
        public static void Save()
        {
            SectionManager.SaveConfigurationSection("insuranceSection");
        }
    }
}
