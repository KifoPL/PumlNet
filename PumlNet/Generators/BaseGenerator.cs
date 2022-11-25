using System.Text;
using PumlNet.Options;

namespace PumlNet.Generators;

public abstract class BaseGenerator
{
    protected readonly StringBuilder Sb;
    protected readonly PumlOptions Options;

    protected BaseGenerator(StringBuilder sb, int indentationLevel, PumlOptions options)
    {
        Sb = sb;
        IndentationLevel = indentationLevel;
        Options = options;
    }

    public int IndentationLevel { get; } = 0;
}