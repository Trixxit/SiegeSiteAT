namespace FPSHome.Services
{
    public class CustomHttpClientHandler : HttpClientHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            request.Headers.UserAgent.ParseAdd("FPSLairAssessmentTask/1.0 (Blazor WebAssembly; .NET 8.0)");
            return base.SendAsync(request, cancellationToken);
        }
    }
}
