using StructureMap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JoveZhao.Framework.DDD
{
    public class AggregationFactory 
    {
        public static T CreateBuiler<T>() where T:IAggregationBuilder
        {
            return ObjectFactory.GetInstance<T>();
        }
    }
}
