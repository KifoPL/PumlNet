namespace PumlNet.Associations;

[AttributeUsage(AttributeTargets.Property)]
public class AssociationLeftCountAttribute : Attribute
{
    /// <inheritdoc />
    public AssociationLeftCountAttribute(string count) { Count = count; }

    public string Count { get; }
}