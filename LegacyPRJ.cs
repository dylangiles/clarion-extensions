using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Redcat.TXA
{
    public class LegacyPRJ
    {
        public string System { get; private set; }
        public string ArtifactType { get; private set; }
        public string Language { get; private set; }
        public List<string> Pragmas { get; private set; }
        public List<CompileDirective> CompileDirectives { get; private set; }
        public string Artifact { get; private set; }

        private string[] _raw;

        public LegacyPRJ(string[] prjLines)
        {
            _raw = prjLines;

            Pragmas = new List<string>();
            CompileDirectives = new List<CompileDirective>();

            string valueConstruct = string.Empty;
            string[] parts;
            for(int i = 0; i< prjLines.Length; i++)
            {
                if(prjLines[i].Contains(Constants.PRJ_SYSTEM))
                {
                    valueConstruct = prjLines[i];
                    valueConstruct = valueConstruct.Replace(Constants.PRJ_SYSTEM, string.Empty);
                    parts = valueConstruct.Split(' ');
                    System = parts[0];
                    ArtifactType = parts[1];
                }

                else if(prjLines[i].Contains(Constants.PRJ_MODEL))
                {
                    valueConstruct = prjLines[i];
                    valueConstruct = valueConstruct.Replace(Constants.PRJ_SYSTEM, string.Empty);
                    parts = valueConstruct.Split(' ');
                    Language = parts[0];
                }

                else if (prjLines[i].Contains(Constants.PRJ_PRAGMA))
                {
                    valueConstruct = prjLines[i];
                    valueConstruct = valueConstruct.Replace(Constants.PRJ_PRAGMA, string.Empty);
                    parts = valueConstruct.Split(' ');
                    Pragmas.Add(parts[0]);
                }

                else if(prjLines[i].Contains(Constants.PRJ_COMPILE))
                {
                    CompileDirectives.Add(new CompileDirective(prjLines[i]));
                }
            }
        }
    }
}
