namespace PumlNet.Options.DiagramElements;

public class TitleElement : DiagramElement
{
    public static implicit operator TitleElement(string value) => new(value);
    public static implicit operator TitleElement(PumlString value) => new(value);

    /// <inheritdoc />
    public TitleElement(PumlString content) : base(content) { }
}