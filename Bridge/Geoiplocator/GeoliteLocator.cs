using ProxyParser.Bridge.Geoiplocator.Abstraction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using MaxMind.Db;
using System.Collections;

namespace ProxyParser.Bridge.Geoiplocator
{
    public class GeoliteLocator : IGeoLocator
    {
        public string GetLocation(IPAddress ip)
        {
            using (var reader = new MaxMind.Db.Reader("GeoLite2-Country.mmdb"))
            {
                
                var data = reader.Find<Dictionary<string, Dictionary<string,object>>>(ip);
                
                if(data != null) 
                    return data["registered_country"]["iso_code"].ToString();
            }

            return string.Empty;
        }
    }
}
