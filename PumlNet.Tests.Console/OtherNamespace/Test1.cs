using PumlNet.Associations;

namespace PumlNet.Tests.Console.OtherNamespace;

public class Test1 : Test
{
    [AssociationType(AssociationType.Composition), AssociationRightCount("1..3")]
    public List<Test> Tests { get; set; }

    [AssociationType(AssociationType.Aggregation)]
    public AbstractTest[] AbstractTests { get; set; }
}