using Newtonsoft.Json.Converters;
using System.Text.Json.Serialization;

namespace ProxyParser.Interfaces
{
    public class SearchFilter
    {
        public int Amount = 0;
        public string[]? Countries;
        [JsonConverter(typeof(StringEnumConverter))] public ProtocolType[]? Protocols;
        [JsonConverter(typeof(StringEnumConverter))] public AnonymityLevel[]? Levels;
        public string[]? Websites;
        public bool? Ban;

    }


    
}
