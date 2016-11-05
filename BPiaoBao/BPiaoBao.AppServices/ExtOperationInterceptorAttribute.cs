/***********************************************************************   
* Copyright(c)    
* CLR 版本: 4.0.30319.34014   
* 命名空间: BPiaoBao.AppServices
* 文 件 名：ExtOperationInterceptorAttribute.cs   
* 创 建 人：duanwei   
* 创建日期：2014/11/11 17:40:11       
* 备注描述：           
************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.Text;
using BPiaoBao.Common.Enums;

namespace BPiaoBao.AppServices
{
    [AttributeUsage(AttributeTargets.Method)]
    public class ExtOperationInterceptorAttribute : Attribute, IOperationBehavior
    {
        /// <summary>
        /// 日志记录类型(默认记录操作日志类型)
        /// </summary>
        private EnumInterception m_InteType = EnumInterception.LogOperation;

        /// <summary>
        /// 日志忽略记录字段
        /// </summary>
        private EnumIgnoreInterception m_IgnoreType=EnumIgnoreInterception.Password;

         
        /// <summary>
        /// 函数描述
        /// </summary>
        private string m_FunctionDescription;
 
         
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="functionDescription">函数功能描述</param>
        public ExtOperationInterceptorAttribute(string functionDescription)
        {
            this.m_FunctionDescription = functionDescription;

        }


 

        protected ExtOperationInvoker CreateInvoker(IOperationInvoker oldInvoker)
        {
            return new ExtOperationInvoker(oldInvoker, m_FunctionDescription, m_InteType, m_IgnoreType);
        }

        public void AddBindingParameters(OperationDescription operationDescription, BindingParameterCollection bindingParameters)
        { }

        public void ApplyClientBehavior(OperationDescription operationDescription, ClientOperation clientOperation)
        { }

        public void ApplyDispatchBehavior(OperationDescription operationDescription, DispatchOperation dispatchOperation)
        {
            IOperationInvoker oldInvoker = dispatchOperation.Invoker;
            dispatchOperation.Invoker = CreateInvoker(oldInvoker);
        }

        public void Validate(OperationDescription operationDescription)
        { }
    }
}
