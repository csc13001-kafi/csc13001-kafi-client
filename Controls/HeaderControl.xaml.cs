using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System.IdentityModel.Tokens.Jwt;
using kafi.Contracts.Services;
using kafi;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace kafi.Controls
{
    public sealed partial class HeaderControl : UserControl
    {
        private readonly ISecureTokenStorage _tokenStorage;

        public HeaderControl(ISecureTokenStorage tokenStorage)
        {
            _tokenStorage = tokenStorage;
            this.InitializeComponent();
            var tokens = _tokenStorage.GetTokens();
            DisplayUserInfoFromToken(tokens.accessToken);
        }

        public HeaderControl()
        {
            _tokenStorage = App.Services.GetService(typeof(ISecureTokenStorage)) as ISecureTokenStorage;
            this.InitializeComponent();
            
            if (_tokenStorage != null)
            {
                var tokens = _tokenStorage.GetTokens();
                DisplayUserInfoFromToken(tokens.accessToken);
            }
        }

        private void DisplayUserInfoFromToken(string token)
        {
            if (string.IsNullOrEmpty(token)) return;
            
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);

            var role = jwtToken.Claims.FirstOrDefault(c => c.Type == "role")?.Value;
            var username = jwtToken.Claims.FirstOrDefault(c => c.Type == "username")?.Value;
            System.Diagnostics.Debug.WriteLine(role);
            System.Diagnostics.Debug.WriteLine(username);
            // Update the TextBlocks directly
            RoleTextBlock.Text = role ?? "Unknown Role";
            UsernameTextBlock.Text = username ?? "Unknown User";
        }
    }
}
