namespace kafi.Models;

public enum TableStatus
{
    Available,
    Selected,
    Ordered,
}

public class Table
{
    public int Id;
    public string? Name;
    public TableStatus Status = TableStatus.Available;
}
