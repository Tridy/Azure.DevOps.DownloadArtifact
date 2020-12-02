using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using AzureArtifactDownoader.Contracts;
using Refit;

namespace AzureArtifactDownoader
{
    public static class Settings
    {
        public static string PersonalAccessToken = "PAT from Azure DevOps"; // todo: fill in yours
        public static string CompanyName = "MyCompany"; // todo: fill in yours
        public static string ProjectName = "MyProject"; // todo: fill in yours
        public static string ExtractPath = "DestinationPath"; // todo: fill in yours
    }
    public class Program
    {
        private static HttpClient _client = null;
        private static IAzureDevOpsApi _api = null;
        private static long _latestBuildId = -1;
        private static string _artifactDownloadUrl = null;

        static async Task Main(string[] args)
        {
            LogInfo("Start retrieving artifact.");

            using (_client = GetHttpClient())
            {
                _api = RestService.For<IAzureDevOpsApi>(_client);

                await GetLatestBuildIdAsync().ConfigureAwait(false);
                await GetArtifactIdFromBuildIdAsync().ConfigureAwait(false);
                CreateExtractDirectoryIfNonExisting();
                await DownloadFile().ConfigureAwait(false);
            }

            LogInfo("Finished retrieving artifact.");

            Console.ReadKey();
        }

        private static HttpClient GetHttpClient()
        {
            var client = new HttpClient();

            client.BaseAddress = new Uri($@"https://dev.azure.com/{Settings.CompanyName}/{Settings.ProjectName}/_apis");

            client.DefaultRequestHeaders.Accept.Add(
                new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
                Convert.ToBase64String(
                    System.Text.Encoding.ASCII.GetBytes($"{""}:{Settings.PersonalAccessToken}")));

            return client;
        }

        private static async Task GetLatestBuildIdAsync()
        {
            var builds = await _api.GetBuildsAsync().ConfigureAwait(false);


            var latestBuild = builds.Builds.Where(b => b.Status == "completed" && b.Result == "succeeded")
                                                     .OrderByDescending(b => b.Id)
                                                     .FirstOrDefault();

            if (latestBuild == null)
            {
                throw new Exception("No builds were found.");
            }

            _latestBuildId = latestBuild.Id;

            LogInfo($"Latest build id: {_latestBuildId}");
        }

        private static async Task GetArtifactIdFromBuildIdAsync()
        {
            var artifacts = await _api.GetArtifactInfoAsync(_latestBuildId);

            if (artifacts.Count < 1)
            {
                throw new Exception("No artifacts found.");
            }

            if (artifacts.Count > 1)
            {
                throw new Exception("More than 1 artifact found.");
            }

            var artifact = artifacts.Artifacts.ElementAt(0);
            _artifactDownloadUrl = artifact.Details.DownloadUrl;

            LogInfo($"Artifact size: {artifact.Details.Properties.Size} Bytes, URL: {_artifactDownloadUrl}");
        }

        private static void CreateExtractDirectoryIfNonExisting()
        {
            if (!Directory.Exists(Settings.ExtractPath))
            {
                Directory.CreateDirectory(Settings.ExtractPath);
            }
        }

        private static async Task DownloadFile()
        {
            LogInfo($"Started downloading artifact.");

            var response = await _client.GetAsync(_artifactDownloadUrl);

            LogInfo($"Finished downloading artifact.");

            LogInfo($"Extracting artifact.");

            using (var ms = new MemoryStream())
            {
                await response.Content.CopyToAsync(ms).ConfigureAwait(false);

                using (ZipArchive archive = new(ms))
                {
                    archive.ExtractToDirectory(Settings.ExtractPath, overwriteFiles: true);
                }
            }

            LogInfo($"Artifact extracted.");
        }

        private static void LogInfo(string text)
        {
            Console.WriteLine($"{DateTime.Now}: {text}");
        }
    }
}