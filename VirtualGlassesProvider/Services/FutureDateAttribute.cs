using System.ComponentModel.DataAnnotations;
using System.Globalization;


namespace VirtualGlassesProvider.Services
{
    public class FutureDateAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is string stringValue)
            {
                // Ensure the format is MM/yy
                if (DateTime.TryParseExact(stringValue, "MM/yy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dateValue))
                {
                    // Compare with the last day of the month
                    if (dateValue.AddMonths(1).AddDays(-1) >= DateTime.UtcNow.Date)
                    {
                        return ValidationResult.Success;
                    }
                    else
                    {
                        return new ValidationResult("Expiry date cannot be in the past.");
                    }
                }
                else
                {
                    return new ValidationResult("Invalid date format. Use MM/YY.");
                }
            }
            return new ValidationResult("Invalid input.");
        }
    }
}
