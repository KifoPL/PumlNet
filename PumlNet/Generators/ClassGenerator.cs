using System.Reflection;
using System.Text;
using PumlNet.Helpers;
using PumlNet.Options;

namespace PumlNet.Generators;

internal sealed class ClassGenerator : BaseGenerator
{
    private MemberGenerator MemberGenerator { get; }

    internal void GenerateClasses(IEnumerable<Type> classes)
    {
        foreach (var type in classes)
        {
            if (!type.IsClass) continue;
            if (type.Name.StartsWith("<>c")) continue;

            if (!Options.NamespaceOptions.IncludeCompilerGenerated)
            {
                if (type.IsSpecialName) continue;
                if (type.Namespace?.StartsWith("Microsoft") ?? false) continue;
                if (type.Namespace?.StartsWith("System") ?? false) continue;
            }
            
            GenerateClass(type);

            Sb.AppendLine();
        }
    }

    internal void GenerateClass(Type type)
    {
        Sb.AppendIndent(IndentationLevel, Options);

        Sb.Append(type.GetAccessor(true));
        
        if (type.IsAbstract)
            Sb.Append("abstract ");
        
        Sb.Append($"class \"{type.GetPumlTypeName()}\" as {type.GetTypeIdentifier(Options)} {{");
        Sb.AppendLine();
        

        MemberGenerator.GenerateMembers(type);

        Sb.AppendIndent(IndentationLevel, Options);
        Sb.Append('}');
        Sb.AppendLine();
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
        MemberGenerator = new(Sb, IndentationLevel + 1, Options);
    }
}