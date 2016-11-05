using StructureMap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JoveZhao.Framework.DDD
{
    //public class BootStrapper
    //{
    //    public static void ConfigureDependenciesByFile()
    //    {

    //        var dddConfig = JZFSection.GetInstances().DDD;
    //        ObjectFactory.Configure(x =>
    //        {

    //            x.For(typeof(IUnitOfWork)).Use(dddConfig.UnitOfWork.UnitOfWorkType);
    //            x.For(typeof(IUnitOfWorkRepository)).Use(dddConfig.UnitOfWork.RepositoryType);

    //            foreach (IocConfigurationElement rep in dddConfig.Repositories)
    //            {
    //                x.For(rep.ForType).Use(rep.UseType);
    //            }

    //            foreach (IocConfigurationElement eve in dddConfig.Events)
    //            {
    //                x.For(eve.ForType).Use(eve.UseType);
    //            }

    //        });
    //    }
        
    //}
}
