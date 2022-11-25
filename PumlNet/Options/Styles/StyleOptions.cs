namespace PumlNet.Options.Styles;

public class StyleOptions
{
    public IndentType IndentType { get; set; } = IndentType.Tab;
    public bool HideEmptyMembers { get; set; } = true;
    public bool HideEmptyMethods { get; set; } = false;
    public bool HideEmptyProperties { get; set; } = false;
}