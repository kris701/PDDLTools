using PDDLParser;
using PDDLParser.Models;
using PDDLParser.Models.Domain;
using PDDLParser.Models.Problem;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLTools.ContextStorage
{
    public static class PDDLFileContexts
    {
        private static Dictionary<string, DateTime> _domainTimes = new Dictionary<string, DateTime>();
        private static Dictionary<string, DomainDecl> _domains = new Dictionary<string, DomainDecl>();
        private static Dictionary<string, DateTime> _problemTimes = new Dictionary<string, DateTime>();
        private static Dictionary<string, ProblemDecl> _problems = new Dictionary<string, ProblemDecl>();

        public static DomainDecl GetDomainContextForFile(string fileName)
        {
            UpdateDomainContextIfNeeded(fileName);
            return _domains[fileName];
        }

        private static void UpdateDomainContextIfNeeded(string file)
        {
            if (_domains.ContainsKey(file))
            {
                var lastWriteDate = new FileInfo(file).LastWriteTime;
                if (lastWriteDate > _domainTimes[file])
                {
                    IPDDLParser parser = new PDDLParser.PDDLParser();
                    var result = parser.ParseDomainFile(file);
                    _domains[file] = result;
                    _domainTimes[file] = lastWriteDate;
                }
            }
            else
            {
                IPDDLParser parser = new PDDLParser.PDDLParser();
                var result = parser.ParseDomainFile(file);
                _domains.Add(file, result);
                _domainTimes.Add(file, new FileInfo(file).LastWriteTime);
            }
        }

        public static ProblemDecl GetProblemContextForFile(string fileName)
        {
            UpdateProblemContextIfNeeded(fileName);
            return _problems[fileName];
        }

        private static void UpdateProblemContextIfNeeded(string file)
        {
            if (_problems.ContainsKey(file))
            {
                var lastWriteDate = new FileInfo(file).LastWriteTime;
                if (lastWriteDate > _problemTimes[file])
                {
                    IPDDLParser parser = new PDDLParser.PDDLParser();
                    var result = parser.ParseProblemFile(file);
                    _problems[file] = result;
                    _problemTimes[file] = lastWriteDate;
                }
            }
            else
            {
                IPDDLParser parser = new PDDLParser.PDDLParser();
                var result = parser.ParseProblemFile(file);
                _problems.Add(file, result);
                _problemTimes.Add(file, new FileInfo(file).LastWriteTime);
            }
        }
    }
}
