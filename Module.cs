using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Redcat.TXA
{
    public class Module
    {
        public string Name { get; private set; }
        public List<Procedure> Procedures { get; private set; }

        private string[] _raw;

        public Module(string[] raw)
        {
            _raw = raw;
            Procedures = new List<Procedure>();

            List<string> currentProcedure = new List<string>();
            
            bool hitProcedure = false;
            for(int i = 0; i< raw.Length; i++)
            {
                if(raw[i] == Constants.SYMBOL_PROCEDURE)
                {
                    if(!hitProcedure) 
                    {
                        hitProcedure = true;
                        continue;
                    }
                    else
                    {
                        Procedures.Add(new Procedure(currentProcedure.ToArray()));
                        currentProcedure.Clear();
                    }
                    
                }
                
                if(hitProcedure)
                    currentProcedure.Add(raw[i]);
            }
        }
    }
}
