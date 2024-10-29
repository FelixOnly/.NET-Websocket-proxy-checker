using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ProxyParser.Bridge.Geoiplocator.Abstraction
{
    public interface IGeoLocator
    {
        public string GetLocation(IPAddress ip);
    }
}
