using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using PumlNet.Helpers;
using PumlNet.Options;

namespace PumlNet.Generators;

public class MemberGenerator : BaseGenerator
{
    /// <inheritdoc />
    public MemberGenerator(StringBuilder sb, int indentationLevel, PumlOptions options) : base(sb,
        indentationLevel,
        options)
    {
    }

    internal void GenerateMembers(Type type)
    {
        var props = type.GetProperties(BindingFlags.Instance
                                     | BindingFlags.NonPublic
                                     | BindingFlags.Public
                                     | BindingFlags.Static);

        GenerateProps(props);

        var methods = type.GetMethods(BindingFlags.Instance
                                    | BindingFlags.NonPublic
                                    | BindingFlags.Public
                                    | BindingFlags.Static)
                          .Where(x => x.DeclaringType is not null
                                   && x.DeclaringType == type
                                   && x.MemberType == MemberTypes.Method
                                   && !x.IsSpecialName);

        GenerateMethods(methods);
    }

    internal void GenerateProps(IEnumerable<PropertyInfo> props)
    {
        foreach (PropertyInfo? prop in props) GenerateProp(prop);
    }

    internal void GenerateProp(PropertyInfo prop)
    {
        Sb.AppendIndent(IndentationLevel, Options);
        Sb.Append(prop.GetAccessor());

        if (prop.GetMethod?.IsStatic ?? false) Sb.Append("{static} ");

        if (prop.GetMethod?.IsAbstract ?? false) Sb.Append("{abstract} ");

        Sb.Append($"{prop.Name} : {prop.PropertyType.GetPumlTypeName()}{prop.NullOperator()}");

        Sb.AppendLine();
    }

    internal void GenerateMethods(IEnumerable<MethodInfo> methods)
    {
        foreach (MethodInfo method in methods) GenerateMethod(method);
    }

    internal void GenerateMethod(MethodInfo method)
    {
        Sb.AppendIndent(IndentationLevel, Options);
        Sb.Append(method.GetAccessor());

        if (method.IsStatic) Sb.Append("{static} ");

        if (method.IsAbstract) Sb.Append("{abstract} ");

        Sb.Append(
            $"{method.Name}({string.Join(", ", method.GetParameters().Select(x => $"{x.Name}: {x.ParameterType.GetPumlTypeName()}{x.NullOperator()}"))}) : {method.ReturnType.GetPumlTypeName()}{method.ReturnParameter.NullOperator()}");

        Sb.AppendLine();
    }
}