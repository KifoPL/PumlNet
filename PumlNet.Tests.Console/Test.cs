namespace PumlNet.Tests.Console;

public class TestGeneric<TList, TItem> where TList : IList<TItem>
{
}

public class Test : AbstractTest
{
    public static string? PublicProp { get; set; }
    private string[] PrivateProp { get; set; }
    protected List<string> ProtectedProp { get; }
    internal string InternalProp { get; }

    public void DoStuff() {}
    
    internal string? AnotherMethod(string param1, int param2)
    {
        return param1 + param2;
    }
}

public abstract class AbstractTest
{
    
}