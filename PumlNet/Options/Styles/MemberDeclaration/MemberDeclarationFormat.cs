namespace PumlNet.Options.Styles.MemberDeclaration;

public static class MemberDeclarationFormat
{
    public const string Uml = $"{{{MemberDeclarationKeys.Name}}} : {{{MemberDeclarationKeys.Type}}}";
    public const string DotNet = $"{{{MemberDeclarationKeys.Type}}} {{{MemberDeclarationKeys.Name}}}";
    public const string TypeScript = $"{{{MemberDeclarationKeys.Name}}}: {{{MemberDeclarationKeys.Type}}}";
    public const string NoTypes = $"{{{MemberDeclarationKeys.Name}}}";

    public static string GetFormat(MemberDeclarationStyle style)
        => style switch
        {
            MemberDeclarationStyle.Uml        => Uml,
            MemberDeclarationStyle.DotNet     => DotNet,
            MemberDeclarationStyle.TypeScript => TypeScript,
            MemberDeclarationStyle.NoTypes    => NoTypes,
            MemberDeclarationStyle.Custom     => "CUSTOM OVERRIDE STRING",
            _ => throw new ArgumentOutOfRangeException(nameof(style),
                                                       style,
                                                       null)
        };
}