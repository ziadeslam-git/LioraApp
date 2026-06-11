namespace LioraApp.Utilities.Validation;

public sealed record PhoneNumberValidationResult(
    bool IsValid,
    string? E164Number,
    string? RegionCode,
    string? ErrorMessage);
