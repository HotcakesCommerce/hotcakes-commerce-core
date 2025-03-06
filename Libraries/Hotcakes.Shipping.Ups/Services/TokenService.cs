#region License

// Distributed under the MIT License
// ============================================================
// Copyright (c) 2020-2025 Upendo Ventures, LLC
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software 
// and associated documentation files (the "Software"), to deal in the Software without restriction, 
// including without limitation the rights to use, copy, modify, merge, publish, distribute, 
// sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is 
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or 
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE 
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, 
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN 
// THE SOFTWARE.

#endregion

using Hotcakes.Shipping.Ups.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Runtime.Caching;
using System.Text;

namespace Hotcakes.Shipping.Ups.Services
{
    /// <summary>
    /// Service responsible for retrieving and caching the OAuth 2.0 access token from the UPS API.
    /// </summary>
    public class TokenService
    {
        /// <summary>
        /// Global settings for UPS service, including client credentials.
        /// </summary>
        private readonly UPSServiceGlobalSettings _settings;

        /// <summary>
        /// The base URL for the UPS API.
        /// </summary>
        private readonly string _baseUrl;

        /// <summary>
        /// The endpoint for retrieving an OAuth 2.0 access token from the UPS API.
        /// </summary>
        private const string _tokenEndpoint = "/security/v1/oauth/token";

        /// <summary>
        /// A memory cache to store the access token and its expiration time.
        /// </summary>
        private static MemoryCache cache = MemoryCache.Default;

        /// <summary>
        /// Cache key for storing the UPS access token.
        /// </summary>
        private const string TokenCacheKey = "UPSToken";

        /// <summary>
        /// Cache key for storing the expiration time of the UPS access token.
        /// </summary>
        private const string ExpirationCacheKey = "UPSTokenExpiration";


        /// <summary>
        /// Initializes a new instance of the <see cref="TokenService"/> class with the specified settings and base URL.
        /// </summary>
        /// <param name="settings">The global settings that include the UPS API client credentials.</param>
        /// <param name="baseUrl">The base URL of the UPS API.</param>
        public TokenService(UPSServiceGlobalSettings settings, string baseUrl)
        {
            _settings = settings;
            _baseUrl = baseUrl;
        }


        /// <summary>
        /// Retrieves the UPS OAuth 2.0 access token. If a valid token is cached, it will be returned. 
        /// Otherwise, a new token is requested from the UPS API.
        /// </summary>
        /// <returns>The UPS API access token as a string.</returns>
        /// <remarks>
        /// The method first checks the cache for a valid token. If a valid token is found and has not expired, it is returned.
        /// Otherwise, an HTTP POST request is made to the UPS API to obtain a new token using client credentials. 
        /// The token and its expiration time are then stored in memory cache.
        /// </remarks>
        /// <example>
        /// <code>
        /// var tokenService = new TokenService(settings, baseUrl);
        /// var accessToken = tokenService.GetAccessTokenAsync();
        /// </code>
        /// </example>
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
                    var jsonResponse = response.Content.ReadAsStringAsync().Result;
                    var tokenResponse = JsonConvert.DeserializeObject<TokenResponse>(jsonResponse);

                    StoreTokenInCache(tokenResponse.AccessToken, DateTime.UtcNow.AddSeconds(tokenResponse.ExpiresIn));

                    return tokenResponse.AccessToken;
                }
            }
        }

        /// <summary>
        /// Stores the access token and its expiration time in the memory cache.
        /// </summary>
        /// <param name="token">The access token to be stored.</param>
        /// <param name="expiration">The expiration time of the access token.</param>
        private static void StoreTokenInCache(string token, DateTime expiration)
        {
            cache.Set(TokenCacheKey, token, new CacheItemPolicy { AbsoluteExpiration = expiration });
            cache.Set(ExpirationCacheKey, expiration, new CacheItemPolicy { AbsoluteExpiration = expiration });
        }
    }

}