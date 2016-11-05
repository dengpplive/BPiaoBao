/***********************************************************************   
* Copyright(c)    
* CLR 版本: 4.0.30319.34014   
* 命名空间: BPiaoBao.AppServices
* 文 件 名：ExtOperationInvoker.cs   
* 创 建 人：duanwei   
* 创建日期：2014/11/11 17:36:08       
* 备注描述：           
************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.ServiceModel.Dispatcher;
using System.Text;
using System.Threading.Tasks;
using BPiaoBao.AppServices.Contracts.SystemSetting.DataObject;
using BPiaoBao.AppServices.SystemSetting;
using BPiaoBao.Common.Enums;
using JoveZhao.Framework;
using JoveZhao.Framework.Expand;

namespace BPiaoBao.AppServices
{
    public class ExtOperationInvoker : IOperationInvoker
    {
        /// <summary>
        /// 接口成员
        /// </summary>
        private IOperationInvoker m_OldInvoker;
        /// <summary>
        /// 日志记录类型
        /// </summary>
        private EnumInterception m_InteType;
        /// <summary>
        /// 日志忽略记录字段
        /// </summary>
        private EnumIgnoreInterception m_IgnoreType;

        /// <summary>
        /// 访问的函数名
        /// </summary>
        private string m_FunctionName;

        /// <summary>
        /// 函数描述
        /// </summary>
        private string m_FunctionDescription;
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="oldInvoker"></param>
        /// <param name="functionDescription"></param>
        /// <param name="inteType"></param>
        /// <param name="ignoreType"></param>
        public ExtOperationInvoker(IOperationInvoker oldInvoker, string functionDescription, EnumInterception inteType, EnumIgnoreInterception ignoreType)
        {
            var mInfo = (MethodInfo)oldInvoker.GetType().GetProperty("Method").GetValue(oldInvoker, null);
            m_OldInvoker = oldInvoker;
            m_InteType = inteType;
            m_FunctionName = mInfo.Name;
            m_FunctionDescription = functionDescription;
            m_IgnoreType = ignoreType;
        }
        public object[] AllocateInputs()
        {
            return m_OldInvoker.AllocateInputs();
        }

        public object Invoke(object instance, object[] inputs, out object[] outputs)
        {
            PreInvoke(instance, inputs);
            object returnedValue = null;
            var outputParams = new object[] { };
            Exception exception = null;
            returnedValue = m_OldInvoker.Invoke(instance, inputs, out outputParams);
            outputs = outputParams;
            //Console.WriteLine("outputs:" + outputs.ToJson());
            PostInvoke(instance, returnedValue, outputParams, exception);
            return returnedValue;




        }

        public IAsyncResult InvokeBegin(object instance, object[] inputs, AsyncCallback callback, object state)
        {
            PreInvoke(instance, inputs);
            return m_OldInvoker.InvokeBegin(instance, inputs, callback, state);
        }

        public object InvokeEnd(object instance, out object[] outputs, IAsyncResult result)
        {
            object returnedValue = null;
            object[] outputParams = { };
            Exception exception = null;
            returnedValue = m_OldInvoker.InvokeEnd(instance, out outputs, result);
            outputs = outputParams;
            PostInvoke(instance, returnedValue, outputParams, exception);
            return returnedValue;
        }

        public bool IsSynchronous
        {
            get
            {
                return m_OldInvoker.IsSynchronous;
            }
        }


        protected void PreInvoke(object instance, object[] inputs)
        {
            switch (m_InteType)
            {
                case EnumInterception.None:
                    break;
                case EnumInterception.LogOperation:
                    {
                        var tempList = inputs.ToList();
                        if (m_IgnoreType == EnumIgnoreInterception.Password)
                        {
                            try
                            {
                                switch (m_FunctionName.ToLower())
                                {
                                    case "consologin":
                                    case "login":
                                        tempList.RemoveAt(2);
                                        break;
                                    case "buyinsurancebycashbycarrier":
                                        tempList.RemoveAt(1);
                                        break;
                                    case "buyinsurancebycashorcredit":
                                        tempList.RemoveAt(3);
                                        break;
                                    case "payorderbycashbagaccount":
                                        tempList.RemoveAt(1);
                                        break;
                                    case "payorderbycreditaccount":
                                        tempList.RemoveAt(1);
                                        break;
                                    case "saleorderpaybycashbagaccount":
                                        tempList.RemoveAt(1);
                                        break;
                                    case "saleorderpaybycreditaccount":
                                        tempList.RemoveAt(1);
                                        break;
                                    case "changepassword":
                                        tempList.RemoveAt(1);
                                        tempList.RemoveAt(2);
                                        break;
                                    case "buysmsbyaccount":
                                        tempList.RemoveAt(3);
                                        break;
                                    case "alipaybind":
                                        tempList.RemoveAt(1);
                                        break;
                                    case "alipayunbind":
                                        tempList.RemoveAt(0);
                                        break;
                                    case "alipaysignrecharge":
                                        tempList.RemoveAt(1);
                                        break;
                                    case "alipaysignrepay":
                                        tempList.RemoveAt(1);
                                        break;
                                    case "payorderbyquikalipay":
                                         tempList.RemoveAt(1);
                                        break;
                                }
                            }
                            catch (Exception ex)
                            {

                                Logger.WriteLog(LogType.ERROR, ex.Message);
                            }

                        }
                   

                        var log = new OperationLogDto
                        {
                            CreateTime = DateTime.Now,
                            FunctionDescription = m_FunctionDescription,
                            FunctionName = m_FunctionName,
                            ModuleFullName = instance.ToString(),
                            RequestParams = tempList.ToJson(),
                            OperatorAcount = AuthManager.GetCurrentUser() == null
                                ? ""
                                : AuthManager.GetCurrentUser().OperatorAccount,
                            BusinessCode = AuthManager.GetCurrentUser() == null
                                ? ""
                                : AuthManager.GetCurrentUser().Code,
                            BusinessName = AuthManager.GetCurrentUser() == null
                          ? ""
                          : AuthManager.GetCurrentUser().BusinessmanName,
                        };
                        Task.Factory.StartNew(() => QueueLogsManager.Enqueue(log));
                      
                        //var sb = new StringBuilder();
                        //sb.Append("\n请求时间：" + DateTime.Now);
                        //sb.Append("\n服务名称：" + instance);
                        //sb.Append("\n函数描述：" + m_FunctionDescription);
                        //sb.Append("\n函数名称：" + m_FunctionName);
                        //sb.Append("\n请求参数：" + tempList.ToJson());
                        //if (AuthManager.GetCurrentUser() != null)
                        //{
                        //    sb.Append("\n操作人：" + AuthManager.GetCurrentUser().Code);
                        //}

                        //Console.WriteLine(sb.ToString());
                    }
                    break;
                case EnumInterception.LogException:
                    break;
            }
        }

        protected void PostInvoke(object instance, object returnedValue, object[] outputs, Exception err)
        {
            switch (m_InteType)
            {
                case EnumInterception.None:
                    break;
                case EnumInterception.LogOperation:
                    break;
                case EnumInterception.LogException:
                    break;
            }
        }
    }
}

