using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace A3.RouteSearchGraphs.Attributes;

public class AllowedLanguageAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is string language)
        {
            var cultures = CultureInfo
                .GetCultures(CultureTypes.SpecificCultures)
                .Where(culture => culture.Name is "en-US" or "pt-BR")
                .Select(culture => culture.Name)
                .ToImmutableArray();

            if (cultures.Contains(language))
            {
                return ValidationResult.Success;
            }
        }

        return new ValidationResult("Supported languages are 'en-US' or 'pt-BR'");
    }
}