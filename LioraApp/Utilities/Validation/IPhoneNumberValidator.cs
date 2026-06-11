namespace LioraApp.Utilities.Validation;

public interface IPhoneNumberValidator
{
    PhoneNumberValidationResult ValidateAndFormat(string? phoneNumber, string? regionCode, bool isRequired);
}
