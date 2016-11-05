using BPiaoBao.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.AppServices.DataContracts.SystemSetting
{
    public class OperatorDto
    {
        public int Id { get; set; }
        public string Account { get; set; }
        public string Password { get; set; }
        public string Realname { get; set; }
        public string Phone { get; set; }
        public DateTime CreateDate { get; set; }
        public EnumOperatorState OperatorState { get; set; }
        public string Tel { get; set; }
    }
}
