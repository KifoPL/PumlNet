using System.Reflection;

namespace PumlNet.Tests;

public class UnitTest1
{
    [Fact]
    public void Test1() => Puml.GeneratePumlDiagram(Assembly.GetExecutingAssembly());
}