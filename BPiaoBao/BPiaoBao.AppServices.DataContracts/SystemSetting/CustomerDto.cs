using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.AppServices.DataContracts.SystemSetting
{
    public class CustomerDto
    {
        public CustomerDto()
        {
            this.AdvisoryQQ = new List<KeyAndValueDto>();
        }
        public List<KeyAndValueDto> AdvisoryQQ{ get; set; }
        public List<KeyAndValueDto> HotlinePhone { get; set; }
        public string CustomPhone { get; set; }
    }
    public class KeyAndValueDto
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }
}
