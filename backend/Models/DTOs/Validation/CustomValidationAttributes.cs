// backend/Models/DTOs/Validation/CustomValidationAttributes.cs
using System.ComponentModel.DataAnnotations;

namespace backend.Models.DTOs.Validation
{
    public class ProjectNumberAttribute : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            if (value is not string projectNumber)
                return false;

            // Project number should be uppercase alphanumeric with hyphens/underscores
            return System.Text.RegularExpressions.Regex.IsMatch(
                projectNumber, 
                @"^[A-Z0-9\-_]+$"
            );
        }

        public override string FormatErrorMessage(string name)
        {
            return $"{name} must contain only uppercase letters, numbers, hyphens, and underscores.";
        }
    }

    public class FutureDateAttribute : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            if (value is not DateTime date)
                return true; // Allow null/empty values

            return date > DateTime.UtcNow;
        }

        public override string FormatErrorMessage(string name)
        {
            return $"{name} must be a future date.";
        }
    }

    public class DateRangeAttribute : ValidationAttribute
    {
        private readonly string _comparisonProperty;
        private readonly bool _allowEqual;

        public DateRangeAttribute(string comparisonProperty, bool allowEqual = true)
        {
            _comparisonProperty = comparisonProperty;
            _allowEqual = allowEqual;
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var currentValue = value as DateTime?;
            var property = validationContext.ObjectType.GetProperty(_comparisonProperty);
            
            if (property == null)
                throw new ArgumentException($"Property {_comparisonProperty} not found");

            var comparisonValue = property.GetValue(validationContext.ObjectInstance) as DateTime?;

            if (currentValue == null || comparisonValue == null)
                return ValidationResult.Success;

            bool isValid = _allowEqual 
                ? currentValue >= comparisonValue 
                : currentValue > comparisonValue;

            if (!isValid)
            {
                return new ValidationResult(
                    ErrorMessage ?? $"{validationContext.DisplayName} must be greater than {_comparisonProperty}",
                    new[] { validationContext.MemberName! }
                );
            }

            return ValidationResult.Success;
        }
    }
}