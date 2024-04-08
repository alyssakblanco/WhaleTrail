namespace WhaleTrail.Services
{
    public class DataService
    {
        private HttpClient _client;

        public DataService()
        {
            _client = new HttpClient();
        }

        public async Task<string> FetchEncounterDataAsync(DateTime lastFetchTimestamp)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "https://critterspot.happywhale.com/v1/csx/encounters/byloc");
            request.Headers.Add("Authorization", "Bearer e9c1598e-4fea-4194-a7ac-303f3dafa34a");

            var requestBody = $@"
            {{
                ""latlng"":{{
                    ""lat"": 36,
                    ""lng"": -121
            }},
                ""since"": ""{lastFetchTimestamp.ToString("yyyy-MM-dd")}""
            }}";
            request.Content = new StringContent(requestBody, System.Text.Encoding.UTF8, "application/json");

            var response = await _client.SendAsync(request);
            response.EnsureSuccessStatusCode(); 

            var responseContent = await response.Content.ReadAsStringAsync();
            return responseContent; 
        }

    }
}
