using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace VirtualGlassesProvider.Services
{
    public class FutureDateAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is string stringValue)
            {
                if (DateTime.TryParseExact(stringValue, "MM/yy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dateValue))
                {
                    if (dateValue > DateTime.UtcNow)
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
                    return new ValidationResult("Invalid date format.");
                }
            }
            return new ValidationResult("Invalid input.");
        }
    }
}
