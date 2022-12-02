using System.Text;
using PumlNet.Helpers;
using PumlNet.Options;
using PumlNet.Options.Styles;

namespace PumlNet.Generators;

internal class InterfaceGenerator : BaseGenerator
{
    private MemberGenerator MemberGenerator { get; }

    internal void GenerateInterfaces(IEnumerable<Type> classes)
    {
        foreach (Type? type in classes)
        {
            if (!type.IsPumlInterface(Options)) continue;

            GenerateInterface(type);

            Sb.AppendLine();
        }

        Sb.AppendLine();
    }

    internal void GenerateInterface(Type type)
    {
        Sb.AppendIndent(IndentationLevel, Options);

        Sb.Append(type.GetAccessor(true));

        Sb.Append($"interface \"{type.GetPumlTypeName()}\" as "
                + $"{type.GetTypeIdentifier(Options)} {(type.IsStatic() ? $"<< (S,{Options.StyleOptions.StaticColor.ToHex()}) >>" : "")} {{");
        Sb.AppendLine();


        MemberGenerator.GenerateMembers(type, true);

        Sb.AppendIndent(IndentationLevel, Options);
        Sb.Append('}');
        Sb.AppendLine();
    }

    internal void GenerateInterface<TInterface>()
    {
        Type type = typeof(TInterface);

        GenerateInterface(type);
    }

    /// <inheritdoc />
    public InterfaceGenerator(StringBuilder sb, int indentationLevel, PumlOptions options) : base(sb,
        indentationLevel,
        options)
    {
        MemberGenerator = new(Sb,
                              IndentationLevel + 1,
                              Options);
    }
}