/*---------------------------------------------------------------------------------------------
* Copyright (c) Bentley Systems, Incorporated. All rights reserved.
* See LICENSE.md in the project root for license terms and full copyright notice.
*--------------------------------------------------------------------------------------------*/
namespace EsApi4DScheduleSampleApp.Models
{
    public class ResourceUserFieldValue
    {
        public List<ResourceUserFieldValueItem>? Items { get; set; }
        public string? NextPageToken { get; set; }
    }
}
