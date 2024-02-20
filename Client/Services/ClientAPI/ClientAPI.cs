namespace How.Client.Services.ClientAPI;

using System.Net.Http.Json;
using ResultClient;

public abstract class ClientAPI
{
    private readonly HttpClient _httpClient;

    protected ClientAPI(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    protected async Task<TReturn> GetAsync<TReturn>(string uri)
    {
        var response = await _httpClient.GetAsync(uri);
        return await ProcessHttpResponse<TReturn>(response);
    }
    
    protected async Task<TReturn> PostAsync<TReturn, TRequest>(string uri, TRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync(uri, request);
        return await ProcessHttpResponse<TReturn>(response);
    }
    
    protected async Task<TReturn> PostAsync<TReturn>(string uri)
    {
        var response = await _httpClient.PostAsync(uri, null);
        return await ProcessHttpResponse<TReturn>(response);
    }
    
    private static async Task<TReturn> ProcessHttpResponse<TReturn>(HttpResponseMessage response)
    {
        var result = await response.Content.ReadFromJsonAsync<TReturn>();
        
        if (result is null)
        {
            var message = await response.Content.ReadAsStringAsync();
            throw new Exception(message);
        }
        
        if (result is ResultResponse resultResponse)
        {
            if (resultResponse.Failed)
            {
                throw new Exception(string.Join(" ", resultResponse.Error.Values.ToList()));
            }
        }
        
        return result;
    }
}