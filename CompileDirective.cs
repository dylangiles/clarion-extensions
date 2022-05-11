using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Redcat.TXA
{
    public class CompileDirective
    {
        public string Name { get; private set; }
        public bool Generated { get; private set; }

        public CompileDirective(string directive)
        {
            string temp = directive.Replace(Constants.PRJ_COMPILE, string.Empty);
            string[] parts = temp.Split(' ');
            Name = parts[0].Replace("\"", string.Empty);
            Generated = temp.Contains(Constants.PRJ_GENERATED);
        }
    }
}
