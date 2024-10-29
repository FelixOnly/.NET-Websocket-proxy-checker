using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProxyParser.Interfaces
{
    internal class SettingsFile
    {
        public int ThreadCount { get; set; }
        public string MyIp { get; set; }
        public string IPsFilePath { get; set; }
        public string BANsFilePath { get; set; }

    }
}
