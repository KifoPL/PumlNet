using System.Reflection;
using System.Text;
using PumlNet.Options;

namespace PumlNet.Helpers;

internal static class TypeExtensions
{
    internal static string GetAccessor(this Type type, bool implicitPublic = false)
        => type switch
        {
            { IsPublic        : true }  => implicitPublic ? string.Empty : "+",
            { IsNestedPrivate : true }  => "-",
            { IsNestedFamily  : true }  => "#",
            { IsNestedAssembly: true }  => "~",
            { IsVisible       : false } => "~",
            _                                        => implicitPublic ? string.Empty : "+"
        };
    internal static string GetAccessor(this PropertyInfo propertyInfo, bool implicitPublic = false)
        => propertyInfo.GetMethod?.GetAccessor(implicitPublic) ?? (implicitPublic ? string.Empty : "+");

    internal static string GetAccessor(this MethodInfo method, bool implicitPublic = false)
        => method switch
        {
            { IsPublic  : true } => implicitPublic ? string.Empty : "+",
            { IsPrivate : true } => "-",
            { IsFamily  : true } => "#",
            { IsAssembly: true } => "~",
            _                    => implicitPublic ? "+" : string.Empty
        };

    internal static string GetTypeIdentifier(this Type type, PumlOptions options)
    {
        if (!type.IsGenericType)
        {
            string? name = type.Name;

            return (type.Namespace ?? "") + (type.Namespace is null ? string.Empty : ".") + PrimitiveTypes.Aggregate(name,
                                                                     (current, primitiveType)
                                                                         => current.Replace(primitiveType.Key, primitiveType.Value));
        }

        Type? genericType = type.GetGenericTypeDefinition();
        var genericArguments = type.GetGenericArguments();
        var genericArgumentNames = genericArguments.Select(x => x.GetTypeIdentifier(options))
                                                   .Select((x, i) => $"{i}{x}");

        string typeName = options.NamespaceOptions.IncludeInDiagram
                              ? genericType.FullName ?? genericType.Name
                              : genericType.Name;
        string genericTypeName = typeName.Split('`')[0];
        return $"{genericTypeName}{string.Join("", genericArgumentNames)}";
    }

    internal static string GetPumlTypeName(this Type type)
    {
        if (!type.IsGenericType)
        {
            string? name = type.Name;

            return PrimitiveTypes.Aggregate(name,
                                            (current, primitiveType)
                                                => current.Replace(primitiveType.Key, primitiveType.Value));
        }

        Type? genericType = type.GetGenericTypeDefinition();
        var genericArguments = type.GetGenericArguments();
        var genericArgumentNames = genericArguments.Select(GetPumlTypeName);

        string? typeName = genericType.Name;
        string? genericTypeName = typeName.Split('`')[0];
        return $"{genericTypeName}<{string.Join(", ", genericArgumentNames)}>";
    }

    internal static string NullOperator(this ParameterInfo param) => param.IsNullable() ? "?" : "";

    internal static string NullOperator(this PropertyInfo prop) => prop.IsNullable() ? "?" : "";

    internal static bool IsNullable(this ParameterInfo param)
    {
        NullabilityInfoContext nullabilityInfoContext = new();

        NullabilityInfo nullabilityInfo = nullabilityInfoContext.Create(param);

        return nullabilityInfo.ReadState == NullabilityState.Nullable;
    }

    internal static bool IsNullable(this PropertyInfo prop)
    {
        NullabilityInfoContext nullabilityInfoContext = new();

        NullabilityInfo nullabilityInfo = nullabilityInfoContext.Create(prop);

        return nullabilityInfo.ReadState == NullabilityState.Nullable;
    }

    private static readonly Dictionary<string, string> PrimitiveTypes = new()
    {
        { "String", "string" },
        { "Int32", "int" },
        { "Int64", "long" },
        { "Boolean", "bool" },
        { "Decimal", "decimal" },
        { "Double", "double" },
        { "Single", "float" },
        { "Byte", "byte" },
        { "Char", "char" },
        { "Object", "object" },
        { "SByte", "sbyte" },
        { "Int16", "short" },
        { "UInt16", "ushort" },
        { "UInt32", "uint" },
        { "UInt64", "ulong" },
        { "Void", "void" },
        { "Nullable", "null" }
    };
}