using Newtonsoft.Json;
using System.Diagnostics;

namespace AzureArtifactDownoader.Model
{
    public class BuildsCollection
    {
        [JsonProperty("count")]
        public long Count { get; set; }
        
        [JsonProperty("value")]
        public Build[] Builds { get; set; }

        [DebuggerDisplay("{BuildNumber}, Status:{Status}, Result:{Result}")]
        public class Build
        {
            [JsonProperty("id")]
            public long Id { get; set; }

            [JsonProperty("buildNumber")]
            public string BuildNumber { get; set; }

            [JsonProperty("status")]
            public string Status { get; set; }

            [JsonProperty("result")]
            public string Result { get; set; }
        }
    }
}