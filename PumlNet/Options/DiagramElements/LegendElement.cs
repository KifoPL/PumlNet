namespace PumlNet.Options.DiagramElements;

public class LegendElement : DiagramElement
{
    public static implicit operator LegendElement(string value) => new(value);
    public static implicit operator LegendElement(PumlString value) => new(value);

    /// <inheritdoc />
    public LegendElement(PumlString content) : base(content) { }
}