using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JoveZhao.Framework;
using System.Data;
using System.IO;
using System.Xml;



namespace BPiaoBao.DomesticTicket.Platform.Plugin
{
    public abstract class BasePlatform
    {

        protected PlatFormElement platformConfig;
        public BasePlatform()
        {
            platformConfig = PlatformSection.GetInstances().Platforms[Code];


        }
        public abstract string Code { get; }


        public bool IsClosed
        {
            get { return platformConfig.IsClosed; }
        }

        public string GetPlatformName()
        {
            return this.platformConfig.Name;
        }
    }
}
