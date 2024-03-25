namespace Bookery.Common.Extensions;

public static class StringExtensions
{
    public static string WithTrailingSlash(this string value) => value.TrimEnd('/') + "/";
}