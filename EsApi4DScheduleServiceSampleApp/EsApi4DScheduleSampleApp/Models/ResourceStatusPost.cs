/*---------------------------------------------------------------------------------------------
* Copyright (c) Bentley Systems, Incorporated. All rights reserved.
* See LICENSE.md in the project root for license terms and full copyright notice.
*--------------------------------------------------------------------------------------------*/
namespace EsApi4DScheduleSampleApp.Models
{
    public class ResourceStatusPost
    {
        public string? ChangeRequestId { get; set; }
        public ResourceStatusPostItem? Item { get; set; }
    }
}
