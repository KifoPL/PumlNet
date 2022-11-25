using System.Drawing;
using PumlNet.Options.Styles.MemberDeclaration;

namespace PumlNet.Options.Styles;

public class StyleOptions
{
    public Direction? Direction { get; set; }
    public IndentType IndentType { get; set; } = IndentType.Tab;
    public bool MarkAsStatic { get; set; } = true;
    public Color StaticColor { get; set; } = Color.Goldenrod;
    public bool HideEmptyMembers { get; set; } = true;
    public bool HideEmptyMethods { get; set; } = false;
    public bool HideEmptyProperties { get; set; } = false;
    public MemberDeclarationOptions MemberDeclarationOptions { get; set; } = new();
}