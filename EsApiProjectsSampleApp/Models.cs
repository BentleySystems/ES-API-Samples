namespace EsApiProjectsSampleApp
{
    public record PagedResponse<T>(T[] Items, string? NextPageToken);

    public record Template(Guid Id, string DisplayName, string Number);

    public record Project(Guid Id, string DisplayName, string Number);

    public record CreateProject
    {
        public Guid TemplateId { get; init; }
        public string DisplayName { get; init; } = string.Empty;
        public string Number { get; init; } = string.Empty;
        public string DataCenterLocation { get; init; } = string.Empty;
        public string BillingCountry { get; init; } = string.Empty;
    }

    public record Provision(Guid ProjectId, Guid ProvisionId);

    public record ProvisionStatus(Guid Id, Guid TemplateId, Guid ProjectId, string State);
}
