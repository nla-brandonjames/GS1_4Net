#region License
/*
 * Copyright 2017 Brandon James
 *
 *  Licensed under the Apache License, Version 2.0 (the "License");
 *  you may not use this file except in compliance with the License.
 *  You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 *  Unless required by applicable law or agreed to in writing, software
 *  distributed under the License is distributed on an "AS IS" BASIS,
 *  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 *  See the License for the specific language governing permissions and
 *  limitations under the License.
 */
#endregion

using EnumStringValues;
using GS1_4Net.Filters;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using GS1_4Net.Responses;
using GS1_4Net.Exceptions;
using Microsoft.Extensions.Logging;

namespace GS1_4Net
{
    public abstract class ClientBase
    {
        public readonly Configuration Configuration;

        public RequestFormat ClientRequestFormat { get; set; } = RequestFormat.Json;

        public int ApiLimitRemaining { get; set; }
        public int LimitResetSeconds { get; set; }

        public Tuple<string, string> AuthenticationDetails { get; set; }

        public int ApiLimit
        {
            get { return Configuration.ApiLimit; }
            set { Configuration.ApiLimit = value; }
        }

        protected string BaseUri { get; set; }

        ILogger ApiStateLogger = new LoggerFactory().CreateLogger("Api State");

        public event EventHandler<Events.GSResponseEventArgs> RequestCompleted;
        
        public ClientBase(Configuration configuration)
        {
            Configuration = configuration;
            LimitResetSeconds = 30;
            ApiLimitRemaining = Configuration.ApiLimit;
        }

        protected async Task<T> GetDataAsync<T>()
        {
            var message = new HttpRequestMessage(HttpMethod.Get, BaseUri);
            var response = await ExecuteRequest<T>(message);
            return response.Data;
        }

        protected async Task<T> GetDataAsync<T>(IFilter filter)
        {
            return await GetDataAsync<T>("", filter);
        }

        protected async Task<T> GetDataAsync<T>(string resourceEndpoint, IFilter filter = null)
        {
            var endpoint = (string.IsNullOrEmpty(resourceEndpoint)) ? BaseUri : string.Format("{0}/{1}", BaseUri, resourceEndpoint);
            // If you are supplying the filters yourself 
            if (resourceEndpoint.StartsWith("?") || resourceEndpoint.Contains("/"))
            {
                endpoint = string.Format("{0}{1}", BaseUri, resourceEndpoint);
            }

            var message = new HttpRequestMessage(HttpMethod.Get, endpoint);

            var response = await ExecuteRequest<T>(message, filter);
            return response.Data;
        }

        protected async Task<T> HeadDataAsync<T>(string resourceEndpoint, IFilter filter = null)
        {
            var endpoint = (string.IsNullOrEmpty(resourceEndpoint)) ? BaseUri : string.Format("{0}/{1}", BaseUri, resourceEndpoint);
            // If you are supplying the filters yourself 
            if (resourceEndpoint.StartsWith("?") || resourceEndpoint.Contains("/"))
            {
                endpoint = string.Format("{0}{1}", BaseUri, resourceEndpoint);
            }

            var message = new HttpRequestMessage(HttpMethod.Head, endpoint);

            var response = await ExecuteRequest<T>(message, filter);
            return response.Data;
        }

        protected async Task<T> PostDataAsync<T>(T data, IFilter filter = null, RequestFormat requestFormat = RequestFormat.Json)
        {
            return await PostDataAsync(BaseUri, data, filter, requestFormat);
        }

        protected async Task<T> PostAsync<T>(string resourceEndpoint, IFilter filter = null)
        {
            var message = new HttpRequestMessage(HttpMethod.Post, string.Format("{0}/{1}", BaseUri, resourceEndpoint));
            var response = await ExecuteRequest<T>(message, filter);
            return response.Data;
        }

