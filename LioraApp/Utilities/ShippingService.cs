namespace LioraApp.Utilities;

/// <summary>
/// Computes shipping cost server-side from configuration and governorate lookup.
/// Shipping prices are sourced from appsettings.json ("Shipping" section) so
/// they can be changed without redeploying code.
/// </summary>
public class ShippingService : IShippingService
{
    private readonly IConfiguration _config;

    // Per-governorate rates (EGP). A single source of truth — the Checkout
    // view must use a GET endpoint to display the estimate, not this dictionary.
    // Keep in sync with Checkout.cshtml JS only for UX preview.
    private static readonly Dictionary<string, decimal> _governorateRates =
        new(StringComparer.OrdinalIgnoreCase)
        {
            { "Cairo",        50m  },
            { "Giza",         50m  },
            { "Alexandria",   70m  },
            { "Dakahlia",     80m  },
            { "Red Sea",     100m  },
            { "Beheira",      80m  },
            { "Fayoum",       80m  },
            { "Gharbia",      80m  },
            { "Ismailia",     80m  },
            { "Menofia",      80m  },
            { "Minya",        90m  },
            { "Qaliubiya",    60m  },
            { "New Valley",  120m  },
            { "Suez",         80m  },
            { "Aswan",       120m  },
            { "Assiut",      100m  },
            { "Beni Suef",    90m  },
            { "Port Said",    80m  },
            { "Damietta",     80m  },
            { "Sharkia",      80m  },
            { "South Sinai", 100m  },
            { "Kafr Al sheikh", 80m},
            { "Matrouh",     100m  },
            { "Luxor",       120m  },
            { "Qena",        110m  },
            { "North Sinai", 100m  },
            { "Sohag",       100m  },
        };

    public ShippingService(IConfiguration config) => _config = config;

    /// <inheritdoc/>
    public decimal CalculateShipping(decimal orderSubtotal, string? governorate = null)
    {
        // Free shipping above configured threshold
        var threshold = _config.GetValue<decimal>("Shipping:FreeShippingThresholdEGP", 0m);
        if (threshold > 0 && orderSubtotal >= threshold)
            return 0m;

        // Governorate-specific rate
        if (!string.IsNullOrWhiteSpace(governorate) &&
            _governorateRates.TryGetValue(governorate.Trim(), out var rate))
        {
            return rate;
        }

        // Fall back to configured default
        return _config.GetValue<decimal>("Shipping:DefaultCostEGP", 100m);
    }
}
