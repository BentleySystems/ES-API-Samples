namespace EsApiProjectsSampleApp
{
    public record Configuration(
        Uri ServiceHost,
        string ProjectType = ProjectTypes.ProjectWise,
        // TODO: Remove. Waiting for ProjectType refactor
        string ProvisionProjectType = "OpenConstruction");
}