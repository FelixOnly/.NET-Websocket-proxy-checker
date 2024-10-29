using ProxyParser.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProxyParser.Bridge.ProxyChecker.Abstraction
{
    public interface IProxyChecker
    {
        public Task<RequestResponce> IsProxy(AdressElement adress, SearchFilter filter);
    }
}
