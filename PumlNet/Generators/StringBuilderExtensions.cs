using System.Text;
using PumlNet.Options;
using PumlNet.Options.DiagramElements;
using PumlNet.Options.Styles;

namespace PumlNet.Generators;

internal static class StringBuilderExtensions
{
    internal static void AppendIndent(this StringBuilder sb, int indent, PumlOptions options)
    {
        if (indent <= 0) return;
        
        switch (options.StyleOptions.IndentType)
        {
            case IndentType.Tab:
                sb.AppendIndent(indent,
                                '\t',
                                1);
                break;
            case IndentType.TwoSpaces:
                sb.AppendIndent(indent,
                                ' ',
                                2);
                break;
            case IndentType.FourSpaces:
                sb.AppendIndent(indent,
                                ' ',
                                4);
                break;
            default: throw new ArgumentOutOfRangeException();
        }
    }

    private static void AppendIndent(this StringBuilder sb,
                                     int indent,
                                     char indentChar,
                                     int indentSize)
    {
        for (int i = 0; i < indent; i++)
        {
            for (int j = 0; j < indentSize; j++) sb.Append(indentChar);
        }
    }

    private static void AppendDiagramElement(this StringBuilder sb, DiagramElement? diagramElement, string name)
    {
        if (diagramElement is null) return;

        sb.AppendLine(name);
        sb.AppendLine(diagramElement.ToString());
        sb.AppendLine($"end {name}");
        sb.AppendLine();
    }

    internal static void AppendDiagramElements(this StringBuilder sb, DiagramElementOptions diagramElementOptions)
    {
        sb.AppendDiagramElement(diagramElementOptions.Title, "title");
        sb.AppendDiagramElement(diagramElementOptions.Header, "header");
        sb.AppendDiagramElement(diagramElementOptions.Footer, "footer");
        sb.AppendDiagramElement(diagramElementOptions.Caption, "caption");
        sb.AppendDiagramElement(diagramElementOptions.Legend, "legend");
    }
}