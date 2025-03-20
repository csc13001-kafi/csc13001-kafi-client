using System;
using Microsoft.UI.Xaml.Controls;

namespace kafi.Contracts.Services
{
    public interface INavigationService
    {
        void Initialize(Frame frame);
        void NavigateTo(Type sourcePage);
        void NavigateTo(Type sourcePage, object parameter);
        void GoBack();
    }
}
