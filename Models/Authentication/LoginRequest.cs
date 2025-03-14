namespace kafi.Models.Authentication
{
    public class LoginRequest
    {
        public required string username { get; set; }
        public required string password { get; set; }
    }
}
