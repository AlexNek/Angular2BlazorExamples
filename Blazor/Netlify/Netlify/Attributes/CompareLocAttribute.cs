﻿using System.ComponentModel.DataAnnotations;

using Netlify.SharedResources;

namespace Netlify.Attributes;

[AttributeUsage(AttributeTargets.Property)]
public class CompareLocAttribute : CompareAttribute
{
    private readonly string _message;

    /// <summary>Initializes a new instance of the <see cref="T:System.ComponentModel.DataAnnotations.CompareAttribute" /> class.</summary>
    /// <param name="password"></param>
    /// <param name="otherProperty">The property to compare with the current property.</param>
    public CompareLocAttribute(string otherProperty, string message)
        : base(otherProperty)
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