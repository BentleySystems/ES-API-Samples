﻿/*---------------------------------------------------------------------------------------------
* Copyright (c) Bentley Systems, Incorporated. All rights reserved.
* See LICENSE.md in the project root for license terms and full copyright notice.
*--------------------------------------------------------------------------------------------*/
using System.Net.Http.Json;
using Newtonsoft.Json.Linq;

namespace EsApi4DScheduleSampleApp.Models
{
    public class HttpGet : HttpRequest
    {
        public HttpGet(string requestUri, HttpClient client) : base(requestUri, client) { }

        public async Task<T> GetJson<T>()
        {
            var response = await Client.GetAsync($"{RequestUri}");
            var jsonResp = await response.Content.ReadFromJsonAsync<T>();
            Console.WriteLine($"Response: {await response.Content.ReadAsStringAsync()}");
            return jsonResp!;
        }

        public async Task<JToken> GetJson(string pageSize = "100", string nextPageToken = "null")
        {
            HttpResponseMessage response;
            if (nextPageToken is not "null")
            {
                response = await Client.GetAsync($"{RequestUri}?pageSize={pageSize}&pageToken={nextPageToken}");
            }
            else
            {
                response = await Client.GetAsync($"{RequestUri}?pageSize={pageSize}");
            }
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Response failed with {response.StatusCode}");
            }
            var jsonResp = JToken.Parse(await response.Content.ReadAsStringAsync());
            Console.WriteLine($"Response: {await response.Content.ReadAsStringAsync()}");
            return jsonResp;
        }

        public async Task Get()
        {
            var response = await Client.GetAsync(RequestUri);
            var stringResp = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Response: {stringResp}");
            Console.WriteLine();
        }
    }
}
