using ProxyParser.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProxyParser.Bridge.Reader.Abstraction
{
    public class ContentReader
    {
        protected IListReader _reader;

        public ContentReader(IListReader reader)
        {
            _reader = reader;
        }

        public virtual IEnumerable<AdressElement> GetAdresses(string link)
        {
            return _reader.Read(link);
        }
    }
}
