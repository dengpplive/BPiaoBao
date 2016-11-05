using AutoUpdate.MainService;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace AutoUpdate
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            if (args.Length < 2)
                return;
            string appName = args[0];
            string url = args[1];
            new Updater().CheckUpdateStatus(appName, url);
        }
    }
}
