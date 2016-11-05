using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.ServiceModel.Web;
using System.Text;

namespace BPiaoBao.AppServices
{
    class EnableJsonFormatterBehavior: IEndpointBehavior
     {
         public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
         {
             foreach (var operation in endpoint.Contract.Operations)
             {
                 DecorateFormatterBehavior(operation);
             }
         }

         public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
         {
             foreach (var operation in endpoint.Contract.Operations)
             {
                 DecorateFormatterBehavior(operation);
             }
         }

         public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters) { }

         public void Validate(ServiceEndpoint endpoint) { }

         private static void DecorateFormatterBehavior(OperationDescription operation)
         {
             //这个行为附加一次。 
             var dfBehavior = operation.Behaviors.Find<WebInvokeAttribute>();
             if (dfBehavior == null)
             {
                 //装饰新的操作行为 
                 //这个行为是操作的行为，但是我们期望只为当前终结点做操作的序列化，所以传入 runtime 进行区分。
                 dfBehavior = new WebInvokeAttribute()
                 {
                     BodyStyle = WebMessageBodyStyle.WrappedRequest,
                     ResponseFormat = WebMessageFormat.Json,
                     RequestFormat = WebMessageFormat.Json
                 };
                 operation.Behaviors.Add(dfBehavior);
             }
         }
     }

    
}
