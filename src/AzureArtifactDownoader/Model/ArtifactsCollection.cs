using Newtonsoft.Json;
using System.Diagnostics;

namespace AzureArtifactDownoader.Model
{
    public class ArtifactsCollection
    {
        [JsonProperty("count")]
        public long Count { get; set; }

        [JsonProperty("value")]
        public Artifact[] Artifacts { get; set; }

        [DebuggerDisplay("{Name} Size:{Details.Properties.Size} Bytes")]
        public class Artifact
        {           
            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("resource")]
            public Resources Details { get; set; }

            public class Resources
            {
                [JsonProperty("properties")]
                public ResourceProperties Properties { get; set; }

                [JsonProperty("downloadUrl")]
                public string DownloadUrl { get; set; }
                public class ResourceProperties
                {
                    [JsonProperty("artifactsize")]
                    public long Size { get; set; }
                }
            }
        }
    }
}