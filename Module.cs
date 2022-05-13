using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Redcat.TXA
{
    public class Module
    {
        public string Name { get; set; }
        public List<ProcedureSortOrder> SortOrder { get; set; }
        
        [JsonIgnore]
        public List<Procedure> Procedures { get; private set; }

        private string[] _raw;

        [JsonConstructor]
        public Module()
        {
            Procedures = new List<Procedure>();
            SortOrder = new List<ProcedureSortOrder>();
        }

        public Module(string[] raw, ParsedApplication refApplication, int moduleIndex)
        {
            _raw = raw;
            Procedures = new List<Procedure>();
            SortOrder = new List<ProcedureSortOrder>();
            List<string> currentProcedure = new List<string>();
            
            bool hitProcedure = false;
            for(int i = 0; i< raw.Length; i++)
            {
                if (!hitProcedure)
                {
                    // Module has a name override
                    if (raw[i].Contains(Constants.PROPERTY_NAME))
                        Name = raw[i].Replace(Constants.PROPERTY_NAME, string.Empty);
                    else
                        Name = refApplication.LegacyPRJ.CompileDirectives[moduleIndex].Name;
                }

                if (hitProcedure)
                {
                    // Make sure the first procedure doesn't rack the [PROCEDURE] symbol for the second                
                    if (raw[i] != Constants.SYMBOL_PROCEDURE)
                        currentProcedure.Add(raw[i]);
                }
                    

                if (raw[i] == Constants.SYMBOL_PROCEDURE)
                {
                    if(!hitProcedure) 
                    {
                        hitProcedure = true;
                        
                        // Fix for missing the [PROCEDURE] symbol in the procedure's content
                        currentProcedure.Add(raw[i]);

                        continue;
                    }
                    else
                    {
                        
                        Procedures.Add(new Procedure(currentProcedure.ToArray()));
                        ProcedureSortOrder sortOrder = new ProcedureSortOrder();
                        sortOrder.Name = Procedures.Last().Name;
                        sortOrder.Order = Procedures.Count - 1;
                        SortOrder.Add(sortOrder);
                        currentProcedure.Clear();
                        
                        // Fix for missing the [PROCEDURE] symbol in the NEXT procedure's content
                        currentProcedure.Add(raw[i]);
                    }
                }
            }

            if(currentProcedure.Count > 0)
            {
                Procedures.Add(new Procedure(currentProcedure.ToArray()));
                ProcedureSortOrder sortOrder = new ProcedureSortOrder();
                sortOrder.Name = Procedures.Last().Name;
                sortOrder.Order = Procedures.Count - 1;
                SortOrder.Add(sortOrder);
                currentProcedure.Clear();
            }
        }

        public void DumpSortOrder(string filename)
        {
            string json = JsonConvert.SerializeObject(this, Formatting.Indented);
            using (FileStream fs = File.Create(filename))
            {
                using (StreamWriter sw = new StreamWriter(fs))
                    sw.Write(json);
            }
        }
    }
}
