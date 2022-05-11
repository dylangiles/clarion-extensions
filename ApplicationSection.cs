using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Redcat.TXA
{
    public class ApplicationSection
    {
        private string[] _raw;
        public string[] Raw => _raw;

        public ApplicationSection(string[] raw)
        {
            _raw = raw;
        }
    }
}
