using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocationSpreader
{
    public static class LocationSpreaderBuilder
    {
        public static List<string> Spreaders => _spreaders.Keys.ToList();
        private static Dictionary<string, ILocationSpreader> _spreaders = new Dictionary<string, ILocationSpreader>()
        {
            { "Random No Overlap", new RandomPathCorrectingSpreader() },
            { "Random", new RandomSpreader() },
        };

        public static ILocationSpreader GetSpreader(string id) => _spreaders[id];
    }
}
