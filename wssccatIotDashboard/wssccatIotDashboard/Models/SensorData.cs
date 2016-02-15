using Newtonsoft.Json;  
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http.Filters;
using System.Web;

namespace wssccatIotDashboard.Models
{
    public class SensorData
    {
        public int BarometricPressure { get; set; } 
        //public float CelciusTemperature { get; set; } 
        public int FahrenheitTemperature { get; set; } 
        public int Humidity { get; set; } 
        public string TimeStamp { get; set; } 
        public string ClientName { get; set; }
        private async Task PostDataAsync()
        {
            string topicString = "/SensorData";
            UriBuilder u1 = new UriBuilder();
            //u1.Host = "localhost"; //DEBUG
            u1.Host = "wssccatiot.westus.cloudapp.azure.com";
            u1.Port = 8082;
            u1.Path = "topics" + topicString;
            u1.Scheme = "http";
            Uri topicUri = u1.Uri;
            //Currently focused on REST API surface for Confluent.io Kafka deployment. We can make this more generic in the future
            string jsonBody = JsonConvert.SerializeObject(data, Formatting.None);
            //string correctedJsonBody = jsonBody.Replace(",", "}}, {\"value\":{"); //have to add in some json features into the string. Easier than creating unnecessary classes that would make this come out automatically
            jsonBody = jsonBody.Replace(",", "}}, {\"value\":{"); //have to add in some json features into the string. Easier than creating unnecessary classes that would make this come out automatically
            string jsonHeader = ("{\"records\":[{\"value\":"); //same as above, fixing string for Server requirements
            string jsonFooter = ("}]}"); //ditto
            string json = jsonHeader + jsonBody + jsonFooter;

            var baseFilter = new HttpBaseProtocolFilter();
            baseFilter.AutomaticDecompression = false; //turn OFF header "Accept-Encoding"
            HttpClient httpClient = new HttpClient(baseFilter);
            try
            {
                var headerContent = new HttpStringContent(json);
                headerContent.Headers.ContentType = null; // removing all header content and will replace with the required values
                headerContent.Headers.ContentType = new HttpMediaTypeHeaderValue("application/vnd.kafka.json.v1+json"); //Content-Type: application/vnd.kafka.json.v1+json
                httpClient.DefaultRequestHeaders.Accept.Add(new HttpMediaTypeWithQualityHeaderValue("application/vnd.kafka.json.v1+json, application/vnd.kafka+json, application/json")); //Add Accept: application/vnd.kafka.json.vl+json, application... header
                HttpResponseMessage postResponse = await httpClient.PostAsync(topicUri, headerContent);
            }
            catch
            {
                
            }

        }
    }
}