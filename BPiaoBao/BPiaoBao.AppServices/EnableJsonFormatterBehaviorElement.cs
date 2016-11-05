using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Configuration;
using System.Text;

namespace BPiaoBao.AppServices
{
    class EnableJsonFormatterBehaviorElement : BehaviorExtensionElement
    {
        public override Type BehaviorType
        {
            get { return typeof(EnableJsonFormatterBehavior); }
        }

        protected override object CreateBehavior()
        {
            return new EnableJsonFormatterBehavior();
        }
    }


}
