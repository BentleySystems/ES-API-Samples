﻿/*---------------------------------------------------------------------------------------------
* Copyright (c) Bentley Systems, Incorporated. All rights reserved.
* See LICENSE.md in the project root for license terms and full copyright notice.
*--------------------------------------------------------------------------------------------*/
namespace EsApiProjectsSampleApp
{
    public record Configuration(Uri ServiceHost, string ProjectType = ProjectTypes.ProjectWise);
}