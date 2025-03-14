namespace kafi.Contracts
{
    public interface ISecureTokenStorage
    {
        void SaveTokens(string accessToken, string refreshToken);
        (string accessToken, string refreshToken) GetTokens();
        void ClearTokens();
    }
}
