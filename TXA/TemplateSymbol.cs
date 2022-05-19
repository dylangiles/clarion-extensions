using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClarionExtensions.TXA
{
    public class TemplateSymbol
    {
        public enum SymbolType
        {
            Default,
            Long,
            Real, 
            String,
            Unknown
        }

        public string Name { get; private set; }
        public SymbolType Type { get; private set; }
        public object Value { get; private set; }
        public object[] MultiValues { get; private set; }
        public string DependsOn { get; private set; }
        public bool IsMulti { get; private set; }
        public bool MultiIsUnique { get; private set; }

        private static readonly Dictionary<string, SymbolType> _typesMap = new Dictionary<string, SymbolType>();
        public TemplateSymbol(string content)
        {
            // %OverrideExternal DEPEND %File DEFAULT TIMES 213
            // WHEN('')('Use Default')
            //MultiValues = new Dictionary<object, object>();
            _typesMap.Add(Constants.TYPE_DEFAULT, SymbolType.Default);
            _typesMap.Add(Constants.TYPE_LONG, SymbolType.Long);
            _typesMap.Add(Constants.TYPE_REAL, SymbolType.Real);
            _typesMap.Add(Constants.TYPE_STRING, SymbolType.String);

            SymbolType tempType = SymbolType.Unknown;


            string[] lines = content.Split('\r', '\n');
            for(int i = 0; i < lines.Length; i++)
            {
                string[] properties = lines[i].Split(' ');
                Name = properties[0].Replace("%", "");

                for(int j = 1; j < properties.Length; j++)
                {
                    switch(properties[j])
                    {
                        case Constants.PROPERTY_UNIQUE:
                            MultiIsUnique = true;
                            break;
                        case Constants.PROPERTY_DEPEND:
                            DependsOn = properties[j + 1];
                            j += 1;
                            break;
                        case Constants.TYPE_DEFAULT:
                        case Constants.TYPE_LONG:
                        case Constants.TYPE_REAL:
                        case Constants.TYPE_STRING:
                            if (!_typesMap.TryGetValue(properties[j], out tempType))
                                Type = SymbolType.Unknown;
                            else
                                Type = tempType;
                            break;
                    }
                }
            }
        }

        public void AddMultiValueInstance(string content)
        {

        }
    }
}
