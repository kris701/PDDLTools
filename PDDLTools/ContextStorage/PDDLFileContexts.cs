using PDDLParser;
using PDDLParser.Helpers;
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
        private static int TryDelay = 2;
        private static int RetryTimes = 3;
        private static Dictionary<string, DateTime> _domainTryTimes = new Dictionary<string, DateTime>();
        private static Dictionary<string, DateTime> _domainTimes = new Dictionary<string, DateTime>();
        private static Dictionary<string, DomainDecl> _domains = new Dictionary<string, DomainDecl>();
        private static Dictionary<string, DateTime> _problemTryTimes = new Dictionary<string, DateTime>();
        private static Dictionary<string, DateTime> _problemTimes = new Dictionary<string, DateTime>();
        private static Dictionary<string, ProblemDecl> _problems = new Dictionary<string, ProblemDecl>();

        public static PDDLDecl TryGetContextForFile(string fileName)
        {
            try
            {
                var returnDecl = new PDDLDecl(null, null);
                if (PDDLHelper.IsFileDomain(fileName))
                    returnDecl.Domain = GetDomainContextForFile(fileName);
                if (PDDLHelper.IsFileProblem(fileName))
                    returnDecl.Problem = GetProblemContextForFile(fileName);
                return returnDecl;
            }
            catch
            {
                return null;
            }
        }

        public static async Task<PDDLDecl> TryGetContextForFileAsync(string fileName)
        {
            var returnDecl = new PDDLDecl(null, null);
            try
            {
                if (PDDLHelper.IsFileDomain(fileName))
                    returnDecl.Domain = GetDomainContextForFile(fileName);
                if (PDDLHelper.IsFileProblem(fileName))
                    returnDecl.Problem = GetProblemContextForFile(fileName);

                int tries = 0;
                while (returnDecl.Domain == null && returnDecl.Problem == null)
                {
                    await Task.Delay(1000);
                    if (PDDLHelper.IsFileDomain(fileName))
                        returnDecl.Domain = GetDomainContextForFile(fileName);
                    if (PDDLHelper.IsFileProblem(fileName))
                        returnDecl.Problem = GetProblemContextForFile(fileName);
                    tries++;
                    if (tries > RetryTimes)
                        break;
                }
            }
            catch { }
            return returnDecl;
        }

        private static DomainDecl GetDomainContextForFile(string fileName)
        {
            UpdateDomainContextIfNeeded(fileName);
            if (!_domains.ContainsKey(fileName))
                return null;
            return _domains[fileName];
        }

        private static void UpdateDomainContextIfNeeded(string file)
        {
            if (!_domainTryTimes.ContainsKey(file))
                _domainTryTimes.Add(file, DateTime.Now);

            if ((DateTime.Now - _domainTryTimes[file]).Seconds > TryDelay)
            {
                _domainTryTimes[file] = DateTime.Now;
                if (_domains.ContainsKey(file))
                {
                    var lastWriteDate = new FileInfo(file).LastWriteTime;
                    if (lastWriteDate > _domainTimes[file])
                    {
                        IPDDLParser parser = new PDDLParser.PDDLParser(true, false);
                        var result = parser.Parse(file);
                        _domains[file] = result.Domain;
                        _domainTimes[file] = lastWriteDate;
                    }
                }
                else
                {
                    IPDDLParser parser = new PDDLParser.PDDLParser(true, false);
                    var result = parser.Parse(file);
                    _domains.Add(file, result.Domain);
                    _domainTimes.Add(file, new FileInfo(file).LastWriteTime);
                }
            }
        }

        private static ProblemDecl GetProblemContextForFile(string fileName)
        {
            UpdateProblemContextIfNeeded(fileName);
            if (!_problems.ContainsKey(fileName))
                return null;
            return _problems[fileName];
        }

        private static void UpdateProblemContextIfNeeded(string file)
        {
            if (!_problemTryTimes.ContainsKey(file))
                _problemTryTimes.Add(file, DateTime.Now);

            if ((DateTime.Now - _problemTryTimes[file]).Seconds > TryDelay)
            {
                _problemTryTimes[file] = DateTime.Now;
                if (_problems.ContainsKey(file))
                {
                    var lastWriteDate = new FileInfo(file).LastWriteTime;
                    if (lastWriteDate > _problemTimes[file])
                    {
                        IPDDLParser parser = new PDDLParser.PDDLParser(true, false);
                        var result = parser.Parse(null, file);
                        _problems[file] = result.Problem;
                        _problemTimes[file] = lastWriteDate;
                    }
                }
                else
                {
                    IPDDLParser parser = new PDDLParser.PDDLParser(true, false);
                    var result = parser.Parse(null, file);
                    _problems.Add(file, result.Problem);
                    _problemTimes.Add(file, new FileInfo(file).LastWriteTime);
                }
            }
        }
    }
}
