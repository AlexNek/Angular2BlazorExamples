namespace Netlify.Attributes;

[AttributeUsage(AttributeTargets.Property)]
public class PasswordLengthDefaultLoc: StringLengthLocAttribute
{
    
    public PasswordLengthDefaultLoc()
        : base(100, "The {0} must be at least {2} and at max {1} characters long.")
    {
        MinimumLength = 8;
    }
}
