using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace skylance_backend.Services
{
    // MLService.cs
    public class MLService
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions;

        public MLService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

        /// Calls POST /predict on the Python service (no body needed beyond empty JSON).
        /// Returns the number of rows the Python service updated.

        public async Task<BulkResult> CallBulkAsync()
        {
            using var content = new StringContent("{}", Encoding.UTF8, "application/json");
            var resp = await _httpClient.PostAsync("/predict", content);
            resp.EnsureSuccessStatusCode();

            var json = await resp.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<BulkResult>(json, _jsonOptions)!;
        }

        /// Calls POST /predict/{bookingId} on the Python service.
        /// Returns both the booking ID and the single prediction.

        public async Task<SingleResult> CallSingleAsync(string bookingId)
        {
            using var content = new StringContent("{}", Encoding.UTF8, "application/json");
            var resp = await _httpClient.PostAsync($"/predict/{bookingId}", content);
            resp.EnsureSuccessStatusCode();

            var json = await resp.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<SingleResult>(json, _jsonOptions)!;
        }

        public class BulkResult
        {
            public int updated { get; set; }
        }

        public class SingleResult
        {
            public string booking_id { get; set; }
            public int prediction { get; set; }
        }
    }
}