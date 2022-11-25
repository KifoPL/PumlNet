using System.Reflection;
using System.Text;
using PumlNet.Generators;
using PumlNet.Helpers;
using PumlNet.Options;
using PumlNet.Options.Styles;

namespace PumlNet;

public static class Puml
{
    private static PumlGenerator GetPumlGenerator(StringBuilder sb, PumlOptions options) => new(sb, options);

    private static void AppendHeader(this StringBuilder sb, PumlOptions options)
    {
        sb.AppendLine("@startuml");

        sb.AppendStyles(options);

        sb.AppendDiagramElements(options.DiagramElements);

        sb.AppendLine();
    }

    private static void AppendFooter(this StringBuilder sb, PumlOptions options) => sb.AppendLine("@enduml");

    private static void AppendStyles(this StringBuilder sb, PumlOptions options)
    {
        if (options.StyleOptions.Direction is not null)
            sb.AppendLine((options.StyleOptions.Direction ?? Direction.TopToBottom).AsStyle());

        if (options.StyleOptions.HideEmptyMembers) sb.AppendLine("hide empty members");

        if (options.StyleOptions.HideEmptyMethods) sb.AppendLine("hide empty methods");

        if (options.StyleOptions.HideEmptyProperties) sb.AppendLine("hide empty properties");

        sb.AppendLine();
    }

    public static string GeneratePumlDiagram(Assembly assembly, PumlOptions? options = null)
    {
        var types = assembly.GetTypes();

        options ??= new();

        return GeneratePumlDiagram(types, options);
    }

    public static string GeneratePumlDiagram<TClass>(PumlOptions? options = null)
        where TClass : class
    {
        Type? type = typeof(TClass);
        var types = new List<Type> { type };
        options ??= new();
        
        types.AddRange(type.GetInterfaces().Where(t => t.IsPumlInterface(options)));

        while (type.BaseType != null && type.BaseType.Assembly == type.Assembly)
        {
            type = type.BaseType;
            types.Add(type);
        }

        var assemblyTypes = type.Assembly.GetTypes();

        types.AddRange(assemblyTypes.Where(t => t.GetInterfaces().Contains(typeof(TClass))));
        assemblyTypes.Aggregate(types,
                                (current, assemblyType) =>
                                {
                                    Type? baseType = assemblyType.BaseType;

                                    while (baseType is not null)
                                    {
                                        if (baseType == type)
                                        {
                                            current.Add(assemblyType);
                                            break;
                                        }

                                        baseType = baseType.BaseType;
                                    }

                                    return current;
                                });

        types = types.DistinctBy(t => t.FullName).ToList();

        return GeneratePumlDiagram(types, options);
    }

    private static string GeneratePumlDiagram(IEnumerable<Type> types, PumlOptions options)
    {
        StringBuilder sb = new();

        sb.AppendHeader(options);

        types = types.ToList();
        
        var interfaceGenerator = new InterfaceGenerator(sb,
                                                        0,
                                                        options);
        
        interfaceGenerator.GenerateInterfaces(types);

        var classGenerator = new ClassGenerator(sb,
                                                0,
                                                options);

        classGenerator.GenerateClasses(types);

        var associationGenerator = new AssociationGenerator(sb,
                                                            0,
                                                            options);

        associationGenerator.GenerateHierarchy(types);

        sb.AppendFooter(options);

        return sb.ToString();
    }
}