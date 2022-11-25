namespace PumlNet.Options.Namespace;

public class IncludeOptions
{
    public bool IncludeNamespace { get; set; } = true;
    public bool IncludeInternal { get; set; } = true;
    public bool IncludeCompilerGenerated { get; set; } = false;
    public bool IncludeOverridenMethods { get; set; } = true;
    public bool IncludeStatic { get; set; } = true;
}