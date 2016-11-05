using BPiaoBao.Common.Enums;
using JoveZhao.Framework;
using JoveZhao.Framework.DDD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.SystemSetting.Domain.Models.Businessmen
{

    public class Operator : EntityBase
    {
        public int Id { get; set; }
        private string account;

        public string Account
        {
            get { return !string.IsNullOrEmpty(account) ? account.Trim() : string.Empty; }
            set { account = value; }
        }
        public string Password { get; set; }
        public string Realname { get; set; }
        public string Phone { get; set; }
        public string Tel { get; set; }
        public DateTime CreateDate { get; set; }
        public EnumOperatorState OperatorState { get; set; }
        public bool IsAdmin { get; set; }
        public int? RoleID { get; set; }
        public virtual Role Role { get; set; }

        public void ChangePassword(string oldPassword, string newPassword)
        {
            if (Password == oldPassword)
                Password = newPassword;
            else
                throw new ChangePasswordException("新密码与原始密码不一致");
        }

        protected override string GetIdentity()
        {
            return Id.ToString();
        }
    }
}
