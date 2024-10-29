using ProxyParser.Bridge.Reader.Abstraction;
using ProxyParser.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ProxyParser.Bridge.Reader.Abstraction
{
    public class LocalReader : IListReader
    {
        public IEnumerable<AdressElement> Read(string link)
        {
            if(!File.Exists(link))
                return new List<AdressElement>();

            string data = File.ReadAllText(link);

            string adressPatern = "\\d+.\\d+.\\d+.\\d+";
            string portPatern = ":\\d+";
            
            
            MatchCollection ipmatches = Regex.Matches(data, adressPatern);
            MatchCollection portmatches = Regex.Matches(data, portPatern);

            List<AdressElement> adresses = new List<AdressElement>();

            for (int i = 0; i < ipmatches.Count; i++)
            {
                var ipmatch = ipmatches[i];
                var portmatch = portmatches[i];

                if (!ipmatch.Success || !portmatch.Success)
                    continue;

                IPAddress ip = IPAddress.Parse(ipmatch.Value);

                int port = Int32.Parse(portmatch.Value.Replace(":",""));

                adresses.Add(new AdressElement(ip, port));

            }

            return adresses;           
        }
    }
}
