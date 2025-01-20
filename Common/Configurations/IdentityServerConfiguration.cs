namespace How.Common.Configurations;

public class IdentityServerConfiguration
{
    public string Authority { get; set; }
    public string Audience { get; set; }
    public string ClientId { get; set; }
    public string ClientSecret { get; set; }
    public string ClientSwaggerSecret { get; set; }
}