using System.Reflection;
using System.Text;
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
            var interfaces = typeIndex.GetInterfaces().Where(t => t != typeIndex && t.IsPumlInterface(Options)).ToList();

            foreach (Type? interfaceType in interfaces)
                interfaces = interfaces.Except(interfaceType.GetInterfaces()).ToList();

            dictionary.Add(typeIndex, interfaces);
        }

        return dictionary;
    }

    public void GenerateInheritance(Type baseType, Type derivedtype)
    {
        if ((baseType.IsClass && derivedtype.IsClass) 
         || (baseType.IsInterface && derivedtype.IsInterface))
            Sb.AppendLine($"{baseType.GetTypeIdentifier(Options)} <|-- {derivedtype.GetTypeIdentifier(Options)}");

        if (baseType.IsInterface && derivedtype.IsClass)
            Sb.AppendLine($"{baseType.GetTypeIdentifier(Options)} <|.. {derivedtype.GetTypeIdentifier(Options)}");
    }
}