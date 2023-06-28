/*---------------------------------------------------------------------------------------------
* Copyright (c) Bentley Systems, Incorporated. All rights reserved.
* See LICENSE.md in the project root for license terms and full copyright notice.
*--------------------------------------------------------------------------------------------*/
using System;
using System.Diagnostics.Metrics;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using ScheduleAPIConsumer;


await ConsoleApp.RunAsync(args, async (arguments, configuration) =>
{
    var client = new HttpClient()
    {
        BaseAddress = configuration.ServiceHost,
        DefaultRequestHeaders = { { "Authorization", $"Bearer {arguments.Token}" } },
    };

    ConsoleApp.Log("Fetching schedules for a given project - Queries all schedules in the specified Project.");
    var scheduleResponse = await client.GetAsync($"/4dschedule/v1/schedules?projectId={arguments.Schedule}");
    var stringResp = scheduleResponse.Content.ReadAsStringAsync().Result;
    Console.WriteLine(stringResp);
    Console.WriteLine();

    // only check once for authentication problems
    if (!scheduleResponse.IsSuccessStatusCode)
        {
            ConsoleApp.Log("Fetching schedules for given project failed with status code: {0}.\nResponse: {1}", scheduleResponse.StatusCode, await scheduleResponse.Content.ReadAsStringAsync(),
                string.Join(";", scheduleResponse.Headers.Select(h => h.ToString())));
            if (scheduleResponse.StatusCode is HttpStatusCode.Unauthorized or HttpStatusCode.Forbidden && scheduleResponse.Headers.TryGetValues("WWW-Authenticate", out var reasonHeader))
            {
                ConsoleApp.Log("Authentication failure reason: {0}", string.Join(";", reasonHeader));
            }
            return;
        }

    ConsoleApp.Log("Authentication successful");
    Console.WriteLine();

    var schedule = new Schedules(
        ProjectId: arguments.Schedule,
        OrderBy: null,
        Order: "Asc",
        PageSize: 100,
        PageToken: null);

    if (arguments.Single)
    {
        ConsoleApp.Log("Fetching single schedule - Use this endpoint to query a single schedule in the specified project schedule list.");
        if (!stringResp.Contains("\"items\":[]"))
        {
            var get = new HttpGet($"/4dschedule/v1/schedules/{schedule.ProjectId}", client);
            var response = get.Get().Result;
        }
        else
        {
            ConsoleApp.Log("No items for single schedule.");
        }
        return;
    }

    if (arguments.Post)
    {
        ConsoleApp.Log("Fetching all resource status history - Use this endpoint to query all resource status history items in the specified project schedule.");
        var get = new HttpGet($"/4dschedule/v1/schedules/{schedule.ProjectId}/resourceStatusHistory", client);
        var response = get.Get().Result;
        var stringManipulated = response.Remove(0, 16);
        var stringSplit = stringManipulated.Split('\"');
        Console.WriteLine();

        if (!response.Contains("\"items\":[]"))
        {
            ConsoleApp.Log("Posting new resource status history - Add a new resource status history item to an associated resource.");
            var post = new HttpPost($"/4dschedule/v1/schedules/{schedule.ProjectId}/resourceStatusHistory", client, stringSplit);
            var postResp = post.Post().Result;
        }
        else
        {
            ConsoleApp.Log("No items in Resource Status History. Skipping individual ID endpoint requests.");
        }
        return;
    }

    if (arguments.All)
    {
        ConsoleApp.Log("Fetching single schedule - Use this endpoint to query a single schedule in the specified project schedule list.");
        var get = new HttpGet($"/4dschedule/v1/schedules/{schedule.ProjectId}", client);
        if (!stringResp.Contains("\"items\":[]"))
        {
            var singleResp = get.Get().Result;
        }
        else
        {
            ConsoleApp.Log("No items in Schedules. Skipping individual ID endpoint requests.");
            Console.WriteLine();
        }

        ConsoleApp.Log("Fetching tasks - Use this endpoint to query all tasks in the specified project schedule.");
        get.RequestUri = $"/4dschedule/v1/schedules/{schedule.ProjectId}/tasks?order={schedule.Order}&pageSize={schedule.PageSize}";
        var response = get.Get().Result;
        var stringManipulated = response.Remove(0, 16);
        var stringSplit = stringManipulated.Split('\"');

        if (!response.Contains("\"items\":[]"))
        {
            ConsoleApp.Log("Fetching specific task - Use this endpoint to query a single task in the specified project schedule.");
            get.RequestUri = $"/4dschedule/v1/schedules/{schedule.ProjectId}/tasks/{stringSplit[1]}";
            response = get.Get().Result;

            ConsoleApp.Log("Fetching child tasks - Use this endpoint to query all children for the specified Task in the specified project schedule.");
            get.RequestUri = $"/4dschedule/v1/schedules/{schedule.ProjectId}/tasks/{stringSplit[1]}/children";
            response = get.Get().Result;

            ConsoleApp.Log("Fetching specific task code values - Use this endpoint to query all code values for the specified Task in the specified project schedule.");
            get.RequestUri = $"/4dschedule/v1/schedules/{schedule.ProjectId}/tasks/{stringSplit[1]}/codeValues";
            response = get.Get().Result;

            ConsoleApp.Log("Fetching specific task user field values - Use this endpoint to query all user field values for the specified Task in the specified project schedule.");
            get.RequestUri = $"/4dschedule/v1/schedules/{schedule.ProjectId}/tasks/{stringSplit[1]}/userFieldValues";
            response = get.Get().Result;

            ConsoleApp.Log("Fetching specific task resource assignments - Use this endpoint to query all resources for the specified Task in the specified project schedule.");
            get.RequestUri = $"/4dschedule/v1/schedules/{schedule.ProjectId}/tasks/{stringSplit[1]}/resourceAssignments";
            response = get.Get().Result;

            ConsoleApp.Log("Fetching task links - Use this endpoint to query all task links in the specified project schedule.");
            get.RequestUri = $"/4dschedule/v1/schedules/{schedule.ProjectId}/tasks/links";
            response = get.Get().Result;

            ConsoleApp.Log("Fetching downstream task links - Use this endpoint to query all downstream task links for the specified Task in the specified project schedule.");
            get.RequestUri = $"/4dschedule/v1/schedules/{schedule.ProjectId}/tasks/{stringSplit[1]}/downstreamLinks";
            response = get.Get().Result;

            ConsoleApp.Log("Fetching upstream task links - Use this endpoint to query all upstream task links for the specified Task in the specified project schedule.");
            get.RequestUri = $"/4dschedule/v1/schedules/{schedule.ProjectId}/tasks/{stringSplit[1]}/upstreamLinks";
            response = get.Get().Result;

            ConsoleApp.Log("Fetching general task user field links - Use this endpoint to query all tasks and their assigned user field values in the specified project schedule.");
            get.RequestUri = $"/4dschedule/v1/schedules/{schedule.ProjectId}/tasks/userFieldValues";
            response = get.Get().Result;

            ConsoleApp.Log("Fetching general task resource assignments - Use this endpoint to query all tasks and their assigned resources in the specified project schedule.");
            get.RequestUri = $"/4dschedule/v1/schedules/{schedule.ProjectId}/tasks/resourceAssignments";
            response = get.Get().Result;

            ConsoleApp.Log("Fetching general task code value assignments - Use this endpoint to query all tasks and their assigned code values in the specified project schedule.");
            get.RequestUri = $"/4dschedule/v1/schedules/{schedule.ProjectId}/tasks/codeValueAssignments";
            response = get.Get().Result;
        }
        else
        {
            ConsoleApp.Log("No items in Tasks. Skipping individual ID endpoint requests.");
            Console.WriteLine();
        }

        ConsoleApp.Log("Fetching user fields for project - Use this endpoint to query all user fields in the specified project schedule.");
        get.RequestUri = $"/4dschedule/v1/schedules/{schedule.ProjectId}/userFields";
        response = get.Get().Result;
        stringManipulated = response.Remove(0, 16);
        stringSplit = stringManipulated.Split('\"');

        if (!response.Contains("\"items\":[]"))
        {
            ConsoleApp.Log("Fetching user fields for given id in project - Use this endpoint to query a single userfield in the specified project schedule.");
            get.RequestUri = $"/4dschedule/v1/schedules/{schedule.ProjectId}/userFields/{stringSplit[1]}";
            response = get.Get().Result;
        }
        else
        {
            ConsoleApp.Log("No items in User Fields. Skipping individual ID endpoint requests.");
            Console.WriteLine();
        }

        ConsoleApp.Log("Fetching change requests for given project - Queries all change requests in the sync queue that have been submitted for the project.");
        get.RequestUri = $"/4dschedule/v1/schedules/{schedule.ProjectId}/changeRequests";
        response = get.Get().Result;
        stringManipulated = response.Remove(0, 16);
        stringSplit = stringManipulated.Split('\"');

        if (!response.Contains("\"items\":[]"))
        {
            ConsoleApp.Log("Fetching single change request for given project - Query a single change request in the specified sync project queue.");
            get.RequestUri = $"/4dschedule/v1/schedules/{schedule.ProjectId}/changeRequests/{stringSplit[1]}";
            response = get.Get().Result;
        }
        else
        {
            ConsoleApp.Log("No items in Change Requests. Skipping individual ID endpoint requests.");
            Console.WriteLine();
        }

        ConsoleApp.Log("Fetching codes for given project - Queries all codes in the specified project schedule.");
        get.RequestUri = $"/4dschedule/v1/schedules/{schedule.ProjectId}/codes";
        response = get.Get().Result;
        stringManipulated = response.Remove(0, 16);
        stringSplit = stringManipulated.Split('\"');

        if (!response.Contains("\"items\":[]"))
        {
            ConsoleApp.Log("Fetching codes for given id - Query a single code in the specified project schedule.");
            get.RequestUri = $"/4dschedule/v1/schedules/{schedule.ProjectId}/codes/{stringSplit[1]}";
            response = get.Get().Result;

            ConsoleApp.Log("Fetching code values for given id - Queries all code values for the provided code id in the specified project schedule.");
            get.RequestUri = $"/4dschedule/v1/schedules/{schedule.ProjectId}/codes/{stringSplit[1]}/codeValues";
            response = get.Get().Result;
        }
        else
        {
            ConsoleApp.Log("No items in Codes. Skipping individual ID endpoint requests.");
            Console.WriteLine();
        }

        ConsoleApp.Log("Fetching code values for given project - Queries all code values in the specified project schedule.");
        get.RequestUri = $"/4dschedule/v1/schedules/{schedule.ProjectId}/codeValues";
        response = get.Get().Result;
        stringManipulated = response.Remove(0, 16);
        stringSplit = stringManipulated.Split('\"');

        if (!response.Contains("\"items\":[]"))
        {
            ConsoleApp.Log("Fetching code values for given id without codes - Query a single code value in the specified project schedule.");
            get.RequestUri = $"/4dschedule/v1/schedules/{schedule.ProjectId}/codeValues/{stringSplit[1]}";
            response = get.Get().Result;

            ConsoleApp.Log("Fetching child code values for given id without codes - Query all code values that are children of the specified code value id in the specified project schedule.");
            get.RequestUri = $"/4dschedule/v1/schedules/{schedule.ProjectId}/codeValues/{stringSplit[1]}/children";
            response = get.Get().Result;
        }
        else
        {
            ConsoleApp.Log("No items in Code Values. Skipping individual ID endpoint requests.");
            Console.WriteLine();
        }

        ConsoleApp.Log("Fetching all Entity 3Ds - Query all entity 3Ds in the specified project schedule.");
        get.RequestUri = $"/4dschedule/v1/schedules/{schedule.ProjectId}/entity3Ds";
        response = get.Get().Result;
        stringManipulated = response.Remove(0, 16);
        stringSplit = stringManipulated.Split('\"');

        if (!response.Contains("\"items\":[]"))
        {
            ConsoleApp.Log("Fetching single Entity 3D - Query a single Entity3D in the specified project schedule.");
            get.RequestUri = $"/4dschedule/v1/schedules/{schedule.ProjectId}/entity3Ds/{stringSplit[1]}";
            response = get.Get().Result;

            ConsoleApp.Log("Fetching single Entity 3D children - Queries all children for the specified Entity3D in the specified project schedule.");
            get.RequestUri = $"/4dschedule/v1/schedules/{schedule.ProjectId}/entity3Ds/{stringSplit[1]}/children";
            response = get.Get().Result;

            ConsoleApp.Log("Fetching all Entity 3Ds root - Queries all root Entity3Ds (top-level records) in the specified project schedule.");
            get.RequestUri = $"/4dschedule/v1/schedules/{schedule.ProjectId}/entity3Ds/root";
            response = get.Get().Result;

            ConsoleApp.Log("Fetching single Entity 3D user field values - Queries all Entity3D user field values for the specified entity in the specified project schedule.");
            get.RequestUri = $"/4dschedule/v1/schedules/{schedule.ProjectId}/entity3Ds/{stringSplit[1]}/userFieldValues";
            response = get.Get().Result;

            ConsoleApp.Log("Fetching all Entity 3D user field values - Use this endpoint to query all 3D entities user field values in the specified project schedule.");
            get.RequestUri = $"/4dschedule/v1/schedules/{schedule.ProjectId}/entity3Ds/userFieldValues";
            response = get.Get().Result;
        }
        else
        {
            ConsoleApp.Log("No items in Entity 3Ds. Skipping individual ID endpoint requests.");
            Console.WriteLine();
        }

        ConsoleApp.Log("Fetching all resource groups - Use this endpoint to query all resource groups in the specified project schedule.");
        get.RequestUri = $"/4dschedule/v1/schedules/{schedule.ProjectId}/resourceGroups";
        response = get.Get().Result;
        stringManipulated = response.Remove(0, 16);
        stringSplit = stringManipulated.Split('\"');

        if (!response.Contains("\"items\":[]"))
        {
            ConsoleApp.Log("Fetching single resource group - Use this endpoint to query a single resource group in the specified project schedule.");
            get.RequestUri = $"/4dschedule/v1/schedules/{schedule.ProjectId}/resourceGroups/{stringSplit[1]}";
            response = get.Get().Result;

            ConsoleApp.Log("Fetching single resource group code values - Use this endpoint to query all code values for the specified Resource Group in the specified project schedule.");
            get.RequestUri = $"/4dschedule/v1/schedules/{schedule.ProjectId}/resourceGroups/{stringSplit[1]}/codeValues";
            response = get.Get().Result;

            ConsoleApp.Log("Fetching single resource group user field values - Use this endpoint to query all user field values for the specified Resource Group in the specified project schedule.");
            get.RequestUri = $"/4dschedule/v1/schedules/{schedule.ProjectId}/resourceGroups/{stringSplit[1]}/userFieldValues";
            response = get.Get().Result;

            ConsoleApp.Log("Fetching single resource group resources - Use this endpoint to query all resources for the specified Resource Group in the specified project schedule.");
            get.RequestUri = $"/4dschedule/v1/schedules/{schedule.ProjectId}/resourceGroups/{stringSplit[1]}/resources";
            response = get.Get().Result;

            ConsoleApp.Log("Fetching all resource groups code value assignments - Use this endpoint to query all resource groups and their assigned code values in the specified project schedule.");
            get.RequestUri = $"/4dschedule/v1/schedules/{schedule.ProjectId}/resourceGroups/codeValueAssignments";
            response = get.Get().Result;

            ConsoleApp.Log("Fetching all resource groups user field values - Use this endpoint to query all resource groups and their assigned user field values in the specified project schedule.");
            get.RequestUri = $"/4dschedule/v1/schedules/{schedule.ProjectId}/resourceGroups/userFieldValues";
            response = get.Get().Result;
        }
        else
        {
            ConsoleApp.Log("No items in Resource Groups. Skipping individual ID endpoint requests.");
            Console.WriteLine();
        }

        Console.WriteLine();
        ConsoleApp.Log("Fetching all resource status categories - Use this endpoint to query all resource status categories in the specified project schedule.");
        get.RequestUri = $"/4dschedule/v1/schedules/{schedule.ProjectId}/resourceStatusCategories";
        response = get.Get().Result;
        stringManipulated = response.Remove(0, 16);
        stringSplit = stringManipulated.Split('\"');

        if (!response.Contains("\"items\":[]"))
        {
            ConsoleApp.Log("Fetching single resource status category - Use this endpoint to query a single resource status category in the specified project schedule.");
            get.RequestUri = $"/4dschedule/v1/schedules/{schedule.ProjectId}/resourceStatusCategories/{stringSplit[1]}";
            response = get.Get().Result;

            ConsoleApp.Log("Fetching single resource status category sets - Use this endpoint to query all resource status sets for the associated category in the specified project schedule.");
            get.RequestUri = $"/4dschedule/v1/schedules/{schedule.ProjectId}/resourceStatusCategories/{stringSplit[1]}/sets";
            response = get.Get().Result;
        }
        else
        {
            ConsoleApp.Log("No items in Resource Status Category. Skipping individual ID endpoint requests.");
            Console.WriteLine();
        }

        Console.WriteLine();
        ConsoleApp.Log("Fetching all resource status history - Use this endpoint to query all resource status history items in the specified project schedule.");
        get.RequestUri = $"/4dschedule/v1/schedules/{schedule.ProjectId}/resourceStatusHistory";
        response = get.Get().Result;
        stringManipulated = response.Remove(0, 16);
        stringSplit = stringManipulated.Split('\"');

        if (!response.Contains("\"items\":[]"))
        {
            ConsoleApp.Log("Fetching single resource status history - Use this endpoint to query a single resource status history item in the specified project schedule.");
            get.RequestUri = $"/4dschedule/v1/schedules/{schedule.ProjectId}/resourceStatusHistory/{stringSplit[1]}";
            response = get.Get().Result;

            ConsoleApp.Log("Posting new resource status history - Add a new resource status history item to an associated resource.");
            var post = new HttpPost($"/4dschedule/v1/schedules/{schedule.ProjectId}/resourceStatusHistory", client, stringSplit);
            var postResp = post.Post().Result;
        }
        else
        {
            ConsoleApp.Log("No items in Resource Status History. Skipping individual ID endpoint requests.");
            Console.WriteLine();
        }

        ConsoleApp.Log("Fetching all resource status items - Use this endpoint to query all resource status items in the specified project schedule.");
        get.RequestUri = $"/4dschedule/v1/schedules/{schedule.ProjectId}/resourceStatusItems";
        response = get.Get().Result;
        stringManipulated = response.Remove(0, 16);
        stringSplit = stringManipulated.Split('\"');

        if (!response.Contains("\"items\":[]"))
        {
            ConsoleApp.Log("Fetching single resource status item - Use this endpoint to query a single resource status item in the specified project schedule.");
            get.RequestUri = $"/4dschedule/v1/schedules/{schedule.ProjectId}/resourceStatusItems/{stringSplit[1]}";
            response = get.Get().Result;
        }
        else
        {
            ConsoleApp.Log("No items in Resource Status Items. Skipping individual ID endpoint requests.");
            Console.WriteLine();
        }

        ConsoleApp.Log("Fetching all resource status sets - Use this endpoint to query all resource status sets in the specified project schedule.");
        get.RequestUri = $"/4dschedule/v1/schedules/{schedule.ProjectId}/resourceStatusSets";
        response = get.Get().Result;
        stringManipulated = response.Remove(0, 16);
        stringSplit = stringManipulated.Split('\"');

        if (!response.Contains("\"items\":[]"))
        {
            ConsoleApp.Log("Fetching single resource status set - Use this endpoint to query a single resource status set in the specified project schedule.");
            get.RequestUri = $"/4dschedule/v1/schedules/{schedule.ProjectId}/resourceStatusSets/{stringSplit[1]}";
            response = get.Get().Result;

            ConsoleApp.Log("Fetching single resource status set items - Use this endpoint to query all resource status items for the associated status set in the specified project schedule.");
            get.RequestUri = $"/4dschedule/v1/schedules/{schedule.ProjectId}/resourceStatusSets/{stringSplit[1]}/items";
            response = get.Get().Result;
        }
        else
        {
            ConsoleApp.Log("No items in Resource Status Sets. Skipping individual ID endpoint requests.");
            Console.WriteLine();
        }

        ConsoleApp.Log("Fetching all resources - Queries all resources in the specified project schedule.");
        get.RequestUri = $"/4dschedule/v1/schedules/{schedule.ProjectId}/resources";
        response = get.Get().Result;
        stringManipulated = response.Remove(0, 16);
        stringSplit = stringManipulated.Split('\"');

        if (!response.Contains("\"items\":[]"))
        {
            ConsoleApp.Log("Fetching single resource - Query a single resource in the specified project schedule.");
            get.RequestUri = $"/4dschedule/v1/schedules/{schedule.ProjectId}/resources/{stringSplit[1]}";
            response = get.Get().Result;

            ConsoleApp.Log("Fetching single resource status history - Query all resource status history for the specified Resource in the specified project schedule.");
            get.RequestUri = $"/4dschedule/v1/schedules/{schedule.ProjectId}/resources/{stringSplit[1]}/statusHistory";
            response = get.Get().Result;

            ConsoleApp.Log("Fetching single resource children - Query all children for the specified Resource in the specified project schedule.");
            get.RequestUri = $"/4dschedule/v1/schedules/{schedule.ProjectId}/resources/{stringSplit[1]}/children";
            response = get.Get().Result;

            ConsoleApp.Log("Fetching single resource code values - Query all code values for the specified Resource in the specified project schedule.");
            get.RequestUri = $"/4dschedule/v1/schedules/{schedule.ProjectId}/resources/{stringSplit[1]}/codeValues";
            response = get.Get().Result;

            ConsoleApp.Log("Fetching single resource user field values - Query all user field values for the specified Resource in the specified project schedule.");
            get.RequestUri = $"/4dschedule/v1/schedules/{schedule.ProjectId}/resources/{stringSplit[1]}/userFieldValues";
            response = get.Get().Result;

            ConsoleApp.Log("Fetching single resource entity 3Ds - Query all entity 3ds for the specified Resource in the specified project schedule.");
            get.RequestUri = $"/4dschedule/v1/schedules/{schedule.ProjectId}/resources/{stringSplit[1]}/entity3Ds";
            response = get.Get().Result;

            ConsoleApp.Log("Fetching all resources code value assignments - Use this endpoint to query all resources and their assigned code values in the specified project schedule.");
            get.RequestUri = $"/4dschedule/v1/schedules/{schedule.ProjectId}/resources/codeValueAssignments";
            response = get.Get().Result;

            ConsoleApp.Log("Fetching all resource user field values - Use this endpoint to query all resources and their assigned 3D entities in the specified project schedule.");
            get.RequestUri = $"/4dschedule/v1/schedules/{schedule.ProjectId}/resources/userFieldValues";
            response = get.Get().Result;

            ConsoleApp.Log("Fetching all resource entity 3D assignments - Use this endpoint to query all resources and their assigned user field values in the specified project schedule.");
            get.RequestUri = $"/4dschedule/v1/schedules/{schedule.ProjectId}/resources/entity3DAssignments";
            response = get.Get().Result;
        }
        else
        {
            ConsoleApp.Log("No items in Resources. Skipping individual ID endpoint requests.");
            Console.WriteLine();
        }
    }
});
