namespace BangaloreTaxi.Api.Configuration;

public sealed class ForwardedHeadersSettings
{
    public const string SectionName = "ForwardedHeaders";

    /// <summary>Proxy addresses allowed to set X-Forwarded-For. Loopback is always trusted.</summary>
    public string[] KnownProxies { get; set; } = [];

    /// <summary>CIDR networks (e.g. 10.0.0.0/8) allowed to set forwarded headers.</summary>
    public string[] KnownNetworks { get; set; } = [];
}
