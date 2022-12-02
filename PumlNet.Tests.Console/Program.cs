// See https://aka.ms/new-console-template for more information

using PumlNet;
using PumlNet.Options;
using PumlNet.Options.Styles;
using PumlNet.Options.Styles.MemberDeclaration;
using PumlNet.Tests.Console;

var pumlOptions = new PumlOptions
{
    DiagramElements =
    {
        Title = "My Title\nThis is 2nd line of a title",
        Header = "My Header",
        Footer = new PumlString("~ at %date(\"yyyy-MM-dd\")", EscapeType.None)
    },
    StyleOptions =
    {
        IndentType = IndentType.TwoSpaces,
        Direction = Direction.LeftToRight,
        MemberDeclarationOptions =
        {
            Style = MemberDeclarationStyle.DotNet
        }
    },
    IncludeOptions =
    {
        IncludeNamespace = true,
        IncludeInternal = true,
        IncludeOverridenMethods = false
    }
};

string diagram = Puml.GeneratePumlDiagram(typeof(Test).Assembly, pumlOptions);

Console.WriteLine(diagram);

File.WriteAllText("../../../diagram.puml", diagram);