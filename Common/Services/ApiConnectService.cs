using Common.Models;
using Newtonsoft.Json;
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

        public async Task<HttpResponseMessage> FromApi<T>(string api, T model) where T : Model
        {
            var json = JsonConvert.SerializeObject(model);

            return await FromApi(api, json);
        }

        public async Task<HttpResponseMessage> FromApi(string api, string str)
        {
            StringContent stringContent = new StringContent(str, Encoding.UTF8, "application/json");
            HttpClient httpClient = new HttpClient();
            return await httpClient.PostAsync($"{apiUrl}/api/{api}/", stringContent);
        }
    }
}
