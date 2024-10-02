using Newtonsoft.Json;

namespace Hotcakes.Shipping.Ups.Models
{
    public class TokenResponse
    {
        [JsonProperty("token_type")]
        public string TokenType { get; set; }
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }
        [JsonProperty("expires_in")]
        public long ExpiresIn { get; set; }
        [JsonProperty("status")]
        public string Status { get; set; }
    }
}
