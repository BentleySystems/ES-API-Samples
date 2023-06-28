﻿/*---------------------------------------------------------------------------------------------
* Copyright (c) Bentley Systems, Incorporated. All rights reserved.
* See LICENSE.md in the project root for license terms and full copyright notice.
*--------------------------------------------------------------------------------------------*/
using System.Text;
using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ScheduleAPIConsumer
{
    public record Schedules(string ProjectId, string OrderBy, string Order, int PageSize, string PageToken);

    public class ResourceStatusPost
    {
        public string? changeRequestId { get; set; }
        public ResourceStatusPostEntity? entity { get; set; }
    }

    public class ResourceStatusPostEntity
    {
        public string? resourceId { get; set; }
        public string? statusCategoryId { get; set; }
        public string? statusItemId { get; set; }
        public DateTime date { get; set; }
    }

    public abstract class HttpRequest
    {
        public string RequestUri { get; set; }
        protected readonly HttpClient Client;

        protected HttpRequest(string requestUri, HttpClient client)
        {
            RequestUri = requestUri;
            Client = client;
        }
    }

    public class HttpGet : HttpRequest
    {
        public HttpGet(string requestUri, HttpClient client) : base(requestUri, client) { }

        public async Task<string> Get()
        {
            var response = await Client.GetAsync(RequestUri);
            var stringResp = response.Content.ReadAsStringAsync().Result;
            Console.WriteLine(stringResp);
            Console.WriteLine();
            return stringResp;
        }
    }

    public class HttpPost : HttpRequest
    {
        protected string[]? Split;

        public HttpPost(string requestUri, HttpClient client, string[] split) : base(requestUri, client)
        {
            Split = split;
        }

        public async Task<string> Post()
        {
            var ent = new ResourceStatusPostEntity
            {
                resourceId = $"{Split[5]}",
                date = new DateTime(2023, 3, 28, 9, 0, 0),
                statusCategoryId = $"{Split[27]}",
                statusItemId = $"{Split[17]}"
            };

            var postReq = new ResourceStatusPost
            {
                changeRequestId = Guid.NewGuid().ToString(),
                entity = ent
            };

            var json = JsonSerializer.Serialize(postReq);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            ConsoleApp.Log("POST request contains the following:");
            Console.WriteLine($"{{\n  changeRequestId: {postReq.changeRequestId},\n  entity: {{\n    resourceId: {ent.resourceId}," +
                $"\n    date: {ent.date},\n    statusCategoryId: {ent.statusCategoryId},\n    statusItemId: {ent.statusItemId},\n  }}\n}}");

            var response = await Client.PostAsync(RequestUri, content);
            string stringResp = response.Content.ReadAsStringAsync().Result;
            Console.WriteLine($"Response: {stringResp}");
            Console.WriteLine();
            return stringResp;
        }
    }
}