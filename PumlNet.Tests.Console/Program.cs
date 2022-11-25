// See https://aka.ms/new-console-template for more information

using System.Reflection;
using PumlNet;
using PumlNet.Generators;
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
        Footer = $"by Michał Sitek {DateTime.Now:yyyy-MM-dd}",
    },
    StyleOptions =
    {
        IndentType = IndentType.TwoSpaces,
        Direction = Direction.LeftToRight,
        MemberDeclarationOptions =
        {
            Style = MemberDeclarationStyle.TypeScript,
        },
    },
    IncludeOptions =
    {
        IncludeInDiagram = true,
        IncludeInternal = false,
        IncludeOverridenMethods = false,
    }
};

var diagram = Puml.GeneratePumlDiagram(typeof(Puml).Assembly, pumlOptions);
//var diagram = Puml.GeneratePumlDiagram<Test>(pumlOptions);
//var diagram = Puml.GeneratePumlDiagram<AssociationGenerator>(pumlOptions);

//var diagram = Puml.GeneratePumlDiagram(Assembly.GetExecutingAssembly(), pumlOptions);

Console.WriteLine(diagram);

File.WriteAllText("../../../diagram.puml", diagram);