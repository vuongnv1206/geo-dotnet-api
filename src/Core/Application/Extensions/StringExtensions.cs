namespace FSH.WebApi.Application.Extensions;
public static class StringExtensions
{
    public static List<string> SplitStringExtension(this string content, string splitString)
    {
        return content.Split(splitString, StringSplitOptions.TrimEntries).ToList();
    }
}
