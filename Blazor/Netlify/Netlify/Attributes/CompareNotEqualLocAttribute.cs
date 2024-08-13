using System.ComponentModel.DataAnnotations;

using Netlify.SharedResources;

namespace Netlify.Attributes;

public class CompareNotEqualLocAttribute : ValidationAttribute
{
    private readonly string _otherProperty;

    private readonly string _message;

    public CompareNotEqualLocAttribute(string otherProperty, string message)
    {
        _otherProperty = otherProperty;
        _message = message;
    }

    protected override ValidationResult IsValid(object value, ValidationContext context)
    {
        var property = context.ObjectType.GetProperty(_otherProperty);
        if (property == null)
        {
            return new ValidationResult(string.Format(StaticLocalizer.GetString("Property '{0}' not found."), _otherProperty));
        }

        var otherValue = property.GetValue(context.ObjectInstance);
        if (value.Equals(otherValue))
        {
            //return new ValidationResult($"'{_otherProperty}' and '{context.MemberName}' cannot be equal.");
            return new ValidationResult(StaticLocalizer.GetString(_message));
        }

        return ValidationResult.Success;
    }
    
}
