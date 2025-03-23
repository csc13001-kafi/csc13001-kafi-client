using CommunityToolkit.Mvvm.ComponentModel;
using System.Threading.Tasks;

namespace kafi.ViewModels
{
    public partial class InfoViewModel : ObservableObject
    {
        [ObservableProperty]
        private string userName;

        [ObservableProperty]
        private string email;

        [ObservableProperty]
        private string role;

        [ObservableProperty]
        private string profilePicture;

        public InfoViewModel()
        {
            // Initialize with default values
            UserName = "User";
            Email = "user@example.com";
            Role = "Employee";
            ProfilePicture = "/Assets/DefaultProfile.png";
        }

        public async Task LoadUserInfoAsync()
        {
            // In a real app, you would load user info from a service
            await Task.Delay(500); // Simulate network delay
            
            // For now, just use placeholder data
            UserName = "John Doe";
            Email = "john.doe@kafi.com";
            Role = "Manager";
        }
    }
}