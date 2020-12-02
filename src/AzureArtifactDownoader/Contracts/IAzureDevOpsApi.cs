using System.Threading.Tasks;
using AzureArtifactDownoader.Model;
using Refit;

namespace AzureArtifactDownoader.Contracts
{
    public interface IAzureDevOpsApi
    {
        [Get("/build/builds/")]
        Task<BuildsCollection> GetBuildsAsync();

        [Get("/build/builds/{buildId}/artifacts")]
        Task<ArtifactsCollection> GetArtifactInfoAsync(long buildId);
    }
}