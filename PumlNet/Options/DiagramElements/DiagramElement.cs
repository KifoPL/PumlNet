namespace PumlNet.Options.DiagramElements;

public abstract class DiagramElement
{
    public PumlString Content { get; set; }

    protected DiagramElement(PumlString content) { Content = content; }
    
    public override string ToString() => Content.ToString();
}