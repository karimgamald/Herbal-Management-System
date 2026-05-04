using PhytoIntellect.Application.Interfaces;

namespace PhytoIntellect.Api.Extensions;

public class LanguageMiddleware
{
    private readonly RequestDelegate _next;

    public LanguageMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, ICurrentLanguageService languageService)
    {
        var langHeader = context.Request.Headers["Accept-Language"].ToString();

        if (!string.IsNullOrWhiteSpace(langHeader))
        {
            var lang = langHeader.Substring(0, 2).ToLower();

            if (lang == "ar" || lang == "en")
            {
                languageService.LanguageCode = lang;
            }
        }

        await _next(context);
    }
}