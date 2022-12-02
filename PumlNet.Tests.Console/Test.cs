namespace PumlNet.Tests.Console;

public class TestGeneric<TList, TItem> where TList : IList<TItem>
{
}

public class Test : AbstractTest, ISpecificTest
{
    public static string? PublicProp { get; set; }
    private string[] PrivateProp { get; set; }
    protected List<string> ProtectedProp { get; }
    internal string InternalProp { get; }

    public void DoStuff() { }

    internal string? AnotherMethod(string param1, int[] param2) => param1 + param2;

    protected string[]? LastTest(IEnumerable<string> test) => throw new NotImplementedException();

    /// <inheritdoc />
    public string Info { get; set; }
}

public abstract class AbstractTest : ITest
{
    /// <inheritdoc />
    public string TestProp { get; set; }

    /// <inheritdoc />
    public void TestMethod() => throw new NotImplementedException();
}

public static class StaticTest
{
    public const string ConstString = "";
    public static readonly string StaticReadonly = "";
    public static ITest Test { get; set; }
    public static TestGeneric<string[], string>? TestGeneric { get; set; }
}

public interface ITest
{
    public string TestProp { get; set; }

    public void TestMethod();
}

public interface ISpecificTest : ITest
{
    public string Info { get; set; }
}