namespace PumlNet.Options.DiagramElements;

public class CaptionElement : DiagramElement
{
    public static implicit operator CaptionElement(string value) => new(value);
    public static implicit operator CaptionElement(PumlString value) => new(value);

    /// <inheritdoc />
    public CaptionElement(PumlString content) : base(content) { }
}