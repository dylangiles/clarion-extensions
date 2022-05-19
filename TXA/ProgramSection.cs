using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClarionExtensions.TXA
{
    public class ProgramSection
    {
        private string[] _raw;
        public string[] Raw => _raw;

        public ProgramSection(string[] raw)
        {
            _raw = raw;
        }
    }
}
