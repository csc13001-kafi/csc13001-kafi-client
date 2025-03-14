namespace kafi.Models.Authentication
{
    public class RefreshTokenResponse
    {
        public required string accessToken { get; set; }
        public required string refreshToken { get; set; }
    }
}
