using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using JoveZhao.Framework.Expand;

namespace JoveZhao.Framework.SOA
{
    /// <summary>
    /// 内部命令
    /// </summary>
    public class InsideCommand
    {
        private Dictionary<string, ICommandProcess> dic = new Dictionary<string, ICommandProcess>();
        private ICommandProcess defaultProcess=null;
        public ICommandProcess this[string cmd]
        {
            get
            {
                if (dic.ContainsKey(cmd))
                {
                    return dic[cmd];
                }
                else
                {
                    return defaultProcess;
                }

            }
        }
        public void Register(string cmd, ICommandProcess process)
        {
            dic.Add(cmd, process);
        }
        public bool HasCmd(string cmd)
        {
            return dic.ContainsKey(cmd);
        }
    }


    public class ProcessThread
    {
        private volatile bool  Running = false;
        private TcpClient client;
        private ServerConfigurationElement config;
        Thread thread;
        private InsideCommand insideCommand;
        public ProcessThread(TcpClient client, ServerConfigurationElement config)
        {
            this.client = client;
            this.config = config;
            this.insideCommand = new InsideCommand();
            this.thread = new Thread(process);

            initInsideCommand();

        }
        //初始化内部命令
        private void initInsideCommand() {
            this.insideCommand.Register("heart", new HeartProcess());
            this.insideCommand.Register("auth", new AuthProcess());
        }






        private void process()
        {
            var ns = client.GetStream();


            while (client.Connected && Running)
            {
                try
                {
                    if (ns.DataAvailable)
                    {
                        byte[] dl = new byte[4];
                        ns.Read(dl, 0, dl.Length);
                        byte[] d = new byte[dl.ToInt()];
                        ns.Read(d, 0, d.Length);
                        var request = d.ToObject<RequestMessage>();


                        ICommandProcess cmdProcess = null;
                        if (insideCommand.HasCmd(request.Cmd))
                        {
                            cmdProcess = insideCommand[request.Cmd];
                        }
                        else
                        {
                            cmdProcess = config.Commands[request.Cmd].Processor;
                        }

                        if (cmdProcess != null)
                        {
                            var response = cmdProcess.Execute(request);



                            var data = response.ToBytes();
                            var datal = data.Length.GetBytes();
                            ns.Write(datal, 0, datal.Length);
                            ns.Write(data, 0, data.Length);
                        }
                        else
                        {
                            Logger.WriteLog(LogType.INFO, "没有找到命令为" + request.Cmd + "的处理器");
                        }
                    }
                    else
                    {
                        Thread.Sleep(10);
                    }
                }
                catch (ThreadAbortException)
                {
                    Logger.WriteLog(LogType.INFO, "服务器关闭，连接停止");
                }
                catch (Exception ex)
                {

                    Logger.WriteLog(LogType.ERROR, "接受数据时出错:", ex);
                }
            }
            Logger.WriteLog(LogType.INFO, "连接关闭");

            ns.Close();
            client.Close();
        }

        public void Start()
        {
            Running = true;
            thread.Start();
        }
        public void Stop()
        {
            Running = false;
            thread.Abort();
        }
    }
}
