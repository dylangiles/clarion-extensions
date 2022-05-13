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

        private bool _declaredGlobally;
        public bool DeclaredGlobally => _declaredGlobally;


        public Procedure(string[] raw)
        {
            _raw = raw;

            for(int i = 0; i < raw.Length; i++)
            {
                if(raw[i].Contains(Constants.PROPERTY_NAME))
                    Name = raw[i].Replace(Constants.PROPERTY_NAME, string.Empty);

                if (raw[i] == Constants.PROPERTY_GLOBAL)
                    _declaredGlobally = true;

                if (raw[i] == Constants.SYMBOL_EMBED)
                    break;
            }
        }
    }
}
