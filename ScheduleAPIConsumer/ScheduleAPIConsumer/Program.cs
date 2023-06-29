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
using EsApi4DScheduleSampleApp;


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
    var stringResp = scheduleResponse.Content.ReadAsStringAsync().Result;
    Console.WriteLine($"Response: {stringResp}");
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

    if (arguments.Single)
    {
        if (!stringResp.Contains("\"items\":[]"))
        {
            ConsoleApp.Log("Fetching single schedule - Use this endpoint to query a single schedule in the specified project schedule list.");
            ConsoleApp.Log($"Sending GET request to {client.BaseAddress}{arguments.Schedule}");
            var get = new HttpGet($"{arguments.Schedule}", client);
            var response = get.Get().Result;
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
        var response = get.Get().Result;
        var stringManipulated = response.Remove(0, 16);
        var stringSplit = stringManipulated.Split('\"');
        Console.WriteLine();

        if (!response.Contains("\"items\":[]"))
        {
            ConsoleApp.Log("Posting new resource status history - Add a new resource status history item to an associated resource.");
            ConsoleApp.Log($"Sending POST request to {client.BaseAddress}{arguments.Schedule}/resourceStatusHistory");
            var post = new HttpPost($"{arguments.Schedule}/resourceStatusHistory", client, stringSplit);
            var postResp = post.Post().Result;
        }
        else
        {
            ConsoleApp.Log("No items in Resource Status History. Skipping individual ID endpoint requests.");
        }
        return;
    }
    else
    {
        ConsoleApp.Log("Fetching all resource status histories - Use this endpoint to query all resource status history items in the specified project schedule.");
        ConsoleApp.Log($"Sending GET request to {client.BaseAddress}{arguments.Schedule}/resourceStatusHistory");
        var get = new HttpGet($"{arguments.Schedule}/resourceStatusHistory", client);
        var response = get.Get().Result;
        var stringManipulated = response.Remove(0, 16);
        var stringSplit = stringManipulated.Split('\"');
        var id = stringSplit[1];

        if (!response.Contains("\"items\":[]"))
        {
            ConsoleApp.Log("Fetching single resource status history - Use this endpoint to query a single resource status history item in the specified project schedule.");
            ConsoleApp.Log($"Sending GET request to {client.BaseAddress}{arguments.Schedule}/resourceStatusHistory/{id}");
            get.RequestUri = $"{arguments.Schedule}/resourceStatusHistory/{id}";
            response = get.Get().Result;

            ConsoleApp.Log("Posting new resource status history - Add a new resource status history item to an associated resource.");
            ConsoleApp.Log($"Sending POST request to {client.BaseAddress}{arguments.Schedule}/resourceStatusHistory");
            var post = new HttpPost($"{arguments.Schedule}/resourceStatusHistory", client, stringSplit);
            var postResp = post.Post().Result;

            if (postResp.Contains("changeRequestId"))
            {
                stringSplit = postResp.Split('\"');
                id = stringSplit[3];
                ConsoleApp.Log("Fetching single change request for given project - Query a single change request in the specified sync project queue.");
                ConsoleApp.Log($"Sending GET request to {client.BaseAddress}{arguments.Schedule}/changeRequests/{id}");
                get.RequestUri = $"{arguments.Schedule}/changeRequests/{id}";
                response = get.Get().Result;
            }
            else
            {
                ConsoleApp.Log("POST failed. Skipping change request endpoint request.");
            }
        }
    }
});
