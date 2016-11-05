using System;
using System.ComponentModel.DataAnnotations;

namespace BPiaoBao.Client.UIExt.Helper
{
    /// <summary>
    /// WPF 输入逻辑 验证助手
    /// </summary>
    public class ValidationHelper
    {
        #region 成员变量

        private static object sync = new object();
        private static ValidationHelper _instance;

        #endregion

        #region 构造

        /// <summary>
        /// Prevents a default instance of the <see cref="ValidationHelper"/> class from being created.
        /// </summary>
        private ValidationHelper()
        {

        }

        #endregion

        #region 公开属性

        /// <summary>
        /// 单例实例访问
        /// </summary>
        public static ValidationHelper Instance
        {
            get
            {
                if (_instance == null)
                {
                    //提高性能
                    lock (sync)
                    {
                        if (_instance == null)
                            _instance = new ValidationHelper();
                    }
                }

                return _instance;
            }
        }

        #endregion

        #region 公开方法

        /// <summary>
        /// 验证属性是否通过，不通过抛出异常,可在vs中过滤该异常，仅仅是通知界面显示
        /// </summary>
        /// <param name="memberName">属性名称</param>
        /// <param name="context">验证上下文</param>
        /// <param name="value">验证值</param>
        public void ValidateProperty(string memberName, object context, string value)
        {
            var validatorContext = new ValidationContext(context, null, null);
            validatorContext.MemberName = memberName;
            Validator.ValidateProperty(value, validatorContext);
        }

        #endregion

        /// <summary>
        /// 验证一个对象的所有属性是否通过，不通过返回false
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool ValidateModel(object model)
        {
            var validatorContext = new ValidationContext(model, null, null);

            bool isOk = Validator.TryValidateObject(model, validatorContext, null);

            return isOk;
        }

        /// <summary>
        /// 判断是否是手机电话号码
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public bool IsMobilePhoneNum(string num)
        {
            if (string.IsNullOrEmpty(num))
            {
                return false;
            }
            bool result = System.Text.RegularExpressions.Regex.IsMatch(num, @"^1[1-9]\d{9}$");
            return result;
        }

        public void CheckPhoneNum(string num)
        {
            ProjectHelper.Utils.Guard.CheckIsNullOrEmpty(num, "号码");
            bool result = System.Text.RegularExpressions.Regex.IsMatch(num, @"^\d[-\d]+\d$");
            if (result)
            {
                throw new Exception("号码不对");
            }
        }
    }
}
