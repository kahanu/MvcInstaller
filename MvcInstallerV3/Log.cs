﻿using System;
using System.IO;
using System.Linq;

namespace MvcInstaller
{
    public class Log
    {
        public static void Exception(Exception ex)
        {
            string _fileName;
            string logsDir = AppDomain.CurrentDomain.BaseDirectory + @"\MvcInstaller\Logs";

            DirectoryInfo di = new DirectoryInfo(logsDir);
            if (!di.Exists)
                di.Create();

            _fileName = logsDir + @"\Errors-" + string.Format("{0:yyyy-MM-dd}", DateTime.Now) + ".log";
            string msg = "[" + DateTime.Now.ToString() + "] Source: " + ex.Source + " - Message: " + ex.Message;
                if (ex != null)
                {
                    msg += Environment.NewLine + "   Message:          " + ex.Message;
                    msg += Environment.NewLine + "   StackTrace:       " + ex.StackTrace;
                    msg += Environment.NewLine + "   Target Site:      " + ex.TargetSite.ToString();
                    msg += Environment.NewLine + "   Exception Source: " + ex.Source;
                    msg += Environment.NewLine;
                }

            if (ex == null)
            {
                FileWriter writer = new FileWriter(_fileName);
                writer.Write(msg);
            }
        }
    }
}
