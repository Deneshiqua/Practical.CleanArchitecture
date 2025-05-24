using System.Text.Json.Serialization;

namespace ClassifiedAds.Services.Identity.IdentityProviders.IdServer;

public class IdServerUser
{
    [JsonPropertyName("userName")]
    public string UserName { get; set; }

    [JsonPropertyName("email")]
    public string Email { get; set; }

    [JsonPropertyName("emailConfirmed")]
    public bool EmailConfirmed { get; set; }

    [JsonPropertyName("phoneNumber")]
    public object PhoneNumber { get; set; }

    [JsonPropertyName("phoneNumberConfirmed")]
    public bool PhoneNumberConfirmed { get; set; }

    [JsonPropertyName("lockoutEnabled")]
    public bool LockoutEnabled { get; set; }

    [JsonPropertyName("twoFactorEnabled")]
    public bool TwoFactorEnabled { get; set; }

    [JsonPropertyName("accessFailedCount")]
    public long AccessFailedCount { get; set; }

    [JsonPropertyName("id")]
    public string UserId { get; set; }
}
public class IdServerResponse
{

    [JsonPropertyName("pageSize")]
    public int PageSize { get; set; }

    [JsonPropertyName("totalCount")]
    public int TotalCount { get; set; }

    [JsonPropertyName("users")]
    public List<IdServerUser> Users { get; set; }
}
