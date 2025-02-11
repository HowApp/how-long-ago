namespace How.Core.Infrastructure.CertificateManagement;

using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using Common.Configurations;
using Common.Constants;
using Microsoft.Extensions.Configuration;

public sealed class CertificateManager
{
    private static readonly string CertPath = GetCertificatePath("how_app_microservices.pfx");

    private static CertificateManager _instance;

    private static readonly object Lock = new object();

    private CertificateManager()
    {
    }

    public static CertificateManager GetInstance()
    {
        if (_instance == null)
        {
            lock (Lock)
            {
                if (_instance == null)
                {
                    _instance = new CertificateManager();
                }
            }
        }
        return _instance;
    }

    public X509Certificate2 GetOrCreateCertificate(IConfiguration configuration)
    {
        var certPassword = new CertificateConfiguration();
        configuration.Bind(nameof(CertificateConfiguration), certPassword);

        if (File.Exists(CertPath))
        {
            var cert = new X509Certificate2(CertPath, certPassword.Password);
            
            return cert;
        }
        else
        {
            throw new FileNotFoundException("Certificate not found");
        }
    }
    
    private static string  GetCertificatePath(string certName)
    {
        var directory = string.Empty;
        
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            directory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "dev-cert", CertificateConstant.ProductName);
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            directory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "dev-cert", CertificateConstant.ProductName);
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            directory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "dev-cert", CertificateConstant.ProductName);
        }
        else
        {
            throw new PlatformNotSupportedException("Unsupported operating system.");
        }

        return Path.Combine(directory, certName);
    }

    private static void EnsureDirectoryExist(string directory)
    {
        if (string.IsNullOrEmpty(directory))
        {
            throw new ArgumentNullException(nameof(directory));
        }

        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }
    }
}