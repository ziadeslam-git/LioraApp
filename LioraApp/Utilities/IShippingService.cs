namespace LioraApp.Utilities;

/// <summary>
/// Calculates the shipping cost for a customer order.
/// </summary>
public interface IShippingService
{
    /// <summary>
    /// Returns the shipping cost in EGP based on the order subtotal and the
    /// destination governorate. The result is always computed server-side —
    /// never trust a value submitted by the client.
    /// </summary>
    decimal CalculateShipping(decimal orderSubtotal, string? governorate = null);
}
