namespace WhaleTrail.Services
{
    public class DataService
    {
        private HttpClient _client;

        public DataService()
        {
            _client = new HttpClient();
        }

        public async Task<string> FetchEncounterDataAsync()
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "https://critterspot.happywhale.com/v1/csx/encounters/byloc");
            request.Headers.Add("Authorization", "Bearer e9c1598e-4fea-4194-a7ac-303f3dafa34a");

            var requestBody = "{\r\n    \"latlng\":{\r\n        \"lat\": 36,\r\n        \"lng\": -121\r\n    },\r\n    \"since\": \"2024-01-01\"\r\n}";
            // var lastFetchDate = Preferences.Get("LastFetchDate", DateTime.MinValue);
            // var requestBody = $@"
            // {{
            //     ""latlng"":{{
            //         ""lat"": 36,
            //         ""lng"": -121
            //     }},
            //     ""since"": ""{lastFetchDate.ToString("yyyy-MM-dd")}""
            // }}";
            request.Content = new StringContent(requestBody, System.Text.Encoding.UTF8, "application/json");

            var response = await _client.SendAsync(request);
            response.EnsureSuccessStatusCode(); // Throws exception if the HTTP response status is an error code.

            var responseContent = await response.Content.ReadAsStringAsync();
            return responseContent; // Here you might want to deserialize the JSON string into a C# object.
        }

    }
}
