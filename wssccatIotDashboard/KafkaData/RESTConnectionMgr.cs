using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace KafkaData
{
    class RESTConnectionMgr
    {
        public async Task<HttpResponseMessage> CreateConsumer()
        {
            //string topicString = "/SensorData";
            //UriBuilder u1 = new UriBuilder();
            ////u1.Host = "localhost"; //DEBUG
            //u1.Host = "wssccatiot.westus.cloudapp.azure.com";
            //u1.Port = 8082;
            //u1.Path = "topics" + topicString;
            //u1.Scheme = "http";
            //Uri topicUri = u1.Uri;
            string topicUri = "http://wssccatiot.westus.cloudapp.azure.com:8082";
            //Currently focused on REST API surface for Confluent.io Kafka deployment. We can make this more generic in the future
            //DEBUG
            //string jsonBody = JsonConvert.SerializeObject(data, Formatting.None);
            //END DEBUG
            string jsonBody = "stuff goes here";
            //string correctedJsonBody = jsonBody.Replace(",", "}}, {\"value\":{"); //have to add in some json features into the string. Easier than creating unnecessary classes that would make this come out automatically
            //jsonBody = jsonBody.Replace(",", "}}, {\"value\":{"); //have to add in some json features into the string. Easier than creating unnecessary classes that would make this come out automatically
            string jsonHeader = ("{\"records\":[{\"value\":"); //same as above, fixing string for Server requirements
            string jsonFooter = ("}]}"); //ditto
            string json = jsonHeader + jsonBody + jsonFooter;

            var baseFilter = new HttpClientHandler();
            baseFilter.AutomaticDecompression = System.Net.DecompressionMethods.None; //turn off all compression methods
            HttpClient httpClient = new HttpClient(baseFilter);
            //httpClient.BaseAddress = topicUri;
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.AcceptEncoding.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.kafka.json.v1+json, application/vnd.kafka+json, application/json")); //Add Accept: application/vnd.kafka.json.vl+json, application... header )
            HttpResponseMessage postResponse = await httpClient.PostAsync(topicUri, new StringContent(json));
            //FROM IOT CLIENT SIDE CODE - DELETE ONCE THE SERVER SIDE CODE WORKS
            //var headerContent = new HttpStringContent(json);
            //headerContent.Headers.ContentType = null; // removing all header content and will replace with the required values
            //headerContent.Headers.ContentType = new MediaTypeWithQualityHeaderValue("application/vnd.kafka.json.v1+json"); //Content-Type: application/vnd.kafka.json.v1+json
            //httpClient.DefaultRequestHeaders.Accept.Add(new HttpMediaTypeWithQualityHeaderValue("application/vnd.kafka.json.v1+json, application/vnd.kafka+json, application/json")); //Add Accept: application/vnd.kafka.json.vl+json, application... header
            //HttpResponseMessage postResponse = await httpClient.PostAsync(topicUri, headerContent);
            //END CLIENT SIDE CODE SNIPPET
            return postResponse;
        }

    }
}
