using System;
using System.Collections.Generic;
using System.Text;

namespace CinemaHikes.Domain.Interfaces.ExternalAuth
{
    public record GitHubUserInfo(long Id, string Login, string? AvatarUrl, string? Email);
    public interface IGitHubOAuthClient
    {
        Task<GitHubUserInfo> GetUserByCodeAsync(string code, CancellationToken cancellationToken);
    }
}
