using System;
using kafi.Contracts;
using Microsoft.UI.Xaml.Controls;

namespace kafi.Service
{
    public class NavigationService : INavigationService
    {
        public Frame Frame { get; set; }
        public void GoBack()
        {
            Frame.GoBack();
        }

        public void NavigateTo(Type sourcePage)
        {
            Frame.Navigate(sourcePage);
        }

        public void NavigateTo(Type sourcePage, object parameter)
        {
            Frame.Navigate(sourcePage, parameter);
        }
    }
}
