﻿/*---------------------------------------------------------------------------------------------
* Copyright (c) Bentley Systems, Incorporated. All rights reserved.
* See LICENSE.md in the project root for license terms and full copyright notice.
*--------------------------------------------------------------------------------------------*/
using System.Net;
using EsApi4DScheduleSampleApp;
using EsApi4DScheduleSampleApp.Models;


await ConsoleApp.RunAsync(args, async (arguments, configuration) =>
{
    var client = new HttpClient()
    {
        BaseAddress = configuration.ServiceHost,
        DefaultRequestHeaders = { { "Authorization", $"Bearer {arguments.Token}" } },
    };

    ConsoleApp.Log("Initial query to test authorization. Fetching schedules for a given project - Queries all schedules in the specified Project.");
    ConsoleApp.Log($"Sending GET request to {client.BaseAddress}?projectId={arguments.Schedule}");
    var scheduleResponse = await client.GetAsync($"?projectId={arguments.Schedule}");
    var stringResp = await scheduleResponse.Content.ReadAsStringAsync();
    Console.WriteLine($"Response: {stringResp}");
    Console.WriteLine();

    // only check once for authentication problems
    if (!scheduleResponse.IsSuccessStatusCode)
        {
            ConsoleApp.Log("Fetching schedules for given project failed with status code: {0}.", scheduleResponse.StatusCode);
            if (scheduleResponse.StatusCode is HttpStatusCode.Unauthorized or HttpStatusCode.Forbidden && scheduleResponse.Headers.TryGetValues("WWW-Authenticate", out var reasonHeader))
            {
                ConsoleApp.Log("Authentication failure reason: {0}", string.Join(";", reasonHeader));
            }
            return;
        }

    ConsoleApp.Log("Authentication successful");
    Console.WriteLine();

    if (arguments.Single)
    {
        if (!stringResp.Contains("\"items\":[]"))
        {
            ConsoleApp.Log("Fetching single schedule - Use this endpoint to query a single schedule in the specified project schedule list.");
            ConsoleApp.Log($"Sending GET request to {client.BaseAddress}{arguments.Schedule}");
            var get = new HttpGet($"{arguments.Schedule}", client);
            var response = await get.Get();
        }
        else
        {
            ConsoleApp.Log("No items for single schedule.");
        }
        return;
    }
    else if (arguments.Post)
    {
        ConsoleApp.Log("Fetching all resource status history - Use this endpoint to query all resource status history items in the specified project schedule.");
        ConsoleApp.Log($"Sending GET request to {client.BaseAddress}{arguments.Schedule}/resourceStatusHistory");
        var get = new HttpGet($"{arguments.Schedule}/resourceStatusHistory", client);
        var response = await get.GetJson<ResourceStatusHistoryGet>();
        Console.WriteLine();

        if (response.Items != null)
        {
            ConsoleApp.Log("Posting new resource status history - Add a new resource status history item to an associated resource.");
            ConsoleApp.Log($"Sending POST request to {client.BaseAddress}{arguments.Schedule}/resourceStatusHistory");
            var post = new HttpPost($"{arguments.Schedule}/resourceStatusHistory", client);
            var postResp = await post.Post(response);
        }
        else
        {
            ConsoleApp.Log("No items in Resource Status History. Skipping individual ID endpoint requests.");
        }
        return;
    }
    else if (arguments.Pagination is not null)
    {
        ConsoleApp.Log("Fetching Resource User Field Values - Use this endpoint to query all resources and their assigned user field values in the specified project schedule.");
        ConsoleApp.Log($"Sending GET request to {client.BaseAddress}{arguments.Schedule}/resources/userFieldValues");
        var get = new HttpGet($"{arguments.Schedule}/resources/userFieldValues", client);
        var response = await get.GetJson<ResourceUserFieldValue>(arguments.Pagination);
        Console.WriteLine();

        if (response.Items != null)
        {
            Console.WriteLine();
            while (response.NextPageToken != string.Empty)
            {
                response = await get.GetJson<ResourceUserFieldValue>(arguments.Pagination, response.NextPageToken!);
                Console.WriteLine();
            }
        }
        else
        {
            ConsoleApp.Log("No items in Resource User Field Values. Skipping.");
        }
        return;
    }
    else
    {
        ConsoleApp.Log("Fetching all resource status histories - Use this endpoint to query all resource status history items in the specified project schedule.");
        ConsoleApp.Log($"Sending GET request to {client.BaseAddress}{arguments.Schedule}/resourceStatusHistory");
        var get = new HttpGet($"{arguments.Schedule}/resourceStatusHistory", client);
        var response = await get.GetJson<ResourceStatusHistoryGet>();
        Console.WriteLine();

        if (response.Items != null)
        {
            ConsoleApp.Log("Fetching single resource status history - Use this endpoint to query a single resource status history item in the specified project schedule.");
            ConsoleApp.Log($"Sending GET request to {client.BaseAddress}{arguments.Schedule}/resourceStatusHistory/{response.Items[0].Id}");
            get.RequestUri = $"{arguments.Schedule}/resourceStatusHistory/{response.Items[0].Id}";
            var itemResponse = await get.GetJson<ResourceStatusHistoryGetItem>();
            Console.WriteLine();

            ConsoleApp.Log("Posting new resource status history - Add a new resource status history item to an associated resource.");
            ConsoleApp.Log($"Sending POST request to {client.BaseAddress}{arguments.Schedule}/resourceStatusHistory");
            var post = new HttpPost($"{arguments.Schedule}/resourceStatusHistory", client);
            var postResp = await post.Post(itemResponse);
            Console.WriteLine();

            if (postResp.changeRequestId != null)
            {
                ConsoleApp.Log("Fetching single change request for given project - Query a single change request in the specified sync project queue.");
                ConsoleApp.Log($"Sending GET request to {client.BaseAddress}{arguments.Schedule}/changeRequests/{postResp.changeRequestId}");
                get.RequestUri = $"{arguments.Schedule}/changeRequests/{postResp.changeRequestId}";
                var changeResponse = await get.Get();
            }
            else
            {
                ConsoleApp.Log("POST failed. Skipping change request endpoint request.");
            }
        }
    }
});
