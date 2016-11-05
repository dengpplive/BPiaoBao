using System.Threading.Tasks;
using JoveZhao.Framework;
using JoveZhao.Framework.Expand;
using StructureMap;
using System;
using System.Collections.Generic;

using System.Linq;
using System.Text;

namespace BPiaoBao.Client.Module
{

    public class PluginManager
    {

        public static void Register()
        {
            ObjectFactory.Configure(x =>
            {
                x.Scan(p =>
                {
                    p.AssembliesFromApplicationBaseDirectory();
                    p.AddAllTypesOf<IPlugin>().NameBy(t => t.GetCustomAttribute<PluginAttribute>().Code);
                });
                x.For<IPlugin>().Singleton();
            });

            ObjectFactory.Configure(x =>
            {

                ObjectFactory.Model.InstancesOf<IPlugin>().ForEach(ir =>
                {
                    x.Scan(p =>
                    {
                        p.Assembly(ir.ConcreteType.Assembly);
                        p.AddAllTypesOf<IPart>().NameBy(t =>
                            ir.Name + "_" + t.GetCustomAttribute<PartAttribute>().Code);
                    });
                });
                x.For<IPart>().Singleton();
            });

        }
        public static IList<PluginAttribute> Modules
        {
            get
            {
                var lst = ObjectFactory.Model.InstancesOf<IPlugin>()
                    .Select(p => p.ConcreteType.GetCustomAttribute<PluginAttribute>())
                    .OrderBy(p => p.Sequence).ToList();
                return lst;
            }
        }

        public static IPlugin GetPlugin(string code)
        {
            return ObjectFactory.GetNamedInstance<IPlugin>(code);
        }

        public static IPart FindItem(string projectCode, string menuCode)
        {
           return ObjectFactory.GetNamedInstance<IPart>(projectCode + "_" + menuCode);
        }



        /// <summary>
        /// 注册命令集模块
        /// </summary>

        public static void RegisterClientCommand()
        {
            var action = new Action(() =>
            {
                ObjectFactory.Configure(x =>
                {
                    x.Scan(p =>
                    {
                        p.AssembliesFromApplicationBaseDirectory();
                        p.AddAllTypesOf<IClientCommand>().NameBy(t => t.GetCustomAttribute<ClientCommandAttribute>().CommandKey);
                    });

                    x.For<IClientCommand>().Singleton();
                });


            });

            Task.Factory.StartNew(action);


        }

        /// <summary>
        /// 获取命令集模块
        /// </summary>
        public static Dictionary<IClientCommand, ClientCommandAttribute> ClientCommandModules
        {
            get
            {
                var dict = new Dictionary<IClientCommand, ClientCommandAttribute>();
                var lst = ObjectFactory.Model.InstancesOf<IClientCommand>();
                //.Select(p => p.ConcreteType.GetCustomAttribute<ClientCommandAttribute>()).ToList();
                foreach (var d in lst.ToList())
                {
                    dict.Add(d.Get<IClientCommand>(), d.ConcreteType.GetCustomAttribute<ClientCommandAttribute>());
                }
                return dict;
            }
        }
    }

}
