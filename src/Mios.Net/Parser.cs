using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Mios.Net
{
    public class Parser
    {
        private static JsonSerializerSettings _settings = new JsonSerializerSettings {
             Formatting = Formatting.Indented,
              
        };

        public SummaryData Parse(string json)
        {
            return JsonConvert.DeserializeObject<SummaryData>(json, _settings);
        }
    }
}
