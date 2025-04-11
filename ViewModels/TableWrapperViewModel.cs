using CommunityToolkit.Mvvm.ComponentModel;
using kafi.Models;

namespace kafi.ViewModels;

public partial class TableWrapperViewModel(Table model) : ObservableObject
{
    private Table Model { get; } = model;

    public int Id => Model.Id;
    public string? Name => Model.Name;

    [ObservableProperty]
    public partial TableStatus Status { get; set; } = model.Status;
}
