/*---------------------------------------------------------------------------------------------
* Copyright (c) Bentley Systems, Incorporated. All rights reserved.
* See LICENSE.md in the project root for license terms and full copyright notice.
*--------------------------------------------------------------------------------------------*/
namespace EsApiProjectsSampleApp
{
    public record PagedResponse<T>(T[] Items, string? NextPageToken);

    public record Template(Guid Id, string DisplayName, string Number);

    public record Project(Guid Id, string DisplayName, string Number);

    public record CreateProject(Guid TemplateId, string DisplayName, string Number, string DataCenterLocation, string BillingCountry);

    public record Provision(Guid ProjectId, Guid ProvisionId);

    public record ProvisionStatus(Guid Id, Guid TemplateId, Guid ProjectId, /* Value from CopyState */ string State);

    public record CreateConnection(Uri Url, string DisplayName, string Description, string WorkAreaName);

    public record GetConnection(string Id, Uri Url, string Type, string DisplayName, string Description, string WorkAreaName);

    public class ProjectTypes
    {
        public const string ProjectWise = "ProjectWise";
        public const string Synchro = "SYNCHRO";
    }

    public class CopyState
    {
        public const string Created = "Created";
        public const string Queued = "Queued";
        public const string Started = "Started";
        public const string Succeeded = "Succeeded";
        public const string Failed = "Failed";
    }
}
