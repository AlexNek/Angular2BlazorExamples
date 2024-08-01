﻿namespace Netlifly.Shared;

public class InternalLoginRequest
{
    public string Username { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public bool RememberMe { get; set; }

    public string Locale { get; set; } // Add locale property
}