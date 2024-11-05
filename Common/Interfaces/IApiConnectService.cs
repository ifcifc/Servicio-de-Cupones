namespace Common.Interfaces
{
    public interface IApiConnectService
    {
        Task<HttpResponseMessage> FromApi(string api);
        Task<HttpResponseMessage> FromApi<T>(string api, T model);
        Task<HttpResponseMessage> FromApiGet(string api);
    }
}