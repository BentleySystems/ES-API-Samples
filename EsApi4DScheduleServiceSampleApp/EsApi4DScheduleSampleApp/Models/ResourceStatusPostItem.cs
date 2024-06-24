/*---------------------------------------------------------------------------------------------
* Copyright (c) Bentley Systems, Incorporated. All rights reserved.
* See LICENSE.md in the project root for license terms and full copyright notice.
*--------------------------------------------------------------------------------------------*/
namespace EsApi4DScheduleSampleApp.Models
{
    public class ResourceStatusPostItem
    {
        public string? ResourceId { get; set; }
        public string? StatusCategoryId { get; set; }
        public string? StatusItemId { get; set; }
        public DateTime Date { get; set; }
    }
}
