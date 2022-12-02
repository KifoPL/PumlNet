using System.Reflection;
using System.Runtime.CompilerServices;
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
            _                           => implicitPublic ? string.Empty : "+"
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

            string typeNamespace
                = options.IncludeOptions.IncludeNamespace
                      ? (type.Namespace ?? "") + "."
                      : string.Empty;

            return typeNamespace
                 + PrimitiveTypes.Aggregate(name,
                                            (current, primitiveType)
                                                => current.Replace(primitiveType.Key, primitiveType.Value));
        }

        TypeInfo genericType = type.GetGenericTypeDefinition().GetTypeInfo();
        var genericArgumentNames = genericType.GenericTypeParameters.Select(x => x.GetTypeIdentifier(options))
                                              .Select((x, i) => $"{i}{x.Split(".")[^1]}");

        string typeName = options.IncludeOptions.IncludeNamespace
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
                                                => current == primitiveType.Key || current == primitiveType.Key + "[]"
                                                       ? current.Replace(primitiveType.Key, primitiveType.Value)
                                                       : current);
        }

        if (type.GetGenericTypeDefinition() == typeof(Nullable<>))
        {
            Type nullableType = type.GetGenericArguments()[0];
            return nullableType.GetPumlTypeName();
        }

        Type? genericType = type.GetGenericTypeDefinition();
        var genericArguments = type.GetGenericArguments();
        var genericArgumentNames = genericArguments.Select(GetPumlTypeName);

        string? typeName = genericType.Name;
        string? genericTypeName = typeName.Split('`')[0];
        return $"{genericTypeName}<{string.Join(", ", genericArgumentNames)}>";
    }

    internal static bool IsPumlClass(this Type type, PumlOptions options)
    {
        if (!type.IsClass && !type.IsValueType) return false;
        if (!options.IncludeOptions.IncludeInternal && !type.IsVisible) return false;

        if (!type.IsPumlType(options)) return false;

        return true;
    }

    internal static bool IsPumlInterface(this Type type, PumlOptions options)
        => type.IsInterface && type.IsPumlType(options);

    internal static bool IsPumlType(this Type type, PumlOptions options)
    {
        if (type.Name.StartsWith("<>c")) return false;

        if (!options.IncludeOptions.IncludeCompilerGenerated)
        {
            if (type.CustomAttributes.Any(x => x.AttributeType == typeof(CompilerGeneratedAttribute))) return false;
            if (type.IsSpecialName) return false;
            if (type.Namespace?.StartsWith("Microsoft") ?? false) return false;
            if (type.Namespace?.StartsWith("System") ?? false) return false;
        }

        return true;
    }

    internal static bool IsPumlType(this PropertyInfo propertyInfo, PumlOptions options)
    {
        if (propertyInfo.Name.StartsWith("<>c")) return false;

        if (!options.IncludeOptions.IncludeCompilerGenerated)
        {
            if (propertyInfo.CustomAttributes.Any(x => x.AttributeType == typeof(CompilerGeneratedAttribute)))
                return false;
            if (propertyInfo.IsSpecialName) return false;
        }

        if (!options.IncludeOptions.IncludeInternal && (propertyInfo.GetMethod?.IsAssembly ?? false)) return false;

        return true;
    }

    internal static bool IsPumlType(this MethodInfo methodInfo, PumlOptions options)
    {
        if (methodInfo.MemberType is not MemberTypes.Method) return false;
        if (methodInfo.Name.StartsWith("get_") || methodInfo.Name.StartsWith("set_")) return false;
        if (!options.IncludeOptions.IncludeInternal && methodInfo.IsAssembly) return false;
        if (!options.IncludeOptions.IncludeCompilerGenerated
         && methodInfo.CustomAttributes.Any(a => a.AttributeType == typeof(CompilerGeneratedAttribute))) return false;
        if (!options.IncludeOptions.IncludeOverridenMethods && methodInfo.IsOverride()) return false;

        return true;
    }

    internal static bool IsOverride(this MethodInfo methodInfo)
        => methodInfo.GetBaseDefinition().DeclaringType != methodInfo.DeclaringType;

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

    internal static bool IsStatic(this Type type) => type.IsAbstract && type.IsSealed;

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