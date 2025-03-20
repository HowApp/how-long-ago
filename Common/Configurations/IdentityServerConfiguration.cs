namespace How.Common.Configurations;

public class IdentityServerConfiguration
{
    public string Authority { get; set; }
    public string Audience { get; set; }
    public string ApiClientId { get; set; }
    public string ApiClientSecret { get; set; }
    public string SwaggerClientId { get; set; }
    public string SwaggerClientSecret { get; set; }
}