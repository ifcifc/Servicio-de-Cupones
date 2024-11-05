using System.Net.Http;

HttpClient httpClient = new HttpClient();
Task<HttpResponseMessage>  task = httpClient.PostAsync("https://localhost:7029/api/Cupon/QuemarCupon/716-117-470", null);

task.Wait();
HttpResponseMessage result = task.Result;
Console.WriteLine(">>" + result.ToString());
