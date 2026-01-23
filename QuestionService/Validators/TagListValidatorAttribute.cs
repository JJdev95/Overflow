using System.ComponentModel.DataAnnotations;

namespace QuestionService.Validators;

[AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)]
public class TagListValidatorAttribute(int min, int max) : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is List<string> tags && tags.Count >= min && tags.Count <= max) return ValidationResult.Success;

        return new ValidationResult($"You must provide at least {min} and {max} tags.");
    }
}