﻿using Microsoft.AspNetCore.Localization;
using System.Globalization;

// Middleware to set the culture based on user preferences or request
namespace Netlify.Middlware;

public class CultureMiddleware
{
    private readonly RequestDelegate _next;

    public CultureMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        var requestCookie = httpContext?.Request.Cookies[CookieRequestCultureProvider.DefaultCookieName];
        if (requestCookie != null)
        {
            var cultureResult = CookieRequestCultureProvider.ParseCookieValue(requestCookie);
            if (cultureResult != null && cultureResult.Cultures.Any())
            {
                var currentCulture = cultureResult.Cultures[0];
                CultureInfo cultureInfo = new CultureInfo(currentCulture.ToString());
                CultureInfo.CurrentCulture = cultureInfo;
                CultureInfo.CurrentUICulture = cultureInfo;
            }
        }
        
        await _next(httpContext);
    }
}