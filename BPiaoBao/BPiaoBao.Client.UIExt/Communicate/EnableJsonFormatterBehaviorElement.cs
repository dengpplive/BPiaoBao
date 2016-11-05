using System;
using System.ServiceModel.Configuration;

namespace BPiaoBao.Client.UIExt.Communicate
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
