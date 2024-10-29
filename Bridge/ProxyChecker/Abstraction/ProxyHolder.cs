using ProxyParser.Bridge.Reader.Abstraction;
using ProxyParser.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProxyParser.Bridge.ProxyChecker.Abstraction
{
    public class ProxyHolder
    {
        protected IProxyChecker _checker;

        public ProxyHolder(IProxyChecker cheker)
        {
            _checker = cheker;
        }

        public virtual Task<RequestResponce> CheckProxy(AdressElement adress, SearchFilter filter)
        {
            return _checker.IsProxy(adress, filter);
        }

    }
}
