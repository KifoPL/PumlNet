using System.Reflection;
using System.Text;
using PumlNet.Helpers;
using PumlNet.Options;
using PumlNet.Options.Styles;

namespace PumlNet.Generators;

internal sealed class ClassGenerator : BaseGenerator
{
    private MemberGenerator MemberGenerator { get; }

    internal void GenerateClasses(IEnumerable<Type> classes)
    {
        foreach (Type type in classes)
        {
            if (!type.IsPumlClass(Options)) continue;

            if (type.IsStatic() && !Options.IncludeOptions.IncludeStatic) continue;

            GenerateClass(type);

            Sb.AppendLine();
        }

        Sb.AppendLine();
    }

    internal void GenerateClass(Type type)
    {
        if (type.IsStatic() && !Options.IncludeOptions.IncludeStatic) return;

        Sb.AppendIndent(IndentationLevel, Options);

        Sb.Append(type.GetAccessor(true));

        Sb.Append(ClassType(type));

        Sb.Append($" \"{type.GetPumlTypeName()}\" as "
                + $"{type.GetTypeIdentifier(Options)}");

        if (type.IsValueType && !type.IsEnum) Sb.Append(" << (S,orange) struct >>");

        if (type.IsStatic() && Options.StyleOptions.MarkAsStatic)
            Sb.Append($" << (S,{Options.StyleOptions.StaticColor.ToHex()}) >>");

        Sb.Append(" {");

        Sb.AppendLine();


        MemberGenerator.GenerateMembers(type, false);

        Sb.AppendIndent(IndentationLevel, Options);
        Sb.Append('}');
        Sb.AppendLine();
    }

    private string ClassType(Type type)
    {
        if (type.IsAbstract)
            return "abstract class";

        if (type.IsEnum)
            return "enum";

        return "class";
    }

    internal void GenerateClass<TClass>()
        where TClass : class
    {
        Type type = typeof(TClass);

        GenerateClass(type);
    }

    /// <inheritdoc />
    public ClassGenerator(StringBuilder sb, int indentationLevel, PumlOptions options) : base(sb,
        indentationLevel,
        options)
    {
        MemberGenerator = new(Sb,
                              IndentationLevel + 1,
                              Options);
    }
}