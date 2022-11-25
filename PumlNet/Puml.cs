using System.Reflection;
using System.Text;
using PumlNet.Generators;
using PumlNet.Options;

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
    
    private static void AppendFooter(this StringBuilder sb, PumlOptions options)
    {
        sb.AppendLine("@enduml");
    }

    private static void AppendStyles(this StringBuilder sb, PumlOptions options)
    {
        if (options.StyleOptions.HideEmptyMembers)
        {
            sb.AppendLine("hide empty members");
        }
        
        if (options.StyleOptions.HideEmptyMethods)
        {
            sb.AppendLine("hide empty methods");
        }
        
        if (options.StyleOptions.HideEmptyProperties)
        {
            sb.AppendLine("hide empty properties");
        }

        sb.AppendLine();
    }
    
    public static string GeneratePumlDiagram(Assembly assembly, PumlOptions? options = null)
    {
        var sb = new StringBuilder();

        options ??= new();

        sb.AppendHeader(options);

        var classGenerator = new ClassGenerator(sb,
                                                0,
                                                options);
        
        classGenerator.GenerateClasses(assembly.GetTypes());

        sb.AppendFooter(options);
        
        return sb.ToString();
    }

    public static string GeneratePumlDiagram<TClass>(PumlOptions? options = null)
    where TClass : class
    {
        var sb = new StringBuilder();

        options ??= new();
        
        sb.AppendHeader(options);

        var classGenerator = new ClassGenerator(sb,
                                                        0,
                                                        options);

        classGenerator.GenerateClass<TClass>();

        sb.AppendFooter(options);
        
        return sb.ToString();
    }
}