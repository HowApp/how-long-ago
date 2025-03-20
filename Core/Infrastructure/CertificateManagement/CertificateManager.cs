namespace How.Core.Infrastructure.CertificateManagement;

using HowCommon.Infrastructure.CertificateManagement;

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