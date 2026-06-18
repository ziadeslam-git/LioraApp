namespace LioraApp.ViewModels.Admin;

/// <summary>
/// Lightweight projection used by the Dashboard's "Recent Orders" table.
/// Avoids passing raw <see cref="LioraApp.Models.Order"/> entities to the view.
/// </summary>
public class RecentOrderVM
{
    public int      Id           { get; set; }
    public string   CustomerName { get; set; } = string.Empty;
    public DateTime CreatedAt    { get; set; }
    public decimal  TotalAmount  { get; set; }
    public string   Status       { get; set; } = string.Empty;
}
