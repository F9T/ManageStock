using System.Globalization;
using System.Windows.Controls;

namespace ManageStock.ValidationsRules
{
    internal class PriceValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (value == null)
            {
                return new ValidationResult(false, "Le prix entré est incorrect.");
            }

            if (!double.TryParse(value.ToString(), NumberStyles.Any, null, out double price))
            {
                return new ValidationResult(false, "Le format du prix est incorrect.");
            }

            if (price < 0)
            {
                return new ValidationResult(false, "Le prix ne peut pas être négatif.");
            }

            return ValidationResult.ValidResult;
        }
    }
}
