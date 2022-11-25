namespace PumlNet.Options.DiagramElements;

public class FooterElement : DiagramElement
{
    public static implicit operator FooterElement(string value) => new(value);

    /// <inheritdoc />
    public FooterElement(PumlString content) : base(content)
    {
    }
}