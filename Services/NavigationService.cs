using System;
using kafi.Contracts.Services;
using Microsoft.UI.Xaml.Controls;

namespace kafi.Service
{
    public class NavigationService : INavigationService
    {
        private Frame _frame;

        public void Initialize(Frame frame)
        {
            _frame = frame;
        }

        public void GoBack()
        {
            _frame.GoBack();
        }

        public void NavigateTo(Type sourcePage)
        {
            _frame.Navigate(sourcePage);
        }

        public void NavigateTo(Type sourcePage, object parameter)
        {
            _frame.Navigate(sourcePage, parameter);
        }
    }
}
