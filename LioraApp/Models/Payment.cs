namespace LioraApp.Models;

public class Payment
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public decimal Amount { get; set; }
    public string Provider { get; set; } = "Manual";

    /// <summary>
    /// Payment gateway transaction reference (e.g. Stripe charge ID).
    /// Null for manual / cash-on-delivery payments.
    /// </summary>
    public string? TransactionId { get; set; }

    /// <summary>
    /// Public URL of the customer-uploaded proof-of-payment screenshot.
    /// Stored separately from <see cref="TransactionId"/> to avoid semantic confusion.
    /// </summary>
    public string? ReceiptImageUrl { get; set; }

    /// <summary>
    /// Cloudinary public ID for the receipt image — used to delete the asset
    /// when the order is cancelled or the receipt is replaced.
    /// </summary>
    public string? ReceiptPublicId { get; set; }

    public string Status { get; set; } = "Pending";    // SD.Payment_*
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public Order Order { get; set; } = null!;
}
