using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Redcat.TXA
{
    public class Procedure
    {
        public string Name { get; private set; }
        private string[] _raw;
        public string[] Raw => _raw;

        public Procedure(string[] raw)
        {
            _raw = raw;
        }
    }
}
