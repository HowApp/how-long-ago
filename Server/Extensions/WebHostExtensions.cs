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
                listenOptions.UseHttps(httpsOptions =>
                {
                    httpsOptions.ServerCertificate = certificateManager.GetCertificate();
                    httpsOptions.ClientCertificateMode = ClientCertificateMode.RequireCertificate;
                    httpsOptions.CheckCertificateRevocation = false;
                    httpsOptions.AllowAnyClientCertificate();
                });
                listenOptions.Protocols = HttpProtocols.Http1AndHttp2AndHttp3;
            });
        });

        return builder;
    }
}