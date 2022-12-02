namespace PumlNet.Helpers;

internal static class PumlStringExtensions
{
    private static readonly Dictionary<string, string> EscapingRulesSingleLine = new()
    {
        { "\n", "\\n" }
    };

    private static readonly Dictionary<string, string> EscapingRulesMultiline = new()
    {
        { "\\", "\\\\" },
        { "\"", "\\\"" },
        { "\t", "\\t" },
        { "\r", "\\r" }
    };

    private static readonly Dictionary<string, string> EscapingRules = EscapingRulesSingleLine
                                                                      .Concat(EscapingRulesMultiline)
                                                                      .ToDictionary(x => x.Key, x => x.Value);

    internal static string Escape(this string unescapedString, EscapeType escapeType)
        => escapeType switch
        {
            EscapeType.None       => unescapedString,
            EscapeType.SingleLine => Escape(EscapingRules, unescapedString),
            EscapeType.MultiLine  => Escape(EscapingRulesMultiline, unescapedString),
            _ => throw new ArgumentOutOfRangeException(nameof(escapeType),
                                                       escapeType,
                                                       null)
        };

    private static string Escape(Dictionary<string, string> rules, string unescapedString)
        => rules.Aggregate(unescapedString, (current, rule) => current.Replace(rule.Key, rule.Value));
}