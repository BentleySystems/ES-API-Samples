/*---------------------------------------------------------------------------------------------
* Copyright (c) Bentley Systems, Incorporated. All rights reserved.
* See LICENSE.md in the project root for license terms and full copyright notice.
*--------------------------------------------------------------------------------------------*/
namespace EsApi4DScheduleSampleApp.Models
{
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
}
