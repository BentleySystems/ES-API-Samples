/*---------------------------------------------------------------------------------------------
* Copyright (c) Bentley Systems, Incorporated. All rights reserved.
* See LICENSE.md in the project root for license terms and full copyright notice.
*--------------------------------------------------------------------------------------------*/
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace EsApi4DScheduleSampleApp.Models
{
    public class HttpPost : HttpRequest
    {

        public HttpPost(string requestUri, HttpClient client) : base(requestUri, client) { }

        public async Task<ChangeRequest> Post(ResourceStatusHistoryGetItem json)
        {
            var ent = new ResourceStatusPostItem
            {
                ResourceId = json.ResourceId,
                Date = DateTime.Now,
                StatusCategoryId = json.StatusCategoryId,
                StatusItemId = json.StatusItemId
            };

            var postReq = new ResourceStatusPost
            {
                ChangeRequestId = Guid.NewGuid().ToString(),
                Item = ent
            };

            var jsonPost = JsonSerializer.Serialize(postReq);
            var content = new StringContent(jsonPost, Encoding.UTF8, "application/json");

            ConsoleApp.Log("POST request contains the following:");
            Console.WriteLine($"{{\n  \"changeRequestId\": \"{postReq.ChangeRequestId}\",\n  \"item\": {{\n    \"resourceId\": \"{ent.ResourceId}\"," +
                $"\n    \"date\": \"{ent.Date:s}\",\n    \"statusCategoryId\": \"{ent.StatusCategoryId}\",\n    \"statusItemId\": \"{ent.StatusItemId}\",\n  }}\n}}");

            var response = await Client.PostAsync(RequestUri, content);
            var jsonResp = await response.Content.ReadFromJsonAsync<ChangeRequest>();
            Console.WriteLine($"Response: {await response.Content.ReadAsStringAsync()}");
            Console.WriteLine();
            return jsonResp!;
        }

        public async Task<ChangeRequest> Post(ResourceStatusHistoryGet json)
        {
            var ent = new ResourceStatusPostItem
            {
                // just get first item in list
                ResourceId = json.Items![0].ResourceId,
                Date = DateTime.Now,
                StatusCategoryId = json.Items[0].StatusCategoryId,
                StatusItemId = json.Items[0].StatusItemId,
            };

            var postReq = new ResourceStatusPost
            {
                ChangeRequestId = Guid.NewGuid().ToString(),
                Item = ent
            };

            var jsonPost = JsonSerializer.Serialize(postReq);
            var content = new StringContent(jsonPost, Encoding.UTF8, "application/json");

            ConsoleApp.Log("POST request contains the following:");
            Console.WriteLine($"{{\n  \"changeRequestId\": \"{postReq.ChangeRequestId}\",\n  \"item\": {{\n    \"resourceId\": \"{ent.ResourceId}\"," +
                $"\n    \"date\": \"{ent.Date:s}\",\n    \"statusCategoryId\": \"{ent.StatusCategoryId}\",\n    \"statusItemId\": \"{ent.StatusItemId}\",\n  }}\n}}");

            var response = await Client.PostAsync(RequestUri, content);
            var jsonResp = await response.Content.ReadFromJsonAsync<ChangeRequest>();
            Console.WriteLine($"Response: {await response.Content.ReadAsStringAsync()}");
            Console.WriteLine();
            return jsonResp!;
        }
    }
}
