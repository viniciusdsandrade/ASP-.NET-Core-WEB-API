using System.ComponentModel.DataAnnotations;

namespace APICatalog.Validations;

public class PriceValidationAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value == null) return new ValidationResult("O preço é obrigatório.");
        var price = (decimal)value;
        return price < 0 ? new ValidationResult("O preço não pode ser negativo.") : ValidationResult.Success;
    }
}