namespace How.Core.Infrastructure.CertificateManagement;

using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using Common.Configurations;
using Common.Constants;
using Microsoft.Extensions.Configuration;

public sealed class CertificateManager : CertificateManagerBase
{
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
}