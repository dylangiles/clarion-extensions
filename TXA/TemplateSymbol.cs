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
        public string Value { get; private set; }
        public string[] MultiValues { get; private set; }
        public string[] DependentValues { get; private set; }
        public string DependsOn { get; private set; }
        public bool IsMulti { get; private set; }
        public bool MultiIsUnique { get; private set; }
        public int Times { get; private set; }

        private static readonly Dictionary<string, SymbolType> _typesMap = new Dictionary<string, SymbolType>() 
        {
            { Constants.TYPE_DEFAULT, SymbolType.Default },
            { Constants.TYPE_LONG, SymbolType.Long },
            { Constants.TYPE_REAL, SymbolType.Real },
            { Constants.TYPE_STRING, SymbolType.String },
        };

        public TemplateSymbol(string content)
        {
            //%ClassItem UNIQUE DEFAULT  ('ErrorManager', 'ErrorStatusManager', 'FileManager:PLUClass', |
            // %OverrideExternal DEPEND %File DEFAULT TIMES 213
            // WHEN('')('Use Default')
            //MultiValues = new Dictionary<object, object>();

            SymbolType tempType = SymbolType.Unknown;


            string[] properties = null;
            //string[] values = null;
            string test = null;

            if(!content.Contains(Constants.PROPERTY_DEPEND) && 
                (content.Contains(Constants.PROPERTY_MULTI) || content.Contains(Constants.PROPERTY_UNIQUE)))
            {
                MultiValues = content
                    .Substring(content.IndexOf('(') + 1)
                    .TrimEnd(')')
                    .Replace("|", string.Empty)
                    .Replace("\'", string.Empty)
                    .Split(new char[] {',', ' '}, StringSplitOptions.RemoveEmptyEntries);

                properties = content
                    .Substring(0, content.IndexOf('(')).TrimEnd(' ')
                    .Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            }
            else if (content.Contains(Constants.PROPERTY_TIMES) && content.Contains(Constants.SYMBOL_WHEN))
            {
                properties = content
                    .Substring(0, content.IndexOf(Constants.SYMBOL_WHEN)).TrimEnd(' ')
                    .Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            }

            else if (content.Contains(Constants.PROPERTY_TIMES))
            {
                properties = content.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                Name = properties[0].Replace("%", string.Empty);
            }

            else
            {
                if(content.Contains('('))
                {
                    properties = content
                        .Substring(0, content.IndexOf('('))
                        .Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                }
                else
                {
                    properties = content.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                }
                
                Value = content
                    .Substring(content.IndexOf('(') + 1)
                    .Replace("(", string.Empty)
                    .Replace(")", string.Empty)
                    .Replace("'", string.Empty);
            }

            if(properties != null)
            {
                Name = properties[0].Replace("%", string.Empty);
            }
            else
            {
                return;
            }
            

            int symbolTimes = 0;
            for(int j = 1; j < properties.Length; j++)
            {
                switch(properties[j])
                {
                    case Constants.PROPERTY_UNIQUE:
                        MultiIsUnique = true;
                        IsMulti = true;
                        break;

                    case Constants.PROPERTY_MULTI:
                        IsMulti = true;
                        break;

                    case Constants.PROPERTY_DEPEND:
                        DependsOn = properties[j + 1].Replace("%", string.Empty);
                        j += 1;
                        break;

                    case Constants.PROPERTY_TIMES:
                        Times = int.TryParse(properties[j + 1], out symbolTimes) ? symbolTimes : 0;
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

            if(DependsOn != null && Times > 0)
            {
                ParseDependentValues(content.Substring(content.IndexOf(Constants.SYMBOL_WHEN)));
            }
        }

        public void ParseDependentValues(string multiValueString)
        {
            string[] items = multiValueString.Split(new string[] { Constants.SYMBOL_WHEN }, 
                StringSplitOptions.RemoveEmptyEntries);

            if (Times > 0)
            {
                DependentValues = new string[Times];
                MultiValues = new string[Times];
            }
                
            else
                return;


            for(int i = 0; i < Times; i++)    
            {
                string[] parts = items[i].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                DependentValues[i] = parts[0]
                    .Replace("(", string.Empty)
                    .Replace(")", string.Empty)
                    .Replace("'", string.Empty);

                MultiValues[i] = parts[1]
                    .Replace("(", string.Empty)
                    .Replace(")", string.Empty)
                    .Replace("'", string.Empty);
            }
        }

        public override string ToString() => IsMulti ? $"%{Name} => []" : $"%{Name} => {Value}";
    }
}
