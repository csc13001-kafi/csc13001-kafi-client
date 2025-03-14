using System;
using Microsoft.UI.Xaml.Controls;

namespace kafi.Contracts
{
    public interface INavigationService
    {
        Frame Frame { get; set; }
        void NavigateTo(Type sourcePage);
        void NavigateTo(Type sourcePage, object parameter);
        void GoBack();
    }
}
