namespace LioraApp.Utilities;

public static class SD
{
    // ──────────────── Roles ────────────────
    public const string Role_Admin    = "Admin";
    public const string Role_Customer = "Customer";
    public const string Role_AdminOrCustomer = Role_Admin + "," + Role_Customer;

    // ──────────────── Order Status ─────────────────
    public const string Status_Pending    = "Pending";
    public const string Status_Confirmed  = "Confirmed";
    public const string Status_Processing = "Processing";
    public const string Status_Shipped    = "Shipped";
    public const string Status_Delivered  = "Delivered";
    public const string Status_Cancelled  = "Cancelled";

    // ──────────────── Payment Status ───────────────
    public const string Payment_Unpaid   = "Unpaid";
    public const string Payment_Paid     = "Paid";
    public const string Payment_Refunded = "Refunded";
    public const string Payment_Failed   = "Failed";
    public const string Payment_Pending  = "Pending";

    // ──────────────── Manual Payment Methods ───────
    public const string PaymentMethod_CashOnDelivery = "CashOnDelivery";
    public const string PaymentMethod_VodafoneCash   = "VodafoneCash";
    public const string PaymentMethod_InstaPay       = "InstaPay";
    public const string PaymentProvider_Manual       = "Manual";

    // ──────────────── Discount Type ────────────────
    public const string Discount_Percentage   = "Percentage";
    public const string Discount_FixedAmount  = "FixedAmount";

    // ──────────────── Image Upload ─────────────────
    public const string Cloudinary_ProductFolder = "ecommerce/products";
    public const string Cloudinary_ProfileFolder = "ecommerce/profiles";
}
