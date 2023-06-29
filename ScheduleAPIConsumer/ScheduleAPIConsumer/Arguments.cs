/*---------------------------------------------------------------------------------------------
* Copyright (c) Bentley Systems, Incorporated. All rights reserved.
* See LICENSE.md in the project root for license terms and full copyright notice.
*--------------------------------------------------------------------------------------------*/
namespace ScheduleAPIConsumer
{
    public record Arguments(string Token, string Schedule, bool Single, bool Post);
}
