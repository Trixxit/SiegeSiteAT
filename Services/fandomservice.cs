namespace FPSHome.Services
{
    public class FandomService
    {
        private readonly IHttpClientFactory _clientFactory;

        public FandomService(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        public async Task<string> GetFandomData(string fandomUrl)
        {
            var client = _clientFactory.CreateClient("customClient");
            return await client.GetStringAsync(fandomUrl);
        }
    }
}
