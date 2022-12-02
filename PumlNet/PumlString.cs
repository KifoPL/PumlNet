using PumlNet.Associations;
using PumlNet.Helpers;

namespace PumlNet;

[AssociationIgnore]
public readonly record struct PumlString
{
    public PumlString(string value, EscapeType escapeType = EscapeType.MultiLine)
    {
        EscapeType = escapeType;
        Value = value.Escape(escapeType);
    }

    public static implicit operator PumlString(string value) => new(value);
    public string Value { get; init; }
    public EscapeType EscapeType { get; init; }

    public override string ToString() => Value;
}

public enum EscapeType { None, SingleLine, MultiLine }