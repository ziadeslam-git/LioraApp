namespace LioraApp.Models;

public class Payment
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public decimal Amount { get; set; }
    public string Provider { get; set; } = "Manual";
    public string? TransactionId { get; set; }
    public string Status { get; set; } = "Pending";    // SD.Payment_*
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public Order Order { get; set; } = null!;
}
