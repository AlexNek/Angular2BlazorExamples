using System.ComponentModel.DataAnnotations;
using Netlify.SharedResources;

namespace Netlify.Attributes;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public class MustBeTrueLocAttribute : ValidationAttribute
{
    private readonly string _message;

    public MustBeTrueLocAttribute(string message)
    {
        _message = message;
    }
    public override bool IsValid(object? value)
    {
        // Check if value is a boolean and equals true
        return value is true;
    }

    public override string FormatErrorMessage(string name)
    {
        // Use the static localizer to get the localized error message
        return StaticLocalizer.GetString(_message); ;
    }
}
