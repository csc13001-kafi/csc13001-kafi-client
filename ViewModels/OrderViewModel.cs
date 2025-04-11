using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Messaging.Messages;
using kafi.Models;
using kafi.Repositories;

namespace kafi.ViewModels;

public partial class OrderViewModel : ObservableRecipient, IRecipient<ValueChangedMessage<string>>
{
    private readonly IOrderRepository _orderRepository;
    private const int DefaultPageSize = 10;

    private List<Order> _orders = [];

    public OrderViewModel(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
        IsActive = true;
    }

    public ObservableCollection<Order> Orders { get; set; } = [];

    [ObservableProperty]
    public partial bool IsLoading { get; set; }

    [ObservableProperty]
    public partial bool IsBillLoading { get; set; }

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(GoToPreviousPageCommand))]
    [NotifyCanExecuteChangedFor(nameof(GoToNextPageCommand))]
    public partial int CurrentPage { get; set; } = 1;

    [ObservableProperty]
    public partial int TotalPages { get; set; } = 1;

    [ObservableProperty]
    public partial int PageSize { get; set; } = DefaultPageSize;

    [ObservableProperty]
    public partial int TotalItems { get; set; } = 0;

    [ObservableProperty]
    public partial Order SelectedOrder { get; set; }

    private bool CanLoadData => !Orders.Any();
    [RelayCommand(CanExecute = nameof(CanLoadData))]
    private async Task LoadDataAsync()
    {
        IsLoading = true;
        try
        {
            _orders = [.. await _orderRepository.GetAll()];
            UpdatePagedView();
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

    private bool CanViewOrder(Guid id) => id != Guid.Empty;
    [RelayCommand(CanExecute = nameof(CanViewOrder))]
    private async Task ViewOrderAsync(Guid id)
    {
        IsBillLoading = true;
        try
        {
            SelectedOrder = await _orderRepository.GetById(id)!;
            if (SelectedOrder == null)
            {
                System.Diagnostics.Debug.WriteLine("Order not found");
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error loading order: {ex.Message}");
        }
        finally
        {
            IsBillLoading = false;
        }
    }

    private void UpdatePagedView()
    {
        TotalItems = _orders.Count;
        CurrentPage = Math.Clamp(CurrentPage, 1, TotalPages);

        TotalPages = (int)Math.Ceiling((double)TotalItems / PageSize);
        TotalPages = TotalPages == 0 ? 1 : TotalPages;

        var pagedItems = _orders
            .Skip((CurrentPage - 1) * PageSize)
            .Take(PageSize);

        Orders.Clear();
        foreach (var item in pagedItems)
        {
            Orders.Add(item);
        }

        GoToPreviousPageCommand.NotifyCanExecuteChanged();
        GoToNextPageCommand.NotifyCanExecuteChanged();
    }

    private bool CanGoToPreviousPage => CurrentPage > 1;
    [RelayCommand(CanExecute = nameof(CanGoToPreviousPage))]
    private void GoToPreviousPage()
    {
        CurrentPage--;
        UpdatePagedView();
    }

    private bool CanGoToNextPage => CurrentPage < TotalPages;
    [RelayCommand(CanExecute = nameof(CanGoToNextPage))]
    private void GoToNextPage()
    {
        CurrentPage++;
        UpdatePagedView();
    }

    partial void OnPageSizeChanged(int value)
    {
        UpdatePagedView();
    }

    public void Receive(ValueChangedMessage<string> message)
    {
        if (message.Value == "ordercreated")
        {
            LoadDataAsync().ConfigureAwait(false);
        }
    }
}
