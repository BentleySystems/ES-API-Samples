/*---------------------------------------------------------------------------------------------
* Copyright (c) Bentley Systems, Incorporated. All rights reserved.
* See LICENSE.md in the project root for license terms and full copyright notice.
*--------------------------------------------------------------------------------------------*/
namespace EsApiProjectsSampleApp
{
    public record PagedResponse<T>(T[] Items, string? NextPageToken);

    public record Template(Guid Id, string DisplayName, string Number, string RegisteredDate, string DatacenterLocation);

    public record Project(Guid Id, string DisplayName, string Number, string RegisteredDate, string BillingCountry, string? ProjectType, string DatacenterLocation, string TimeZone, bool IsTemplate, string GeographicLocation, double Latitude, double Longitude);

    public record CreateProject(Guid TemplateId, string DisplayName, string Number, string DatacenterLocation, string BillingCountry, string GeographicLocation, double Latitude, double Longitude, string? TimeZone);

    public record TemplateInfo(Guid Id);

    public record TargetInfo(Guid Id);

    public record ProvisionCopyStatus(string CopyConfigurationId, string CopyConfigurationName, string Service, /* Value from CopyStatus */ string? Status, string ReasonPhrase);

    public record Provision(Guid Id, TemplateInfo Template, TargetInfo Target, /* Value from ProvisionStatus */ string? State, string Region, ICollection<ProvisionCopyStatus>? CopyStatuses);
    
    public record CreateProjectResponse(Project Project, Provision Provision);

    public record CreateConnection(Uri Url, string DisplayName, string Description, string WorkAreaName);

    public record GetConnection(string Id, Uri Url, string Type, string DisplayName, string Description, string WorkAreaName);

    public class ProjectTypes
    {
        public const string ProjectWise = "ProjectWise";
        public const string Synchro = "SYNCHRO";
    }

    public class ProvisionStatus
    {
        public const string Created = "Created";
        public const string Queued = "Queued";
        public const string Started = "Started";
        public const string Succeeded = "Succeeded";
        public const string Failed = "Failed";
        public const string Terminated = "Terminated";
    }

    public class CopyStatus
    {
        public const string Accepted = "Accepted";
        public const string Queued = "Queued";
        public const string InProgress = "InProgress";
        public const string Failed = "Failed";
        public const string Succeeded = "Succeeded";
    }
}
