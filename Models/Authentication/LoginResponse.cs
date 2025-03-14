namespace kafi.Models.Authentication
{
    public class LoginResponse
    {
        public required string accessToken { get; set; }
        public required string refreshToken { get; set; }
        public required string message { get; set; }
    }
}
