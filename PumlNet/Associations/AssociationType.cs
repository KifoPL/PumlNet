namespace PumlNet.Associations;

public enum AssociationType { Association, Aggregation, Composition }

public static class AssociationTypeExtensions
{
    public static string AsSymbol(this AssociationType associationType)
        => associationType switch
        {
            AssociationType.Association => "",
            AssociationType.Aggregation => "o",
            AssociationType.Composition => "*",
            _ => throw new ArgumentOutOfRangeException(nameof(associationType),
                                                       associationType,
                                                       null)
        };
}