        protected async Task<TResponse> PostDataAsync<TRequest, TResponse>(string resourceEndpoint, TRequest data, IFilter filter = null, RequestFormat requestFormat = RequestFormat.Json)
        {
            var message = new HttpRequestMessage(HttpMethod.Post, string.Format("{0}/{1}", BaseUri, resourceEndpoint));

            message.Content = new StringContent(JsonConvert.SerializeObject(data), System.Text.Encoding.UTF8, requestFormat.GetStringValue());
            var response = await ExecuteRequest<TResponse>(message, filter);
            return response.Data;
        }

        protected async Task<TResponse> PostDataAsync<TRequest, TResponse>(TRequest data, IFilter filter = null, RequestFormat requestFormat = RequestFormat.Json)
        {
            return await PostDataAsync<TRequest, TResponse>("", data, filter, requestFormat);
        }

        protected async Task<T> PostDataAsync<T>(string resourceEndpoint, T data, IFilter filter = null, RequestFormat requestFormat = RequestFormat.Json)
        {
            return await PostDataAsync<T, T>(resourceEndpoint, data, filter, requestFormat);
        }

        protected async Task<T> PutAsync<T>(string resourceEndpoint, IFilter filter = null, RequestFormat requestFormat = RequestFormat.Json)
        {
            var message = new HttpRequestMessage(HttpMethod.Put, string.Format("{0}/{1}", BaseUri, resourceEndpoint));
            message.Content = new StringContent("", System.Text.Encoding.UTF8, requestFormat.GetStringValue());
            var response = await ExecuteRequest<T>(message, filter);
            return response.Data;
        }

        protected async Task<T> PutDataAsync<T>(string resourceEndpoint, T data, IFilter filter = null, RequestFormat requestFormat = RequestFormat.Json)
        {
            var message = new HttpRequestMessage(HttpMethod.Put, string.Format("{0}/{1}", BaseUri, resourceEndpoint));
            message.Content = new StringContent(JsonConvert.SerializeObject(data), System.Text.Encoding.UTF8, requestFormat.GetStringValue());
            var response = await ExecuteRequest<T>(message);
            return response.Data;
        }

        protected async Task<TResponse> PutDataAsync<TRequest, TResponse>(TRequest data, IFilter filter = null)
        {
            return await PutDataAsync<TRequest, TResponse>("", data, filter);
        }

        protected async Task<TResponse> PutDataAsync<TRequest, TResponse>(string resourceEndpoint, TRequest data, IFilter filter = null, RequestFormat requestFormat = RequestFormat.Json)
        {
            var message = new HttpRequestMessage(HttpMethod.Put, string.Format("{0}/{1}", BaseUri, resourceEndpoint));
            message.Content = new StringContent(JsonConvert.SerializeObject(data), System.Text.Encoding.UTF8, requestFormat.GetStringValue());
            var response = await ExecuteRequest<TResponse>(message);
            return response.Data;
        }

        protected async Task<T> PatchDataAsync<T>(string resourceEndpoint, T data, IFilter filter = null, RequestFormat requestFormat = RequestFormat.Json)
        {
            return await PatchDataAsync<T, T>(resourceEndpoint, data, filter, requestFormat);
        }

        protected async Task<TResponse> PatchDataAsync<TRequest, TResponse>(TRequest data, IFilter filter = null, RequestFormat requestFormat = RequestFormat.Json)
        {
            return await PatchDataAsync<TRequest, TResponse>("", data, filter);
        }

        protected async Task<TResponse> PatchDataAsync<TRequest, TResponse>(string resourceEndpoint, TRequest data, IFilter filter = null, RequestFormat requestFormat = RequestFormat.Json)
        {
            var message = new HttpRequestMessage(new HttpMethod("PATCH"), string.Format("{0}/{1}", BaseUri, resourceEndpoint));
            message.Content = new StringContent(JsonConvert.SerializeObject(data), System.Text.Encoding.UTF8, requestFormat.GetStringValue());
            var response = await ExecuteRequest<TResponse>(message);
            return response.Data;
        }

        protected async Task<T> DeleteDataAsync<T>(string resourceEndpoint, IFilter filter = null)
        {
            var message = new HttpRequestMessage(HttpMethod.Delete, string.Format("{0}/{1}", BaseUri, resourceEndpoint));

            var response = await ExecuteRequest<T>(message, filter);
            return response.Data;
        }

