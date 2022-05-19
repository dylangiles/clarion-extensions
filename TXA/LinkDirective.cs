using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClarionExtensions.TXA
{
    public class LinkDirective
    {
        public string Name { get; private set; }
        public bool Generated { get; private set; }

        public LinkDirective(string directive)
        {
            string temp = directive.Replace(Constants.PRJ_PRAGMA + " link(\"", string.Empty);
            temp = temp.Replace("\")", string.Empty);
            Generated = temp.Contains(Constants.PRJ_GENERATED);
            
            if (Generated)
                temp = temp.Replace(Constants.PRJ_GENERATED, string.Empty);
            
            Name = temp;
            
        }
    }
}
