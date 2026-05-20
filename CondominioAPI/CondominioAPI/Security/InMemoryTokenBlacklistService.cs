using Microsoft.Extensions.Caching.Memory;

namespace CondominioAPI.Security;

public class InMemoryTokenBlacklistService : ITokenBlacklistService
{
    private readonly IMemoryCache _cache;

    public InMemoryTokenBlacklistService(IMemoryCache cache)
    {
        _cache = cache;
    }

    public void RevokeToken(string token, DateTime expiresAtUtc)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            return;
        }

        var safeExpiration = expiresAtUtc <= DateTime.UtcNow ? DateTime.UtcNow.AddMinutes(1) : expiresAtUtc;
        _cache.Set(GetCacheKey(token), true, safeExpiration);
    }

    public bool IsTokenRevoked(string token)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            return false;
        }

        return _cache.TryGetValue(GetCacheKey(token), out _);
    }

    private static string GetCacheKey(string token) => $"revoked_token:{token}";
}
