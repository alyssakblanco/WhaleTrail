namespace WhaleTrail.Services
{
    public class DataService
    {
        private HttpClient _client;

        public DataService()
        {
            _client = new HttpClient();
        }

        public async Task<string> FetchEncounterDataAsync(DateTime lastFetchTimestamp, double lat, double lng)
        {
            Console.WriteLine("FetchEncounterDataAsync");
            Console.WriteLine($"lat {lat}");
            Console.WriteLine($"lng {lng}");
            Console.WriteLine($"timestamp? {lastFetchTimestamp}");

            var request = new HttpRequestMessage(HttpMethod.Post, "https://critterspot.happywhale.com/v1/csx/encounters/byloc");
            request.Headers.Add("Authorization", "Bearer e9c1598e-4fea-4194-a7ac-303f3dafa34a");

            var requestBody = $@"
            {{
                ""latlng"":{{
                    ""lat"": {lat},
                    ""lng"": {lng}
            }},
                ""since"": ""{lastFetchTimestamp:yyyy-MM-dd}""
            }}";
            request.Content = new StringContent(requestBody, System.Text.Encoding.UTF8, "application/json");

            var response = await _client.SendAsync(request);
            response.EnsureSuccessStatusCode(); 

            var responseContent = await response.Content.ReadAsStringAsync();
            return responseContent; 
        }

        public async Task<string> FetchWhaleDataAsync(string whaleId)
        {
            Console.WriteLine("FetchWhaleDataAsync");

            var request = new HttpRequestMessage(HttpMethod.Get, $"https://critterspot.happywhale.com/v1/csx/individual/get/{whaleId}");
            request.Headers.Add("Authorization", "Bearer e9c1598e-4fea-4194-a7ac-303f3dafa34a");

            var response = await _client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            return responseContent;
        }

    }
}
