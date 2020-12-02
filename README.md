# Azure.DevOps.DownloadArtifact
an example of how to download an artifact from Azure DevOps

## what it does:

- finds the latest build for the specified project
- gets the artifact that was published from the build
- downloads the artifact
- extracts the content of the artifact into the specified directory

the models used in this example do not use all the properties of the objects that API provides/returns; only the properties that are needed for this example to functions are used.

## setup:

there are several variables that need to be populated in the Settings class, which include AzureDevOps PAT, Company and Project names as well as the path to where the artifact will be extracted.

```c#
public static class Settings
{
    public static string PersonalAccessToken = "PAT from Azure DevOps"; // todo: fill in yours
    public static string CompanyName = "MyCompany"; // todo: fill in yours
    public static string ProjectName = "MyProject"; // todo: fill in yours
    public static string ExtractPath = "DestinationPath"; // todo: fill in yours
}
```
## dependencies/helpers:

[ReFit](https://github.com/reactiveui/refit) REST library is used for calling Azure DevOps API 