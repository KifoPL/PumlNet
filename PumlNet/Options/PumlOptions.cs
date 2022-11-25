using PumlNet.Options.DiagramElements;
using PumlNet.Options.Namespace;
using PumlNet.Options.Styles;

namespace PumlNet.Options;

public sealed class PumlOptions
{
    public DiagramElementOptions DiagramElements { get; set; } = new();
    public StyleOptions StyleOptions { get; set; } = new();
    public IncludeOptions IncludeOptions { get; set; } = new();
}