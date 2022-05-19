using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClarionExtensions.TXA
{
    public class Procedure
    {
        public string Name { get; private set; }
        public string Prototype { get; private set; }
        public string Parameters { get; private set; }
        public bool NoExport { get; private set; }
        
        private string[] _raw;
        public string[] Raw => _raw;



        private bool _declaredGlobally;
        public bool DeclaredGlobally => _declaredGlobally;


        public Procedure(string[] raw)
        {
            _raw = raw;

            // [PROCEDURE]
            // NAME applyStyle
            Name = raw[1].Replace(Constants.PROPERTY_NAME, string.Empty);      
            
            for(int i = 0; i < raw.Length; i++)
            {
                if(raw[i].Contains(Constants.PROPERTY_PROTOTYPE))
                    Prototype = raw[i].Replace(Constants.PROPERTY_PROTOTYPE, string.Empty).Replace("\'", string.Empty);

                if (raw[i].Contains(Constants.PROPERTY_PARAMETERS))
                    Parameters = raw[i].Replace(Constants.PROPERTY_PARAMETERS, string.Empty).Replace("\'", string.Empty);

                if (raw[i] == Constants.PROPERTY_GLOBAL)
                    _declaredGlobally = true;

                if (raw[i] == Constants.PROPERTY_NOEXPORT)
                    NoExport = true;

                if (raw[i] == Constants.SYMBOL_EMBED)
                    break;
            }
        }
    }
}
