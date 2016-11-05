using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Net.Sockets;
using JoveZhao.Framework.Expand;
using System.Threading;

namespace JoveZhao.Framework.SOA
{
    /*
     * Pool.Init();
     * Client client=new Client();
     * Client.Pushed+=Pushed;
     * client.Send();
     */

   
    public class SoaClient
    {
        private Pool pool;
        public SoaClient(Pool pool)
        {
            this.pool = pool;
        }

        public ResponseMessage Send(RequestMessage message)
        {
            var item = pool.Dequeue();
            var client = item.tcpClient;
            var ns = client.GetStream();
            var data = message.ToBytes();
            var datal = data.Length.GetBytes();
            ns.Write(datal, 0, datal.Length);
            ns.Write(data, 0, data.Length);
            ns.Flush();


            var dl = new byte[4];
            ns.Read(dl, 0, 4);
            var d = new byte[dl.ToInt()];
            ns.Read(d, 0, d.Length);

            pool.Enqueue(item);
            var responseMessage = d.ToObject<ResponseMessage>();

            return responseMessage;

        }


    }


}
