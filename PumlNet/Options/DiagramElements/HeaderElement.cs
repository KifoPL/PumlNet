namespace PumlNet.Options.DiagramElements;

public class HeaderElement : DiagramElement
{
    public static implicit operator HeaderElement(string value) => new(value);
    public static implicit operator HeaderElement(PumlString value) => new(value);

    /// <inheritdoc />
    public HeaderElement(PumlString content) : base(content) { }
}