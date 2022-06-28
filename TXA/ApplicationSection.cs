using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClarionExtensions.TXA
{
    public class ApplicationSection
    {
        private string[] _raw;
        public string[] Raw => _raw;

        private List<TemplateSymbol> _symbols;
        public List<TemplateSymbol> Symbols => _symbols;

        public ApplicationSection(string[] raw)
        {
            _raw = raw;
            _symbols = new List<TemplateSymbol>();
        }

        public void ParseSymbols()
        {
            StringBuilder sb = new StringBuilder();
            
            for (int i = 0; i < _raw.Length; i++)
            {
                string line = _raw[i];
                string previousLine = null;
                if(i > 0)
                {
                    previousLine = _raw[i - 1];
                }

                if(line.Length == 0)
                    continue;

                if (line[0] == '%')
                {
                    if(sb.Length > 0)
                    {
                        _symbols.Add(new TemplateSymbol(sb.ToString()));
                        sb.Clear();
                    }

                    sb.Append(line);
                }
                

                if (previousLine != null && previousLine.Length > 0 && previousLine[previousLine.Length - 1] == '|')
                {
                    sb.Append(line);
                }

                if(line.Substring(0, Constants.SYMBOL_WHEN.Length) == Constants.SYMBOL_WHEN)
                {
                    sb.Append(" ");
                    sb.Append(line);
                }
            }
        }
    }
}
