using System.Collections;
using System.Reflection;
using System.Text;
using PumlNet.Associations;
using PumlNet.Helpers;
using PumlNet.Options;

namespace PumlNet.Generators;

internal class AssociationGenerator : BaseGenerator
{
    /// <inheritdoc />
    public AssociationGenerator(StringBuilder sb, int indentationLevel, PumlOptions options) : base(sb,
        indentationLevel,
        options)
    {
    }

    public void GenerateHierarchy(IEnumerable<Type> types)
    {
        types = types.ToList();

        GenerateClassInheritance(types);

        GenerateInterfaceInheritance(types);

        GenerateOneToOneAssociations(types);

        GenerateOneToManyAssociations(types);

        Sb.AppendLine();
    }

    private void GenerateClassInheritance(IEnumerable<Type> types)
    {
        var hierarchy = types
                       .Where(t => t.BaseType != null && t.IsPumlClass(Options) && t.BaseType.IsPumlClass(Options))
                       .GroupBy(t => t.BaseType!)
                       .ToDictionary(g => g.Key, g => g.ToList());

        foreach ((Type? baseType, var derivedTypes) in hierarchy)
        {
            foreach (Type? derivedType in derivedTypes) GenerateInheritance(baseType, derivedType);
        }
    }

    private void GenerateInterfaceInheritance(IEnumerable<Type> types)
    {
        var hierarchy = GetInterfaces(types);

        foreach ((Type? type, var interfaces) in hierarchy)
        {
            foreach (Type? baseInterface in interfaces) GenerateInheritance(baseInterface, type);
        }
    }

    private Dictionary<Type, List<Type>> GetInterfaces(IEnumerable<Type> types)
    {
        var dictionary = new Dictionary<Type, List<Type>>();

        foreach (Type? typeIndex in types)
        {
            var interfaces = typeIndex.GetInterfaces()
                                      .Where(t => t != typeIndex && t.IsPumlInterface(Options))
                                      .ToList();

            foreach (Type? interfaceType in interfaces)
                interfaces = interfaces.Except(interfaceType.GetInterfaces()).ToList();

            if (typeIndex.BaseType is not null)
                interfaces = interfaces.Except(typeIndex.BaseType.GetInterfaces()).ToList();

            dictionary.Add(typeIndex, interfaces);
        }

        return dictionary;
    }

    private void GenerateInheritance(Type baseType, Type derivedtype)
    {
        if ((baseType.IsClass && derivedtype.IsClass)
         || (baseType.IsInterface && derivedtype.IsInterface))
            Sb.AppendLine($"{baseType.GetTypeIdentifier(Options)} <|-- {derivedtype.GetTypeIdentifier(Options)}");

        if (baseType.IsInterface && derivedtype.IsClass)
            Sb.AppendLine($"{baseType.GetTypeIdentifier(Options)} <|.. {derivedtype.GetTypeIdentifier(Options)}");
    }

    private void GenerateOneToOneAssociations(IEnumerable<Type> types)
    {
        var associations = types
                          .SelectMany(t => t.GetProperties())
                          .Where(p => p.PropertyType.IsPumlType(Options)
                                   && p.CustomAttributes.All(a => a.AttributeType != typeof(AssociationIgnoreAttribute))
                                   && p.PropertyType.CustomAttributes.All(
                                          a => a.AttributeType != typeof(AssociationIgnoreAttribute))
                                   && !p.PropertyType.GetInterfaces().Contains(typeof(IEnumerable<>))
                                   && !p.PropertyType.IsArray)
                          .GroupBy(p => p.PropertyType)
                          .ToDictionary(g => g.Key, g => g.ToList());

        foreach ((Type? type, var properties) in associations)
        {
            foreach (PropertyInfo? property in properties) GenerateOneToOneAssociation(property, type);
        }
    }

    private void GenerateOneToOneAssociation(PropertyInfo property, Type type)
        => Sb.AppendLine(
            $"{property.DeclaringType!.GetTypeIdentifier(Options)} \"1\" -- \"{GetCount(property)}\" {type.GetTypeIdentifier(Options)}");

    private string GetCount(PropertyInfo property, bool isRight = true)
    {
        if (isRight)
        {
            CustomAttributeData? rightCount
                = property.CustomAttributes.FirstOrDefault(
                    a => a.AttributeType == typeof(AssociationRightCountAttribute));

            if (rightCount is not null) return rightCount.ConstructorArguments[0].Value!.ToString()!;

            if (!property.PropertyType.GetInterfaces().Contains(typeof(IEnumerable))
             && !property.PropertyType.IsArray)
                return property.IsNullable() ? "0..1" : "1";

            return "0..*";
        }

        CustomAttributeData? leftCount
            = property.CustomAttributes.FirstOrDefault(a => a.AttributeType == typeof(AssociationLeftCountAttribute));

        return leftCount is not null ? leftCount.ConstructorArguments[0].Value!.ToString()! : "1";
    }

    private void GenerateOneToManyAssociations(IEnumerable<Type> types)
    {
        var associations = types
                          .SelectMany(t => t.GetProperties())
                          .Where(IsOneToManyAssociation)
                          .GroupBy(p => p.PropertyType.IsArray
                                            ? p.PropertyType.GetElementType()
                                            : p.PropertyType.GetGenericArguments().First())
                          .ToDictionary(g => g.Key, g => g.ToList());

        foreach ((Type? type, var properties) in associations)
        {
            foreach (PropertyInfo property in properties) GenerateOneToManyAssociation(property, type);
        }
    }

    private bool IsOneToManyAssociation(PropertyInfo prop)
    {
        // Has to be a collection
        if (!prop.PropertyType.GetInterfaces().Contains(typeof(IEnumerable))) return false;

        // String is a collection of chars
        if (prop.PropertyType == typeof(string)) return false;

        // Cannot be ignored
        if (prop.CustomAttributes.Any(a => a.AttributeType == typeof(AssociationIgnoreAttribute)))
            return false;
        if (prop.PropertyType.CustomAttributes.Any(a => a.AttributeType == typeof(AssociationIgnoreAttribute)))
            return false;

        // Has to be Puml type
        Type type = prop.PropertyType.IsArray
                        ? prop.PropertyType.GetElementType()!
                        : prop.PropertyType.GetGenericArguments().First();

        return type.IsPumlType(Options);
    }

    private void GenerateOneToManyAssociation(PropertyInfo property, Type type)
    {
        Sb.Append(property.DeclaringType!.GetTypeIdentifier(Options));
        Sb.Append(" \"");
        Sb.Append(GetCount(property, false));
        Sb.Append("\" ");
        CustomAttributeData? aggregationTypeAttr
            = property.CustomAttributes.FirstOrDefault(a => a.AttributeType == typeof(AssociationTypeAttribute));
        if (aggregationTypeAttr is not null)
        {
            var aggregationType = (AssociationType)aggregationTypeAttr.ConstructorArguments[0].Value!;
            Sb.Append(aggregationType.AsSymbol());
        }

        Sb.Append("-- \"");
        Sb.Append(GetCount(property));
        Sb.Append("\" ");
        Sb.Append(type.GetTypeIdentifier(Options));
        Sb.AppendLine();
    }
}