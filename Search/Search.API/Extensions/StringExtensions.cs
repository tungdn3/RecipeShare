namespace Search.API.Extensions;

public static class StringExtensions
{
    public static string ToCamelCase(this string str)
    {
        var words = str.Split(new[] { "_", " " }, StringSplitOptions.RemoveEmptyEntries);
        var leadWord = words[0].ToLower();
        var tailWords = words.Skip(1)
            .Select(word => char.ToUpper(word[0]) + word.Substring(1))
            .ToArray();

        return $"{leadWord}{string.Join(string.Empty, tailWords)}";
    }
}
