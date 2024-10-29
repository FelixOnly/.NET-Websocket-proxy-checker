using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ProxyParser.Interfaces
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum AnonymityLevel
    {
        None,
        Transparent,
        Anonymin,
        Elite
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum ProtocolType
    {
        HTTP,
        HTTPS,
        SOCKS4,
        SOCKS5
    }

    public class AdressElement
    {
        public AdressElement
            (IPAddress address, int port) 
        {
            Address = address;
            Port = port;
        }

        public IPAddress Address;
        public int Port;

        public string GetRaw => $"{Address}:{Port}";
    }

    public class AdressRecord
    {
        public AdressRecord(AdressElement adress, float speed)
        {
            Adress = adress;
            Speed = speed;
        }

        public AdressElement Adress;
        public float Speed;
        public string Country;
        public ProtocolType Protocol;
        public AnonymityLevel Anonymous;

        public bool HasBanned;
        public List<string> Websites = new List<string>();

    }
}
