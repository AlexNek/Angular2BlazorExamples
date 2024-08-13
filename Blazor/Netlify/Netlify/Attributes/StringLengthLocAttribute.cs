using System.ComponentModel.DataAnnotations;

using Netlify.SharedResources;

namespace Netlify.Attributes;

[AttributeUsage(AttributeTargets.Property)]
public class StringLengthLocAttribute : StringLengthAttribute
{
    private readonly string _message;

    /// <summary>Initializes a new instance of the <see cref="StringLengthLocAttribute"/> class.</summary>
    /// <param name="maximumLength">The maximum length allowed.</param>
    /// <param name="message">The error message format string.</param>
    public StringLengthLocAttribute(int maximumLength, string message)
        : base(maximumLength)
    {
        _message = message;
    }

    /// <summary>Applies formatting to an error message, based on the data field where the error occurred.</summary>
    /// <param name="name">The name of the field that caused the validation failure.</param>
    /// <returns>The formatted error message.</returns>
    public override string FormatErrorMessage(string name)
    {
        return StaticLocalizer.GetString(_message);
    }
}