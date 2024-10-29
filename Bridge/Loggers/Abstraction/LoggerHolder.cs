using ProxyParser.Bridge.ProxyChecker.Abstraction;
using ProxyParser.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProxyParser.Bridge.Loggers.Abstraction
{
    public class LoggerHolder : ILogger
    {

        protected ILogger _logger;

        public LoggerHolder(ILogger logger)
        {
            _logger = logger;
        }

        public virtual string GetHistory()
        {
            return _logger.GetHistory();
        }

        public virtual void Error(string message)
        {
            _logger.Error(message);
        }

        public virtual void Info(string message)
        {
            _logger.Info(message);
        }

        public virtual void Success(string message)
        {
            _logger.Success(message);
        }

        public virtual void Warning(string message)
        {
            _logger.Warning(message);
        }
    }
}
