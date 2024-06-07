/*---------------------------------------------------------------------------------------------
* Copyright (c) Bentley Systems, Incorporated. All rights reserved.
* See LICENSE.md in the project root for license terms and full copyright notice.
*--------------------------------------------------------------------------------------------*/
namespace EsApi4DScheduleSampleApp.Models
{
    public class ResourceStatusHistoryGet
    {
        public List<ResourceStatusHistoryGetItem>? Items { get; set; }
        public object? NextPageToken { get; set; }
    }
}
