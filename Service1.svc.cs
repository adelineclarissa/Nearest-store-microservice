using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;

namespace StoreSearchService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Service1.svc or Service1.svc.cs at the Solution Explorer and start debugging.
    public class Service1 : IService1
    {
        public async Task<string> FindNearestStoreAsync(string location, string storeName)
        {
            string apiKey = "uCTeZLec478jNNOJhGr8S-wzls3HDFrKiEsyIMK2moWyxZa6gU-ehwzuXPxuT6x9b7Z61YnfEe6Abwi9SeIUWQJCk8_i4YqAvHUrQ0G546b68lwjsVnxmtpfw98YZXYx"; // Define your API key here

            var client = new RestClient(new RestClientOptions("https://api.yelp.com/v3/businesses/search"));
            var request = new RestRequest("");

            request.AddHeader("accept", "application/json");
            request.AddHeader("Authorization", $"Bearer {apiKey}");
            request.AddParameter("location", location);
            request.AddParameter("term", storeName);
            request.AddParameter("limit", 5);

            var response = await client.GetAsync(request);

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                JObject data = JObject.Parse(response.Content);
                int totalResults = (int)data["total"];


                if (totalResults > 0)
                {
                    JArray stores = (JArray)data["businesses"];
                    StringBuilder resultStringBuilder = new StringBuilder($"Closest {storeName} to {location}:\n");

                    for (int i = 0; i < Math.Min(5, stores.Count); i++)
                    {
                        JObject store = (JObject)stores[i];
                        string storeNameResult = store["name"].ToString();
                        string storeAddress = string.Join(", ", store["location"]["display_address"]);
                        resultStringBuilder.AppendLine($"{storeNameResult} located at {storeAddress}");
                    }

                    return resultStringBuilder.ToString();

                }
                else
                {
                    return $"No {storeName} found near {location}";
                }
            }
            else
            {
                return $"Error: Unable to fetch data from Yelp API (Status Code {response.StatusCode})";
            }
        }
    }
}
