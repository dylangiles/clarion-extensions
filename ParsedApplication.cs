using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Globalization;
using Newtonsoft.Json;

namespace Redcat.TXA
{
    public class ParsedApplication
    {
        public enum ParseState
        {
            Application,
            Project,
            Program,
            Module,
            Procedure,
            ApplicationCommon,
            ApplicationPrompts,
            TemplateSymbol,
            MultiSymbolInstance,
            Embed
        }

        private string _name;
        public string Name => _name;

        private string _originalFilename;
        private List<string> _rawLines;
        private ParseState _state;

        private LegacyPRJ _legacyPRJ;
        public LegacyPRJ LegacyPRJ => _legacyPRJ;

        private ApplicationSection _applicationSection;
        public ApplicationSection ApplicationSection => _applicationSection;

        private ProgramSection _programSection;
        public ProgramSection ProgramSection => _programSection;

        private List<Module> _modules;
        public List<Module> Modules => _modules;

        private int _moduleIndex;
        public int ModuleIndex => _moduleIndex;

        private List<string> _assumedModules;
        public List<string> AssumedModules => _assumedModules;

        private List<ModuleSortOrder> _moduleSortOrder;

        // [APPLICATION]
        private int _version;
        private string _todoTemplate;
        private string _dictionaryFile;
        private string _firstProcedure;

        // [COMMON]
        private string _applicationTemplate;
        private DateTime _lastModified;

        private List<TemplateSymbol> _globalSymbols;
        private TemplateSymbol _currentSymbol;

        public ParsedApplication(string name)
        {
            _name = name;
            _rawLines = new List<string>();
            _globalSymbols = new List<TemplateSymbol>();
            _modules = new List<Module>();
            _assumedModules = new List<string>();
            _moduleSortOrder = new List<ModuleSortOrder>();
        }

        public void Parse(string filename)
        {
            
            using(FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                using(StreamReader sr = new StreamReader(fs))
                {
                    while(!sr.EndOfStream)
                        _rawLines.Add(sr.ReadLine());   
                }
            }

            List<string> rawApplication = new List<string>();
            List<string> rawPrj = new List<string>();
            List<string> rawProgramSection = new List<string>();
            List<string> currentModule = new List<string>();
            bool buildingModuleList = false;

            for (int i = 0; i < _rawLines.Count; i++)
            {
                switch(_rawLines[i])
                {
                    case Constants.SYMBOL_APPLICATION:
                        _state = ParseState.Application;
                        break;

                    case Constants.SYMBOL_PROJECT:
                        _state = ParseState.Project;
                        break;

                    case Constants.SYMBOL_PROGRAM:
                        if (_state == ParseState.Project)
                        {
                            // Do not include the [PROJECT] symbol in the project section
                            if (rawPrj[0] == Constants.SYMBOL_PROJECT)
                                rawPrj.RemoveAt(0);

                            _legacyPRJ = new LegacyPRJ(rawPrj.ToArray());
                            rawPrj.Clear();
                            rawPrj = null;

                            _applicationSection = new ApplicationSection(rawApplication.ToArray());
                            rawApplication.Clear();
                            rawApplication = null;
                        }
                         
                        _state = ParseState.Program;
                        break;
                        
                    case Constants.SYMBOL_MODULE:
                        if(_state == ParseState.Program)
                        {
                            _programSection = new ProgramSection(rawProgramSection.ToArray());
                            rawProgramSection.Clear();
                            rawProgramSection = null;
                        }

                        else if (_state == ParseState.Module)
                        {
                            // Modules.Count + 1 because the module hasn't been added yet
                            _modules.Add(new Module(currentModule.ToArray(), this, Modules.Count + 1));

                            AddLastModuleToSortOrder();

                            currentModule.Clear();
                        }

                        _state = ParseState.Module;
                        break;

                    //case Constants.SYMBOL_PROCEDURE:
                    //    _state = ParseState.Procedure;
                    //    break;
                }

                if(buildingModuleList)
                {
                    if(_rawLines[i].Contains("WHEN  "))
                    {
                        string moduleName = _rawLines[i].Replace("WHEN  (\'", string.Empty);
                        int nextQuoteLocation = -1;
                        for(int j = 0; j < moduleName.Length; j++)
                        {
                            if (moduleName[j] == '\'')
                            {
                                nextQuoteLocation = j;
                                break;
                            }   
                        }

                        moduleName = moduleName.Substring(0, nextQuoteLocation);
                        _assumedModules.Add(moduleName);
                    }
                }

                if(_rawLines[i].Contains('%') && buildingModuleList)
                    buildingModuleList = false;

                switch (_state)
                {
                    case ParseState.Application: 
                    case ParseState.Project:
                        // Add line to application section if state is either [APPLICATION] or [PROJECT]
                        rawApplication.Add(_rawLines[i]);

                        // Only add line to project section if state is [PROJECT]
                        if (_state == ParseState.Project)
                            rawPrj.Add(_rawLines[i]);

                        break;

                    case ParseState.Program:
                        rawProgramSection.Add(_rawLines[i]);
                        break;

                    case ParseState.Module:
                        currentModule.Add(_rawLines[i]);
                        break;
                }

                if (_state == ParseState.Application)
                {
                    if (_rawLines[i].Contains("%GenerationCompleted"))
                        buildingModuleList = true;
                }
            }
            
            
            // Get the last module
            if(_state == ParseState.Module)
            {
                _modules.Add(new Module(currentModule.ToArray(), this, Modules.Count + 1));
                AddLastModuleToSortOrder();
            }
                
            

            // This is only here as a spot to whack a breakpoint
            Console.WriteLine();
        }

        public string GetPropertyString(string content, string property) 
        {
            if(content == null || !content.Contains(property))
                return null;
            string value = content.Replace(property, string.Empty);
            return value;

        }


        public void IncrementModuleIndex() => _moduleIndex += 1;

        public void DumpModuleSortOrder(string filename)
        {
            string json = JsonConvert.SerializeObject(_moduleSortOrder, Formatting.Indented);
            using (FileStream fs = File.Create(filename))
            {
                using (StreamWriter sw = new StreamWriter(fs))
                    sw.Write(json);
            }
        }

        private void AddLastModuleToSortOrder()
        {
            _moduleSortOrder.Add(
                new ModuleSortOrder {
                    Name = _modules.Last().Name,
                    Order = _moduleSortOrder.Count
                }
            );
        }
    }
}
