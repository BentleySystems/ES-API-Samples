/*---------------------------------------------------------------------------------------------
* Copyright (c) Bentley Systems, Incorporated. All rights reserved.
* See LICENSE.md in the project root for license terms and full copyright notice.
*--------------------------------------------------------------------------------------------*/
using System.Text;
using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Net.Http.Json;

namespace EsApi4DScheduleSampleApp
{
    public class ChangeRequest
    {
        public string? changeRequestId { get; set; }
    }

    public class ResourceStatusHistoryGet
    {
        public List<ResourceStatusHistoryGetItem>? items { get; set; }
        public object? nextPageToken { get; set; }
    }

    public class ResourceStatusHistoryGetItem
    {
        public string? id { get; set; }
        public string? resourceId { get; set; }
        public DateTime? date { get; set; }
        public DateTime? recordDate { get; set; }
        public string? statusItemId { get; set; }
        public bool? isCritical { get; set; }
        public string? note { get; set; }
        public string? statusCategoryId { get; set; }
    }


    public class ResourceStatusPost
    {
        public string? changeRequestId { get; set; }
        public ResourceStatusPostItem? item { get; set; }
    }

    public class ResourceStatusPostItem
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

        public async Task<ResourceStatusHistoryGet> GetResourceStatusHistory()
        {
            var response = await Client.GetAsync(RequestUri);
            var jsonResp = await response.Content.ReadFromJsonAsync<ResourceStatusHistoryGet>();
            Console.WriteLine($"Response: {await response.Content.ReadAsStringAsync()}");
            return jsonResp;
        }

        public async Task<ResourceStatusHistoryGetItem> GetSingleResourceStatusHistory()
        {
            var response = await Client.GetAsync(RequestUri);
            var jsonResp = await response.Content.ReadFromJsonAsync<ResourceStatusHistoryGetItem>();
            Console.WriteLine($"Response: {await response.Content.ReadAsStringAsync()}");
            return jsonResp;
        }

        public async Task<string> Get()
        {
            var response = await Client.GetAsync(RequestUri);
            var stringResp = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Response: {stringResp}");
            Console.WriteLine();
            return stringResp;
        }
    }

    public class HttpPost : HttpRequest
    {

        public HttpPost(string requestUri, HttpClient client) : base(requestUri, client) { }

        public async Task<ChangeRequest> Post(ResourceStatusHistoryGetItem json)
        {
            var ent = new ResourceStatusPostItem
            {
                resourceId = json.resourceId,
                date = DateTime.Now,
                statusCategoryId = json.statusCategoryId,
                statusItemId = json.statusItemId
            };

            var postReq = new ResourceStatusPost
            {
                changeRequestId = Guid.NewGuid().ToString(),
                item = ent
            };

            var jsonPost = JsonSerializer.Serialize(postReq);
            var content = new StringContent(jsonPost, Encoding.UTF8, "application/json");

            ConsoleApp.Log("POST request contains the following:");
            Console.WriteLine($"{{\n  \"changeRequestId\": \"{postReq.changeRequestId}\",\n  \"item\": {{\n    \"resourceId\": \"{ent.resourceId}\"," +
                $"\n    \"date\": \"{ent.date:s}\",\n    \"statusCategoryId\": \"{ent.statusCategoryId}\",\n    \"statusItemId\": \"{ent.statusItemId}\",\n  }}\n}}");

            var response = await Client.PostAsync(RequestUri, content);
            var jsonResp = await response.Content.ReadFromJsonAsync<ChangeRequest>();
            Console.WriteLine($"Response: {await response.Content.ReadAsStringAsync()}");
            Console.WriteLine();
            return jsonResp;
        }

        public async Task<ChangeRequest> Post(ResourceStatusHistoryGet json)
        {
            var ent = new ResourceStatusPostItem
            {
                // just get first item in list
                resourceId = json.items[0].resourceId,
                date = DateTime.Now,
                statusCategoryId = json.items[0].statusCategoryId,
                statusItemId = json.items[0].statusItemId
            };

            var postReq = new ResourceStatusPost
            {
                changeRequestId = Guid.NewGuid().ToString(),
                item = ent
            };

            var jsonPost = JsonSerializer.Serialize(postReq);
            var content = new StringContent(jsonPost, Encoding.UTF8, "application/json");

            ConsoleApp.Log("POST request contains the following:");
            Console.WriteLine($"{{\n  \"changeRequestId\": \"{postReq.changeRequestId}\",\n  \"item\": {{\n    \"resourceId\": \"{ent.resourceId}\"," +
                $"\n    \"date\": \"{ent.date:s}\",\n    \"statusCategoryId\": \"{ent.statusCategoryId}\",\n    \"statusItemId\": \"{ent.statusItemId}\",\n  }}\n}}");

            var response = await Client.PostAsync(RequestUri, content);
            var jsonResp = await response.Content.ReadFromJsonAsync<ChangeRequest>();
            Console.WriteLine($"Response: {await response.Content.ReadAsStringAsync()}");
            Console.WriteLine();
            return jsonResp;
        }
    }
}
