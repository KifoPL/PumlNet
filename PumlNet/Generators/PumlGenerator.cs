using System.Text;
using PumlNet.Options;

namespace PumlNet.Generators;

internal sealed class PumlGenerator : BaseGenerator
{
    /// <inheritdoc />
    public PumlGenerator(StringBuilder sb, PumlOptions options) : base(sb,
                                                                       0,
                                                                       options)
    {
    }
}