/*---------------------------------------------------------------------------------------------
* Copyright (c) Bentley Systems, Incorporated. All rights reserved.
* See LICENSE.md in the project root for license terms and full copyright notice.
*--------------------------------------------------------------------------------------------*/
namespace EsApi4DScheduleSampleApp.Models
{
    public class ResourceStatusHistoryGetItem
    {
        public string? Id { get; set; }
        public string? ResourceId { get; set; }
        public DateTime? Date { get; set; }
        public DateTime? RecordDate { get; set; }
        public string? StatusItemId { get; set; }
        public bool? IsCritical { get; set; }
        public string? Note { get; set; }
        public string? StatusCategoryId { get; set; }
    }
}
