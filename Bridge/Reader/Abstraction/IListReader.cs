using ProxyParser.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProxyParser.Bridge.Reader.Abstraction
{
    public interface IListReader
    {
        public IEnumerable<AdressElement> Read(string link);

    }
}
