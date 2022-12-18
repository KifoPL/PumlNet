using System.Reflection;
using System.Text;
using PumlNet.Helpers;
using PumlNet.Options;
using PumlNet.Options.Styles.MemberDeclaration;

namespace PumlNet.Generators;

internal class MemberGenerator : BaseGenerator
{
    /// <inheritdoc />
    public MemberGenerator(StringBuilder sb, int indentationLevel, PumlOptions options) : base(sb,
        indentationLevel,
        options)
    {
    }

    internal void GenerateMembers(Type type, bool isInterface)
    {
        if (type.IsEnum)
        {
            GenerateEnumValues(type);
            return;
        }

        var props = type.GetProperties(BindingFlags.Instance
                                     | BindingFlags.NonPublic
                                     | BindingFlags.Public
                                     | BindingFlags.Static)
                        .Where(t => t.IsPumlType(Options));

        GenerateProps(props, isInterface);

        var methods = type.GetMethods(BindingFlags.Instance
                                    | BindingFlags.NonPublic
                                    | BindingFlags.Public
                                    | BindingFlags.Static)
                          .Where(x => x.DeclaringType is not null
                                   && x.DeclaringType == type
                                   && x.IsPumlType(Options));

        GenerateMethods(methods, isInterface);
    }

    internal void GenerateProps(IEnumerable<PropertyInfo> props, bool isInterface)
    {
        foreach (PropertyInfo? prop in props) GenerateProp(prop, isInterface);
    }

    internal void GenerateProp(PropertyInfo prop, bool isInterface)
    {
        Sb.AppendIndent(IndentationLevel, Options);
        Sb.Append(prop.GetAccessor());

        if (prop.GetMethod?.IsStatic ?? false) Sb.Append("{static} ");

        if (!isInterface && (prop.GetMethod?.IsAbstract ?? false)) Sb.Append("{abstract} ");

        Sb.Append(Options.StyleOptions.MemberDeclarationOptions.Format
                         .Replace($"{{{MemberDeclarationKeys.Name}}}", prop.Name)
                         .Replace($"{{{MemberDeclarationKeys.Type}}}",
                                  prop.PropertyType.GetPumlTypeName() + prop.NullOperator()));

        Sb.AppendLine();
    }

    internal void GenerateMethods(IEnumerable<MethodInfo> methods, bool isInterface)
    {
        foreach (MethodInfo method in methods) GenerateMethod(method, isInterface);
    }

    internal void GenerateMethod(MethodInfo method, bool isInterface)
    {
        Sb.AppendIndent(IndentationLevel, Options);
        Sb.Append(method.GetAccessor());

        if (method.IsStatic) Sb.Append("{static} ");

        if (!isInterface && method.IsAbstract) Sb.Append("{abstract} ");

        Sb.Append(Options.StyleOptions.MemberDeclarationOptions.Format
                         .Replace($"{{{MemberDeclarationKeys.Name}}}",
                                  $@"{method.Name}({string
                                     .Join(", ", method.GetParameters()
                                                       .Select(x => Options.StyleOptions.MemberDeclarationOptions.Format
                                                                           .Replace($"{{{MemberDeclarationKeys.Name}}}", x.Name)
                                                                           .Replace($"{{{MemberDeclarationKeys.Type}}}",
                                                                                x.ParameterType.GetPumlTypeName() + x.NullOperator())))})")
                         .Replace($"{{{MemberDeclarationKeys.Type}}}",
                                  method.ReturnType.GetPumlTypeName() + method.ReturnParameter.NullOperator()));

        Sb.AppendLine();
    }

    internal void GenerateEnumValues(Type type)
    {
        foreach (object? value in Enum.GetValues(type))
        {
            Sb.AppendIndent(IndentationLevel, Options);
            Sb.Append(value);
            Sb.AppendLine();
        }
    }
}