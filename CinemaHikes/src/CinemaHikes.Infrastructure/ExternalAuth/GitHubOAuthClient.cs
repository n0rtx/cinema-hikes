using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json.Serialization;
using CinemaHikes.Domain.Interfaces.ExternalAuth;
using Microsoft.Extensions.Options;

namespace CinemaHikes.Infrastructure.ExternalAuth
{
    public class GitHubOAuthOptions
    {
        public string ClientId { get; set; } = "";
        public string ClientSecret { get; set; } = "";
        public string RedirectUrl { get; set; } = "";
    }
    public class GitHubOAuthClient(HttpClient http,IOptions<GitHubOAuthOptions> options):IGitHubOAuthClient
    {
        private record TokenResponse([property: JsonPropertyName("access_token")] string? AccessToken);
        private record GhUser(long Id, string Login, [property: JsonPropertyName("avatar_url")] string? AvatarUrl);
        private record GhEmail(string Email, bool Primary, bool Verified);
        public async Task<GitHubUserInfo> GetUserByCodeAsync(string code,CancellationToken ct)
        {
            var o = options.Value;
            using var tokenReq = new HttpRequestMessage(HttpMethod.Post, "https://github.com/login/oauth/access_token")
            {
                Content = new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    ["client_id"] = o.ClientId,
                    ["client_secret"] = o.ClientSecret,
                    ["code"] = code,
                    ["redirect_uri"] = o.RedirectUrl
                })
            };
            tokenReq.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var tokenRes = await http.SendAsync(tokenReq,ct);
            tokenRes.EnsureSuccessStatusCode();
            var token = (await tokenRes.Content.ReadFromJsonAsync<TokenResponse>(ct))?.AccessToken
                    ?? throw new UnauthorizedAccessException("incorrect GitHub code");

            var user = await GetAsync<GhUser>("https://api.github.com/user", token, ct);
            var emails = await GetAsync<List<GhEmail>>("https://api.github.com/user/emails", token, ct);
            var email = emails.FirstOrDefault(e => e.Primary && e.Verified)?.Email;
            return new GitHubUserInfo(user.Id, user.Login, user.AvatarUrl, email);

        }
        public async Task<T> GetAsync<T>(string url,string token,CancellationToken ct)
        {
            using var req = new HttpRequestMessage(HttpMethod.Get, url);
            req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            req.Headers.UserAgent.Add(new ProductInfoHeaderValue("CinemaHikes", "1.0"));
            req.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.github+json"));
            var res = await http.SendAsync(req, ct);
            res.EnsureSuccessStatusCode();
            return (await res.Content.ReadFromJsonAsync<T>(ct))!;
        }
    }
}
