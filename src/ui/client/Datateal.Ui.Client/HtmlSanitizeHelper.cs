using Ganss.Xss;

namespace Datateal.Ui.Client;

public static class HtmlSanitizeHelper
{
    private static readonly HtmlSanitizer Sanitizer = CreateSanitizer();

    public static string Sanitize(string html) => Sanitizer.Sanitize(html);

    private static HtmlSanitizer CreateSanitizer()
    {
        var sanitizer = new HtmlSanitizer();
        // The default allowlist is already sensible (allows formatting, tables, images, etc.)
        // and strips <script>, <iframe>, event handlers, javascript: URLs.
        return sanitizer;
    }
}
