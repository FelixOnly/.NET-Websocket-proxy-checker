using ProxyParser.Bridge.ProxyChecker.Abstraction;
using ProxyParser.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ProxyParser.Bridge.Geoiplocator.Abstraction
{
    public class LocatorHolder
    {
        protected IGeoLocator _locator;

        public LocatorHolder(IGeoLocator locator)
        {
            _locator = locator;
        }

        public virtual string Search(AdressElement adress)
        {
            return _locator.GetLocation(adress.Address);
        }
    }
}
