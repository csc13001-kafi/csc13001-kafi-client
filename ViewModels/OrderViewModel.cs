using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using kafi.Models;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Linq;
using kafi.Repositories;

namespace kafi.ViewModels
{
    public partial class OrderViewModel : ObservableObject
    {
        private readonly IOrderRepository _orderRepository;

        public ObservableCollection<Order> Orders
        {
            get; private set;
        }

        [ObservableProperty]
        private bool isLoading;

        public OrderViewModel(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
            Orders = new ObservableCollection<Order>();
        }

        [RelayCommand]
        private async Task LoadData()
        {
            IsLoading = true;
            try
            {
                var orders = await _orderRepository.GetAll();
                Orders.Clear();
                foreach (var order in orders)
                {
                    Orders.Add(order);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading data: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }
    }
}
