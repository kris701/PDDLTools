using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PDDLParser.SystemTests
{
    [TestClass]
    public class PDDLParserTests
    {
        private static long MaxFileSize = 10000;
        private static Dictionary<string, List<string>> _testDict = new Dictionary<string, List<string>>();
        private static IEnumerable<object[]> GetDictionaryData()
        {
            foreach (var key in _testDict.Keys)
                yield return new object[] { key, _testDict[key] };
        }
        [ClassInitialize]
        public static async Task InitialiseAsync(TestContext context)
        {
            await BenchmarkFetcher.CheckAndDownloadBenchmarksAsync();
            List<string> validDomains = new List<string>();
            foreach (var path in Directory.GetDirectories(BenchmarkFetcher.OutputPath))
            {
                if (path.EndsWith("-strips"))
                    validDomains.Add(path);
            }
            foreach (var domainPath in validDomains)
            {
                var domainFile = Path.Combine(domainPath, "domain.pddl");
                if (File.Exists(domainFile))
                {
                    if (!_testDict.ContainsKey(domainFile))
                    {
                        _testDict.Add(domainFile, new List<string>());
                        foreach (var problem in Directory.GetFiles(domainPath))
                        {
                            if (problem != domainFile && problem.EndsWith(".pddl"))
                                _testDict[domainFile].Add(problem);
                        }
                    }
                }
            }
        }

        [TestMethod]
        [DynamicData(nameof(GetDictionaryData), DynamicDataSourceType.Method)]
        public void Can_ParseDomains_ParseOnly_STRIPS(string domain, List<string> problems)
        {
            System.Diagnostics.Trace.WriteLine($"Domain: {new FileInfo(domain).Directory.Name}, problems: {problems.Count}");

            // ARRANGE
            IPDDLParser parser = new PDDLParser(false, false);
            parser.Listener.ThrowIfTypeAbove = Listener.ParseErrorType.Warning;
            if (!parser.IsDomainRequirementsSupported(domain))
                Assert.Inconclusive("Contains unsupported packages!");
            
            // ACT
            parser.Parse(domain);

            // ASSERT
            Assert.IsFalse(parser.Listener.Errors.Any(x => x.Type == Listener.ParseErrorType.Error));
        }

        [TestMethod]
        [DynamicData(nameof(GetDictionaryData), DynamicDataSourceType.Method)]
        public void Can_ParseDomains_Contextualize_STRIPS(string domain, List<string> problems)
        {
            System.Diagnostics.Trace.WriteLine($"Domain: {new FileInfo(domain).Directory.Name}, problems: {problems.Count}");

            // ARRANGE
            IPDDLParser parser = new PDDLParser(true, false);
            parser.Listener.ThrowIfTypeAbove = Listener.ParseErrorType.Warning;
            if (!parser.IsDomainRequirementsSupported(domain))
                Assert.Inconclusive("Contains unsupported packages!");

            // ACT
            parser.Parse(domain);

            // ASSERT
            Assert.IsFalse(parser.Listener.Errors.Any(x => x.Type == Listener.ParseErrorType.Error));
        }

        [TestMethod]
        [DynamicData(nameof(GetDictionaryData), DynamicDataSourceType.Method)]
        public void Can_ParseDomains_Analyse_STRIPS(string domain, List<string> problems)
        {
            System.Diagnostics.Trace.WriteLine($"Domain: {new FileInfo(domain).Directory.Name}, problems: {problems.Count}");

            // ARRANGE
            IPDDLParser parser = new PDDLParser(true, true);
            parser.Listener.ThrowIfTypeAbove = Listener.ParseErrorType.Warning;
            if (!parser.IsDomainRequirementsSupported(domain))
                Assert.Inconclusive("Contains unsupported packages!");

            // ACT
            parser.Parse(domain);

            // ASSERT
            Assert.IsFalse(parser.Listener.Errors.Any(x => x.Type == Listener.ParseErrorType.Error));
        }

        [TestMethod]
        [DynamicData (nameof(GetDictionaryData), DynamicDataSourceType.Method)]
        public void Can_ParseProblems_ParseOnly_STRIPS(string domain, List<string> problems)
        {
            System.Diagnostics.Trace.WriteLine($"Domain: {new FileInfo(domain).Directory.Name}, problems: {problems.Count}");

            // ARRANGE
            IPDDLParser parser = new PDDLParser(false, false);
            parser.Listener.ThrowIfTypeAbove = Listener.ParseErrorType.Warning;
            if (!parser.IsDomainRequirementsSupported(domain))
                Assert.Inconclusive("Contains unsupported packages!");
            Random rnd = new Random();

            // ACT
            foreach (var problem in problems.OrderBy(x => rnd.Next()))
            {
                if (new FileInfo(problem).Length < MaxFileSize)
                {
                    parser.TryParse(null, problem);
                    Assert.IsFalse(parser.Listener.Errors.Any(x => x.Type == Listener.ParseErrorType.Error));
                    parser.Listener.Errors.Clear();
                }
            }

            // ASSERT
            Assert.IsFalse(parser.Listener.Errors.Any(x => x.Type == Listener.ParseErrorType.Error));
        }

        [TestMethod]
        [DynamicData(nameof(GetDictionaryData), DynamicDataSourceType.Method)]
        public void Can_ParseProblems_Contextualise_STRIPS(string domain, List<string> problems)
        {
            System.Diagnostics.Trace.WriteLine($"Domain: {new FileInfo(domain).Directory.Name}, problems: {problems.Count}");

            // ARRANGE
            IPDDLParser parser = new PDDLParser(true, false);
            parser.Listener.ThrowIfTypeAbove = Listener.ParseErrorType.Warning;
            if (!parser.IsDomainRequirementsSupported(domain))
                Assert.Inconclusive("Contains unsupported packages!");
            Random rnd = new Random();

            // ACT
            foreach (var problem in problems.OrderBy(x => rnd.Next()))
            {
                if (new FileInfo(problem).Length < MaxFileSize)
                {
                    parser.TryParse(null, problem);
                    Assert.IsFalse(parser.Listener.Errors.Any(x => x.Type == Listener.ParseErrorType.Error));
                    parser.Listener.Errors.Clear();
                }
            }

            // ASSERT
            Assert.IsFalse(parser.Listener.Errors.Any(x => x.Type == Listener.ParseErrorType.Error));
        }

        [TestMethod]
        [DynamicData(nameof(GetDictionaryData), DynamicDataSourceType.Method)]
        public void Can_ParseProblems_Analyse_STRIPS(string domain, List<string> problems)
        {
            System.Diagnostics.Trace.WriteLine($"Domain: {new FileInfo(domain).Directory.Name}, problems: {problems.Count}");

            // ARRANGE
            IPDDLParser parser = new PDDLParser(true, true);
            parser.Listener.ThrowIfTypeAbove = Listener.ParseErrorType.Warning;
            if (!parser.IsDomainRequirementsSupported(domain))
                Assert.Inconclusive("Contains unsupported packages!");
            Random rnd = new Random();

            // ACT
            foreach (var problem in problems.OrderBy(x => rnd.Next()))
            {
                if (new FileInfo(problem).Length < MaxFileSize)
                {
                    parser.TryParse(null, problem);
                    Assert.IsFalse(parser.Listener.Errors.Any(x => x.Type == Listener.ParseErrorType.Error));
                    parser.Listener.Errors.Clear();
                }
            }

            // ASSERT
            Assert.IsFalse(parser.Listener.Errors.Any(x => x.Type == Listener.ParseErrorType.Error));
        }
    }
}