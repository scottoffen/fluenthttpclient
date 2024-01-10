using System.Security.Cryptography.X509Certificates;

namespace FluentHttpClient;

public class NullHttpResponseMessage : HttpResponseMessage
{
    public NullHttpResponseMessage() : base()
    {
        StatusCode = 0;
    }
}
