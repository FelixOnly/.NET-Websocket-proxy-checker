using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProxyParser.Bridge.Loggers.Abstraction
{
    public interface ILogger
    {
        public void Info(string message);
        public void Error(string message);
        public void Warning(string message);
        public void Success(string message);
        public string GetHistory();
    }
}
