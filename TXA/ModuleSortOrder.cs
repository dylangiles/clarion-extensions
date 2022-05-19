using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClarionExtensions.TXA
{
    public class ModuleSortOrder
    {
        public string Name { get; set; }
        public int Order { get; set; }

        [JsonConstructor]
        public ModuleSortOrder() { }
    }
}
