using Azure.Core;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ClassifiedAds.Services.Identity.IdentityProviders.IdServer;

public class IdServerProvider : IIdentityProvider
{
    private readonly IdServerOptions _options;
    private HttpClient _httpClient = new HttpClient();

    public IdServerProvider(IdServerOptions options)
    {
        _options = options;
    }

    public async Task<string> GetAccessToken()
    {
        var request = new HttpRequestMessage(HttpMethod.Post, _options.TokenUrl);
        var collection = new List<KeyValuePair<string, string>>();
        collection.Add(new("client_id", _options.ClientId));
        collection.Add(new("client_secret", _options.ClientSecret));
        collection.Add(new("grant_type", _options.GrantType));
        collection.Add(new("scope", _options.Scope));
        var content = new FormUrlEncodedContent(collection);
        request.Content = content;
        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();
        var responseText = await response.Content.ReadAsStringAsync();
        var tokens = JsonSerializer.Deserialize<Dictionary<string, object>>(responseText);
        return tokens["access_token"].ToString();
    }

    public async Task SetAccessToken()
    {
        var accessToken = await GetAccessToken();
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        _httpClient.DefaultRequestHeaders.Accept.Clear(); // Önce temizlemek iyi olur
        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

    }

    public async Task CreateUserAsync(IUser user)
    {
        await SetAccessToken();

        var json = JsonSerializer.Serialize(new IdServerUser
        {
            UserId = user.Id,
            Email = user.Email
        });

        var request = new HttpRequestMessage(HttpMethod.Post, _options.Audience);
        request.Headers.Add("Accept", "application/json");
        var content = new StringContent("{   \n  \"userName\": \"WsjyeOv7q\",\n  \"email\": \"WsjyeOv7q@gmail.com\"\n}", null, "application/json");

        var response = await _httpClient.SendAsync(request);

        response.EnsureSuccessStatusCode();

        var responseText = await response.Content.ReadAsStringAsync();

        var createdUser = JsonSerializer.Deserialize<IdServerUser>(responseText);

        user.Id = createdUser.UserId;
    }

    public async Task DeleteUserAsync(string userId)
    {
        await SetAccessToken();

        var request = new HttpRequestMessage(HttpMethod.Delete, _options.Audience + $"/{userId}");
        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();
    }

    public async Task<IUser> GetUserById(string userId)
    {
        await SetAccessToken();

        var request = new HttpRequestMessage(HttpMethod.Get, _options.Audience + $"/{userId}");
        var response = await _httpClient.SendAsync(request);

        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return null;
        }

        var responseText = await response.Content.ReadAsStringAsync();

        var user = JsonSerializer.Deserialize<IdServerUser>(responseText);

        return new User
        {
            Id = user.UserId,
            Username = user.Email,
            Email = user.Email,
        };
    }

    public async Task<IUser> GetUserByUsernameAsync(string username)
    {
        await SetAccessToken();

        var request = new HttpRequestMessage(HttpMethod.Get, _options.Audience + $"?searchText={username}");
        var response = await _httpClient.SendAsync(request);
        var responseText = await response.Content.ReadAsStringAsync();

        var userResponse = JsonSerializer.Deserialize<IdServerResponse>(responseText);

        if (userResponse.TotalCount == 0)
        {
            return null;
        }

        var user = userResponse.Users.First();

        return new User
        {
            Id = user.UserId,
            Username = user.Email,
            Email = user.Email,
        };
    }

    public async Task<IList<IUser>> GetUsersAsync()
    {
        await SetAccessToken();

        var request = new HttpRequestMessage(HttpMethod.Get, _options.Audience);
        var response = await _httpClient.SendAsync(request);
        var responseText = await response.Content.ReadAsStringAsync();

        var users = JsonSerializer.Deserialize<IdServerResponse>(responseText);

        return users.Users.Select(x => (IUser)new User
        {
            Id = x.UserId,
            Username = x.Email,
            Email = x.Email,
        }).ToList();
    }

    public async Task UpdateUserAsync(string userId, IUser user)
    {
        await SetAccessToken();

        var options = new JsonSerializerOptions()
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };
        var json = JsonSerializer.Serialize(new IdServerUser
        {
            UserName = user.Username
        }, options);
        // Content-Type burada açıkça belirtiliyor
        var content = new StringContent(json, Encoding.UTF8, "application/json");


        var request = new HttpRequestMessage(HttpMethod.Put, _options.Audience + $"/{userId}")
        {
            Content = content
        };
        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var responseText = await response.Content.ReadAsStringAsync();

        var updatedUser = JsonSerializer.Deserialize<IdServerUser>(responseText);
    }
}
