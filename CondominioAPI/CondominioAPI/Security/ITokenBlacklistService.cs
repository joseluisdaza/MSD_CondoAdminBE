namespace CondominioAPI.Security;

public interface ITokenBlacklistService
{
    void RevokeToken(string token, DateTime expiresAtUtc);
    bool IsTokenRevoked(string token);
}
