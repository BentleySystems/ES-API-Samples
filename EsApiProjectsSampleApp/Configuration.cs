namespace EsApiProjectsSampleApp
{
    public record Configuration(Uri ServiceHost, string ProjectType = ProjectTypes.ProjectWise);
}