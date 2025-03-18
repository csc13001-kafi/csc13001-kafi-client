using System.Linq;
using kafi.Contracts.Services;
using Windows.Security.Credentials;

namespace kafi.Service
{
    public class SecureTokenStorage : ISecureTokenStorage
    {
        private const string _resourceName = "kafi";
        public void SaveTokens(string accessToken, string refreshToken)
        {
            var vault = new PasswordVault();

            try
            {
                var existingAccess = vault.FindAllByResource(_resourceName)
                    .FirstOrDefault(c => c.UserName == "AccessToken");
                if (existingAccess != null)
                {
                    vault.Remove(existingAccess);
                }
            }
            catch { }

            try
            {
                var existingRefresh = vault.FindAllByResource(_resourceName)
                    .FirstOrDefault(c => c.UserName == "RefreshToken");
                if (existingRefresh != null)
                {
                    vault.Remove(existingRefresh);
                }
            }
            catch { }

            vault.Add(new PasswordCredential(_resourceName, "AccessToken", accessToken));
            vault.Add(new PasswordCredential(_resourceName, "RefreshToken", refreshToken));
        }
        public (string accessToken, string refreshToken) GetTokens()
        {
            var vault = new PasswordVault();
            string accessToken = null;
            string refreshToken = null;

            try
            {
                var accessCred = vault.Retrieve(_resourceName, "AccessToken");
                accessCred.RetrievePassword();
                accessToken = accessCred.Password;
            }
            catch { }

            try
            {
                var refreshCred = vault.Retrieve(_resourceName, "RefreshToken");
                refreshCred.RetrievePassword();
                refreshToken = refreshCred.Password;
            }
            catch { }

            return (accessToken, refreshToken);
        }
        public void ClearTokens()
        {
            var vault = new PasswordVault();
            try
            {
                var accessCred = vault.Retrieve(_resourceName, "AccessToken");
                vault.Remove(accessCred);
            }
            catch { }

            try
            {
                var refreshCred = vault.Retrieve(_resourceName, "RefreshToken");
                vault.Remove(refreshCred);
            }
            catch { }
        }
    }
}
