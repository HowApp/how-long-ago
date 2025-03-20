namespace How.Server.Extensions;

using Core.Infrastructure.CertificateManagement;
using HowCommon.Configurations;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.AspNetCore.Server.Kestrel.Https;

public static class WebHostExtensions
{
    public static WebApplicationBuilder ConfigureKestrel(this WebApplicationBuilder builder)
    {
        var certConfig = new CertificateConfiguration();
        builder.Configuration.Bind(nameof(CertificateConfiguration), certConfig);

        var certificateManager = CertificateManager.GetInstance();
        certificateManager.SetUpManagerConfig(certConfig);

        builder.WebHost.ConfigureKestrel(options =>
        {
            options.ListenAnyIP(7060, listenOptions =>
            {
                listenOptions.UseHttps();
                listenOptions.Protocols = HttpProtocols.Http1AndHttp2AndHttp3;
            });

            options.ListenAnyIP(7061, listenOptions =>
            {
                listenOptions.UseHttps(certificateManager.GetCertificate());
                listenOptions.Protocols = HttpProtocols.Http1AndHttp2AndHttp3;
            });

            options.ConfigureHttpsDefaults(h =>
            {
                h.ClientCertificateMode = ClientCertificateMode.RequireCertificate;
                h.CheckCertificateRevocation = false;
                h.ServerCertificate = certificateManager.GetCertificate();
            });
        });

        return builder;
    }
}