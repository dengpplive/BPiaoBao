using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;
using JoveZhao.Framework.Expand;
using System.Xml.Linq;
using NPOI.SS.Formula.Functions;
using System.Collections;

namespace BPiaoBao.Web.SupplierManager.Controllers.Helpers
{
    public static class MenuHelper
    {
        public static List<Module> GetMenu()
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + "Menu.config";
            XElement root = XElement.Load(path);
            var list = root.Descendants("Module").Select(p => new Module
            {
                Code = int.Parse(p.Attribute("Code").Value),
                Name = p.Attribute("Name").Value,
                Icon = p.Attribute("Icon").Value,
                Share = int.Parse(p.Attribute("Share").Value),
                Menus = p.Elements("Menus").Select(n => new Menu
                {
                    Code = int.Parse(n.Attribute("Code").Value),
                    Name = n.Attribute("Name").Value,
                    Url = n.Attribute("Url") != null ? n.Attribute("Url").Value : string.Empty,
                    Share = int.Parse(n.Attribute("Share").Value),
                    ItemMenus = n.Elements("Menu").Select(m => new Menu
                    {
                        Code = int.Parse(m.Attribute("Code").Value),
                        Name = m.Attribute("Name").Value,
                        Url = m.Attribute("Url") != null ? m.Attribute("Url").Value : string.Empty,
                        Share = int.Parse(m.Attribute("Share").Value)
                    }).ToList()
                }).ToList()
            }).ToList();
            return list;
        }

    }

    /// <summary>
    /// 程序模块
    /// </summary>
    public class Module
    {
        /// <summary>
        /// 代码
        /// </summary>
        public int Code { get; set; }
        /// <summary>
        /// 模块名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 图标
        /// </summary>
        public string Icon { get; set; }
        /// <summary>
        /// 权限访问
        /// </summary>
        public int Share { get; set; }
        /// <summary>
        /// 下级菜单
        /// </summary>
        public List<Menu> Menus { get; set; }
    }
    /// <summary>
    /// 菜单导航
    /// </summary>
    public class Menu
    {
        /// <summary>
        /// 导航代码
        /// </summary>
        public int Code { get; set; }
        /// <summary>
        /// 导航名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 链接地址
        /// </summary>
        public string Url { get; set; }
        /// <summary>
        /// 下级导航
        /// </summary>
        public List<Menu> ItemMenus { get; set; }
        /// <summary>
        /// 权限访问
        /// </summary>
        public int Share { get; set; }

    }
}