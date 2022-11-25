using System.Reflection;
using System.Text;
using PumlNet.Helpers;
using PumlNet.Options;

namespace PumlNet.Generators;

public class PropertyGenerator : BaseGenerator
{
    /// <inheritdoc />
    public PropertyGenerator(StringBuilder sb, int indentationLevel, PumlOptions options) : base(sb,
        indentationLevel,
        options)
    {
    }

    internal void GenerateProps(IEnumerable<PropertyInfo> props)
    {
        foreach (var prop in props) { GenerateProp(prop); }
    }

    internal void GenerateProp(PropertyInfo prop)
    {
        Sb.AppendIndent(IndentationLevel, Options);
        Sb.Append(prop.GetAccessor());

        if (prop.GetMethod?.IsStatic ?? false) Sb.Append("{static} ");

        if (prop.GetMethod?.IsAbstract ?? false) Sb.Append("{abstract} ");

        Sb.Append($"{prop.Name} : {prop.PropertyType.GetPumlTypeName()}");

        Sb.AppendLine();
    }

    private static string GetPropAccessor(PropertyInfo prop)
    {
        if (prop.GetMethod?.IsPublic ?? false) return "+";
        if (prop.GetMethod?.IsPrivate ?? false) return "-";
        if (prop.GetMethod?.IsAssembly ?? false) return "~";
        if (prop.GetMethod?.IsFamily ?? false) return "#";

        return "";
    }
}