        private async Task<IRestResponse<T>> ExecuteRequest<T>(HttpRequestMessage message, IFilter filter = null)
        {
            IRestResponse<T> response;

            if (filter != null && !message.RequestUri.ToString().Contains("?"))
            {
                filter.AddFilter(message);
            }

            var credentials = new NetworkCredential(Configuration.UserName, Configuration.UserApiKey);
            var handler = new HttpClientHandler { Credentials = credentials };

            using (var client = new HttpClient(handler))
            {
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                if (AuthenticationDetails != null)
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(AuthenticationDetails.Item1, AuthenticationDetails.Item2);
                }

                client.BaseAddress = Configuration.BaseUri;

                var httpResponse = await client.SendAsync(message);

                if (!httpResponse.IsSuccessStatusCode)
                {
                    switch (httpResponse.StatusCode)
                    {
                        case (HttpStatusCode)400:
                            throw new BadRequestException(httpResponse.ReasonPhrase, httpResponse);
                        case (HttpStatusCode)401:
                            throw new NotAuthorizedException(httpResponse.ReasonPhrase, httpResponse);
                        case (HttpStatusCode)404:
                            throw new NotFoundException(httpResponse.ReasonPhrase, httpResponse);
                        case (HttpStatusCode)405:
                            throw new MethodNotAllowedException(httpResponse.ReasonPhrase, httpResponse);
                        case (HttpStatusCode)429:
                            ApiLimitRemaining = int.Parse(httpResponse.Headers.GetValues("X-Rate-Limit-Remaining").FirstOrDefault());
                            LimitResetSeconds = int.Parse(httpResponse.Headers.GetValues("X-Rate-Limit-Reset").FirstOrDefault());
                            ApiLimit = int.Parse(httpResponse.Headers.GetValues("X-Rate-Limit-Limit").FirstOrDefault());
                            throw new ApiLimitReachedException(
                                $"{httpResponse.ReasonPhrase}\nThere are {LimitResetSeconds} seconds remaning until a request reset.",
                                httpResponse,
                                ApiLimitRemaining,
                                LimitResetSeconds);
                        case (HttpStatusCode)500:
                            throw new InternalServerErrorException(httpResponse.ReasonPhrase, httpResponse);
                        default:
                            throw new RestException($"This error is not handled\n{httpResponse.ReasonPhrase}", httpResponse);
                    }
                }

                ApiLimitRemaining = int.Parse(httpResponse.Headers.GetValues("X-Rate-Limit-Remaining").FirstOrDefault());
                LimitResetSeconds = int.Parse(httpResponse.Headers.GetValues("X-Rate-Limit-Reset").FirstOrDefault());
                ApiLimit = int.Parse(httpResponse.Headers.GetValues("X-Rate-Limit-Limit").FirstOrDefault());

                PrintApiLimitDetails(httpResponse);
                var headerString = "";

                foreach (var header in httpResponse.Headers)
                {
                    headerString += $"{header.Key}: {header.Value}\n";
                }

                var json = await httpResponse.Content.ReadAsStringAsync();
                ApiStateLogger.LogDebug(headerString + json);

                T responseData = JsonConvert.DeserializeObject<T>(json);

                response = new RestResponse<T>
                {
                    Data = responseData,
                    Request = message,
                    Response = httpResponse
                };

                OnRequestCompleted(response.Request, response.Response);
            }

            return response;
        }

        protected virtual void OnRequestCompleted(HttpRequestMessage request, HttpResponseMessage response)
        {
            var responseEventArgs = new Events.GSResponseEventArgs { Request = request, Response = response };

            RequestCompleted?.Invoke(this, responseEventArgs);
        }

        private void PrintApiLimitDetails(HttpResponseMessage response)
        {
            string limitRemaining = response.Headers.GetValues("X-Rate-Limit-Remaining").FirstOrDefault();
            string limitResetSeconds = response.Headers.GetValues("X-Rate-Limit-Reset").FirstOrDefault();

            ApiStateLogger.LogInformation($"There are {limitRemaining} requests left before a reset in {limitResetSeconds} seconds.");
        }
    }
}
