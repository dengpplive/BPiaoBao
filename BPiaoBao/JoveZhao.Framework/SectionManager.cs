using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace JoveZhao.Framework
{
    public static class SectionManager
    {
        private static Configuration cfg = null;
        static Dictionary<string, object> configContainer = new Dictionary<string, object>();
        public static T GetConfigurationSection<T>(string name) where T : ConfigurationSection
        {
            T t = null;
            if (!configContainer.ContainsKey(name))
            {
               cfg = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                t = cfg.GetSection(name) as T;
                if (t == null)
                    throw new NotFoundResourcesException("没有找到节点为：" + name + "的配置项");
                configContainer[name] = t;
            }
            else
            {

                t = configContainer[name] as T;
            }
            return t;
        }


        public static void SaveConfigurationSection(string name)
        {
            if (cfg == null)
            {
                cfg = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            }
            cfg.Save();
            ConfigurationManager.RefreshSection(name);
            configContainer[name] = null;
            configContainer.Remove(name);

        }
    }
}
