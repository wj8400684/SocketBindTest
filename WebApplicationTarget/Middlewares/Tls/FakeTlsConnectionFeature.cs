using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Http.Features;

namespace WebApplicationTarget.Middlewares.Tls;

/// <summary>
/// 假冒的TlsConnectionFeature
/// </summary>
internal sealed class FakeTlsConnectionFeature : ITlsConnectionFeature
{
    public static FakeTlsConnectionFeature Instance { get; } = new();

    public X509Certificate2? ClientCertificate
    {
        get => throw new NotImplementedException();
        set => throw new NotImplementedException();
    }

    public Task<X509Certificate2?> GetClientCertificateAsync(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}