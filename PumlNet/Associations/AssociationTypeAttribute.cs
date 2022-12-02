namespace PumlNet.Associations;

public class AssociationTypeAttribute : Attribute
{
    public AssociationType Type { get; }
    public AssociationTypeAttribute(AssociationType type) { Type = type; }
}