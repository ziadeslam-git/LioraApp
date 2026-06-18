namespace LioraApp.ViewModels.Customer;

public class CheckoutVM
{
    public List<CheckoutItemCustomerVM> Items { get; set; } = [];
    public List<AddressOptionCustomerVM> Addresses { get; set; } = [];
    public int? DefaultAddressId { get; set; }
    public string? CouponCode { get; set; }
    public string PaymentMethod { get; set; } = "CashOnDelivery";
    public bool CouponApplied { get; set; }
    public string? CouponMessage { get; set; }
    public int? AddressId { get; set; }
    public bool ShowNewAddressForm { get; set; }
    public bool SaveNewAddress { get; set; } = true;
    public string NewAddressFullName { get; set; } = string.Empty;
    public string NewAddressPhoneNumber { get; set; } = string.Empty;
    public string NewAddressStreet { get; set; } = string.Empty;
    public string NewAddressCity { get; set; } = string.Empty;
    public string? NewAddressState { get; set; }
    public string NewAddressCountry { get; set; } = string.Empty;
    public string NewAddressPostalCode { get; set; } = string.Empty;
    public decimal Subtotal { get; set; }
    public decimal DiscountAmount { get; set; }

    /// <summary>
    /// Shipping cost calculated server-side for display purposes on the Checkout page.
    /// This value is NEVER trusted on POST — it is always re-computed by <see cref="IShippingService"/>.
    /// </summary>
    public decimal ShippingCost { get; set; }

    /// <summary>
    /// Receipt image uploaded by the customer as proof-of-payment for manual transfers
    /// (VodafoneCash / InstaPay). Validated server-side with magic-byte inspection.
    /// Not serialized to TempData — only present on the POST request.
    /// </summary>
    [System.Text.Json.Serialization.JsonIgnore]
    public IFormFile? ReceiptImage { get; set; }

    public decimal Total => Subtotal - DiscountAmount;
    public int ItemsCount => Items.Sum(i => i.Quantity);

    public bool HasNewAddressInput =>
        !string.IsNullOrWhiteSpace(NewAddressFullName) ||
        !string.IsNullOrWhiteSpace(NewAddressPhoneNumber) ||
        !string.IsNullOrWhiteSpace(NewAddressStreet) ||
        !string.IsNullOrWhiteSpace(NewAddressCity) ||
        !string.IsNullOrWhiteSpace(NewAddressState) ||
        !string.IsNullOrWhiteSpace(NewAddressPostalCode);
}

public class CheckoutItemCustomerVM
{
    public int CartItemId { get; set; }
    public int? ProductVariantId { get; set; }
    public int? GiftBundleId { get; set; }
    public string? GiftBundleTitle { get; set; }
    public decimal? GiftBundleOriginalTotal { get; set; }
    public List<GiftBundleCheckoutProductVM> BundleItems { get; set; } = [];
    public bool IsGiftBundle => GiftBundleId.HasValue;
    public int Quantity { get; set; }
    public decimal PriceSnapshot { get; set; }
    public decimal CurrentPrice { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string Size { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public int Stock { get; set; }

    public decimal DisplayPrice => CurrentPrice > 0 ? CurrentPrice : PriceSnapshot;
    public decimal LineTotal => Quantity * DisplayPrice;
}

public class GiftBundleCheckoutProductVM
{
    public string ProductName { get; set; } = string.Empty;
    public string Size { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
}

public class AddressOptionCustomerVM
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Street { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string? State { get; set; }
    public string Country { get; set; } = string.Empty;
    public string PostalCode { get; set; } = string.Empty;
    public bool IsDefault { get; set; }
    public string DisplayLine { get; set; } = string.Empty;
}

