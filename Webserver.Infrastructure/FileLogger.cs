using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Webserver.Infrastructure
{
    public class FileLogger : ILogger
    {
        public void Log(string message)
        {
            if (File.Exists("Log.txt") == false)
            {
                File.WriteAllText("Log.txt", message);
            }
            else
            {
                File.AppendAllText("Log.txt", message);
            }
        }
    }
}
