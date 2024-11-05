using Common.Models;
using Newtonsoft.Json;
using Serilog;
using System.Net.Http;
using System.Text;

namespace Common.Services
{
    public class ApiConnectService
    {
        private string apiUrl { get; set; }

        public ApiConnectService(string apiUrl)
        {
            this.apiUrl = apiUrl;
        }

        public async Task<HttpResponseMessage> FromApi<T>(string api, T model)
        {
            var json = JsonConvert.SerializeObject(model);
            StringContent stringContent = new StringContent(json, Encoding.UTF8, "application/json");
            HttpClient httpClient = new HttpClient();
            return await httpClient.PostAsync($"{apiUrl}/api/{api}", stringContent);
        }


        public async Task<HttpResponseMessage> FromApi(string api)
        {
            HttpClient httpClient = new HttpClient();
            HttpResponseMessage httpResponseMessage = await httpClient.PostAsync($"{apiUrl}/api/{api}", null);
            return httpResponseMessage;
        }

        public async Task<HttpResponseMessage> FromApiGet(string api)
        {
            HttpClient httpClient = new HttpClient();
            HttpResponseMessage httpResponseMessage = await httpClient.GetAsync($"{apiUrl}/api/{api}");
            return httpResponseMessage;
        }
    }
}
