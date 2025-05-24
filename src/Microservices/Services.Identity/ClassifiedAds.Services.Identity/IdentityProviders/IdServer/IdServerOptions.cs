namespace ClassifiedAds.Services.Identity.IdentityProviders.IdServer;

public class IdServerOptions
{
    public bool Enabled { get; set; }

    public string TokenUrl { get; set; }

    public string ClientId { get; set; }

    public string ClientSecret { get; set; }
    public string GrantType { get; set; }

    public string Scope { get; set; }
    public string Audience { get; set; }
}
