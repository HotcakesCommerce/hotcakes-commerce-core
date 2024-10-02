using Hotcakes.Shipping.Ups.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Runtime.Caching;
using System.Text;

namespace Hotcakes.Shipping.Ups.Services
{
    public class TokenService
    {
        private readonly UPSServiceGlobalSettings _settings;
        private readonly string _baseUrl;
        private const string _tokenEndpoint = "/security/v1/oauth/token";

        private static MemoryCache cache = MemoryCache.Default;
        private const string TokenCacheKey = "UPSToken";
        private const string ExpirationCacheKey = "UPSTokenExpiration";

        public TokenService(UPSServiceGlobalSettings settings, string baseUrl)
        {
            _settings = settings;
            _baseUrl = baseUrl;
        }

        public string GetAccessTokenAsync()
        {
            var token = cache.Get(TokenCacheKey) as string;
            DateTime? tokenExpiration = cache.Get(ExpirationCacheKey) as DateTime?;

            if (token != null && tokenExpiration.HasValue && tokenExpiration > DateTime.UtcNow)
            {
                return token;
            }
            else
            {
                using (var client = new HttpClient())
                {
                    var request = new HttpRequestMessage(HttpMethod.Post, $"{_baseUrl}{_tokenEndpoint}");
                    string credentials = $"{_settings.ClientId}:{_settings.ClientSecret}";
                    string base64Credentials = Convert.ToBase64String(Encoding.UTF8.GetBytes(credentials));
                    request.Headers.Add("Authorization", $"Basic {base64Credentials}");

                    var collection = new List<KeyValuePair<string, string>>
                    {
                        new("grant_type", "client_credentials")
                    };

                    var content = new FormUrlEncodedContent(collection);
                    request.Content = content;

                    var response = client.SendAsync(request).Result;
                    response.EnsureSuccessStatusCode();
                    var jsonResponse =  response.Content.ReadAsStringAsync().Result;
                    var tokenResponse = JsonConvert.DeserializeObject<TokenResponse>(jsonResponse);

                    StoreTokenInCache(tokenResponse.AccessToken, DateTime.UtcNow.AddSeconds(tokenResponse.ExpiresIn));

                    return tokenResponse.AccessToken;
                }
            }
        }

        private static void StoreTokenInCache(string token, DateTime expiration)
        {
            cache.Set(TokenCacheKey, token, new CacheItemPolicy { AbsoluteExpiration = expiration });
            cache.Set(ExpirationCacheKey, expiration, new CacheItemPolicy { AbsoluteExpiration = expiration });
        }
    }

}
