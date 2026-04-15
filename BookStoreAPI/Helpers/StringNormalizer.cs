using System.Text.RegularExpressions;

namespace BookStoreAPI.Helpers;

public static class StringNormalizer
{
    private static readonly Dictionary<char, char> AccentMap = new()
    {
        ['á'] = 'A', ['à'] = 'A', ['ä'] = 'A', ['â'] = 'A', ['ã'] = 'A', ['å'] = 'A',
        ['Á'] = 'A', ['À'] = 'A', ['Ä'] = 'A', ['Â'] = 'A', ['Ã'] = 'A', ['Å'] = 'A',
        ['é'] = 'E', ['è'] = 'E', ['ë'] = 'E', ['ê'] = 'E',
        ['É'] = 'E', ['È'] = 'E', ['Ë'] = 'E', ['Ê'] = 'E',
        ['í'] = 'I', ['ì'] = 'I', ['ï'] = 'I', ['î'] = 'I',
        ['Í'] = 'I', ['Ì'] = 'I', ['Ï'] = 'I', ['Î'] = 'I',
        ['ó'] = 'O', ['ò'] = 'O', ['ö'] = 'O', ['ô'] = 'O', ['õ'] = 'O', ['ø'] = 'O',
        ['Ó'] = 'O', ['Ò'] = 'O', ['Ö'] = 'O', ['Ô'] = 'O', ['Õ'] = 'O', ['Ø'] = 'O',
        ['ú'] = 'U', ['ù'] = 'U', ['ü'] = 'U', ['û'] = 'U',
        ['Ú'] = 'U', ['Ù'] = 'U', ['Ü'] = 'U', ['Û'] = 'U',
        ['ñ'] = 'N', ['Ñ'] = 'N',
        ['ç'] = 'C', ['Ç'] = 'C',
        ['ý'] = 'Y', ['ÿ'] = 'Y', ['Ý'] = 'Y'
    };

    private static readonly Regex DigitsRegex = new(@"\d", RegexOptions.Compiled);
    private static readonly Regex MultiSpaceRegex = new(@"\s+", RegexOptions.Compiled);

    public static string Normalize(string input)
    {
        if (string.IsNullOrEmpty(input))
            return string.Empty;

        var upper = input.ToUpperInvariant();

        var noDigits = DigitsRegex.Replace(upper, string.Empty);

        var chars = noDigits.Select(c => AccentMap.TryGetValue(c, out var replacement) ? replacement : c);
        var replaced = new string(chars.ToArray());

        var singleSpaced = MultiSpaceRegex.Replace(replaced, " ");

        return singleSpaced.Trim();
    }
}
