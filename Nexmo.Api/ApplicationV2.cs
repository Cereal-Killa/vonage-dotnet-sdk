﻿using Newtonsoft.Json;
using Nexmo.Api.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Nexmo.Api
{
    public class AppRequest
    {
        /// <summary>
        /// Optional. Used when updating an existing application.
        /// </summary>
        [JsonProperty("id")]
        public string Id { get; set; }
        /// <summary>
        /// The name of your application. Required.
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }
        /// <summary>
        /// The private and public keys to be used with the Application. 
        /// </summary>
        [JsonProperty("keys")]
        public Keys Keyes { get; set; }
        /// <summary>
        /// The Nexmo product or products that you access with this application. Possible values are:
        ///   voice, messages, RTC, VBC
        ///   This contains the configuration for each product and replaces type from V1 of the Application API.
        /// </summary>
        [JsonProperty("capabilities")]
        public Capabilities Capabilities { get; set; }
    }

    public class Keys
    {
        /// <summary>
        /// The public key used to validate the jwt in a https://en.wikipedia.org/wiki/JSON_Web_Token.
        /// </summary>
        [JsonProperty("private_key")]
        public string Private_key { get; set; }
        /// <summary>
        /// The private key you use to generate the JSON Web Token (JWT) that authenticates your requests to Voice API.
        /// </summary>
        [JsonProperty("public_key")]
        public string Public_key { get; set; }
    }

    public class Capabilities
    {
        [JsonProperty("voice")]
        public VoiceWebhook[] Voice { get; set; }
        [JsonProperty("rtc")]
        public RtcWebhook[] Rtc { get; set; }
        [JsonProperty("messages")]
        public MessagesWebhook[] Messages { get; set; }
        [JsonProperty("vbc")]
        public VbcWebhook[] Vbc { get; set; }

    }

    public class VoiceWebhook
    {
        [JsonProperty("answer_url")]
        public WebHook AnswerUrl { get; set; }
        [JsonProperty("event_url")]
        public WebHook EventUrl { get; set; }
    }

    public class RtcWebhook
    {
        [JsonProperty("event_url")]
        public WebHook EventUrl { get; set; }
    }

    public class MessagesWebhook
    {
        [JsonProperty("inbound_url")]
        public WebHook InboundUrl { get; set; }
        [JsonProperty("status_url")]
        public WebHook StatusUrl { get; set; }
    }

    public class VbcWebhook
    {
        
    }

    public class Webhook
    {
        [JsonProperty("address")]
        public string Address { get; set; }
        [JsonProperty("http_method")]
        public string HttpMethod { get; set; }
    }

    public class AppResponse
    {
        /// <summary>
        /// The name of your application
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }
        /// <summary>
        /// The Nexmo product or products that you access with this application. Currently only voice is supported.
        /// </summary>
        [JsonProperty("capabilities")]
        public Capabilities Capabilities { get; set; }
        /// <summary>
        /// The ID allocated to your application by Nexmo.
        /// </summary>
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("keys")]
        public Keys Keys { get; set; }
    }

    public class AppList
    {
        [JsonProperty("applications")]
        public List<AppResponse> Applications { get; set; }
    }

    public class AppListResponse : PaginatedResponse<AppList>
    { }

    public class ApplicationV2
    {
        /// <summary>
        /// Create a new application
        /// </summary>
        /// <param name="request">Application request</param>
        /// <param name="credentials">(Optional) Overridden credentials for only this request</param>
        /// <returns></returns>
        public static AppResponse Create(AppRequest request, Credentials credentials = null)
        {
            var response = ApiRequest.DoPostRequest(ApiRequest.GetBaseUriFor(typeof(ApplicationV2), "/v2/applications"), request, credentials);

            return JsonConvert.DeserializeObject<AppResponse>(response.JsonResponse);
        }

        /// <summary>
        /// Gets an application
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="credentials"></param>
        /// <returns></returns>
        public static AppResponse Get(string appId, Credentials credentials = null)
        {
            var response = ApiRequest.DoRequest(ApiRequest.GetBaseUriFor(typeof(ApplicationV2), $"/v2/applications/{appId}"), new { }, credentials);

            return JsonConvert.DeserializeObject<AppResponse>(response);
        }
       
        /// <summary>
        /// List all of the applications associated with this account
        /// </summary>
        /// <param name="PageSize">Set the number of items returned on each call to this endpoint. The default is 10 records.</param>
        /// <param name="PageIndex">Set the offset from the first page. The default value is 0, calls to this endpoint return a page of <page_size>. For example, set page_index to 3 to retrieve items 31 - 40 when page_size is the default value.</param>
        /// <param name="AppId">Optional id of specific application to retrieve</param>
        /// <param name="credentials">(Optional) Overridden credentials for only this request</param>
        /// <returns></returns>
        public static List<AppResponse> List(int PageSize = 10, int PageIndex = 0, string AppId = "", Credentials credentials = null)
        {
            if (!string.IsNullOrEmpty(AppId))
            {
                return new List<AppResponse>
                {
                    JsonConvert.DeserializeObject<AppResponse>(
                        ApiRequest.DoRequest(ApiRequest.GetBaseUriFor(typeof(ApplicationV2),
                        $"/v2/applications/{AppId}"),
                        // TODO: using this method sig allows us to have the api auth injected at the expense of opaque code here
                        new Dictionary<string, string>(),
                        credentials))
                };

            }

            var json = ApiRequest.DoRequest(ApiRequest.GetBaseUriFor(typeof(ApplicationV2), "/v2/applications"), new Dictionary<string, string>
            {
                { "page_size", PageSize.ToString()},
                { "page_index", PageIndex.ToString()}
            },
            credentials);
            var response = JsonConvert.DeserializeObject<AppListResponse>(json);
            return response._embedded.Applications;
        }

        /// <summary>
        /// Modify a single application
        /// </summary>
        /// <param name="request">Application request</param>
        /// <param name="credentials">(Optional) Overridden credentials for only this request</param>
        /// <returns></returns>
        public static AppResponse Update(AppRequest request, Credentials credentials = null)
        {
            var sb = ApiRequest.GetQueryStringBuilderFor(request);
            var response = ApiRequest.DoPutRequest(ApiRequest.GetBaseUriFor(typeof(ApplicationV2),
                $"/v2/applications/{request.Id}?{sb}"), null, credentials);

            return JsonConvert.DeserializeObject<AppResponse>(response.JsonResponse);
        }

        /// <summary>
        /// Delete a single application
        /// </summary>
        /// <param name="appId">The application id to delete</param>
        /// <param name="credentials">(Optional) Overridden credentials for only this request</param>
        /// <returns></returns>
        public static bool Delete(string appId, Credentials credentials = null)
        {
            var sb = ApiRequest.GetQueryStringBuilderFor(new object());
            var response = ApiRequest.DoDeleteRequest(ApiRequest.GetBaseUriFor(typeof(ApplicationV2),
                $"/v2/applications/{appId}?{sb}"), null, credentials);

            return response.Status == HttpStatusCode.NoContent;
        }
    }
}
