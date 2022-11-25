namespace PumlNet.Options.Styles.MemberDeclaration;

public class MemberDeclarationOptions
{
    public MemberDeclarationOptions() : this(MemberDeclarationStyle.Uml) {
    }

    public MemberDeclarationOptions(MemberDeclarationStyle style) { Style = style; }

    private MemberDeclarationStyle _style = MemberDeclarationStyle.Uml;

    public MemberDeclarationStyle Style
    {
        get => _style;
        set
        {
            _style = value;
            if (value is not MemberDeclarationStyle.Custom)
            {
                Format = MemberDeclarationFormat.GetFormat(value);
            }
        }
    }

    private string _format = MemberDeclarationFormat.GetFormat(MemberDeclarationStyle.Uml);

    public string Format
    {
        get => _format;
        set
        {
            _format = value;
            _style = MemberDeclarationStyle.Custom;
        }
    }
}