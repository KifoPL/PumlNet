# Parse your .NET codebase to PlantUML

> Generate PlantUML *(markup language)* from .NET, which can be then drawn using tools of your choice.

## About

This project is a `NuGet` package that can be used to generate `PlantUML` markup from `.NET` code. It is compatible with **.NET 6.0+**.

### Features

Currently the project is in early stages of development. You can only generate class diagrams for now, although it is subject to change. It supports the following PlantUML features:

- Declaring elements
  - class
  - abstract class
  - interface
  - struct
  - static class
  - enum
  - generic types
  - namespaces
- Declaring relations *(specify relation using attributes)*
  - inheritance
  - composition
  - aggregation
- Labels on relations
  - one-to-one
    - based on nullability, `1` or `0..1` 
  - one-to-many
    - fine-tune using attributes *(e.g. `1..*`, `2..3`, etc.)*
  - many-to-many **(not yet implemented)**
- Element members
  - properties
  - methods
  - values (for enum)
- Declaring visibility
  > For props, getter visibility is used
  - public
  - private
  - protected
  - internal
- Styling
  - hide empty members
  - top to bottom, or left to right direction
  - member name + type layout (`TypeScript`-like, `.NET`-like, or even custom)
  - include overriden members in derived classes
  - indent style (tabs, spaces, none)
- Diagram elements
  - title
  - legend
  - header
  - footer
  - caption

### Future plans

- [ ] Add support for many-to-many relations (with association class)
- [ ] Add support for notes on type, member, or relation
- [ ] *If you have any ideas, feel free to open an issue*

# How does it work

This library scans the assembly in which you are generating the `PUML`. Then, using reflection, it is reading all metadata about your code and generating `PUML` markup. It is also converting type names to developer-readable, such as `System.Int32` to `string`, ``Dictionary`2`` to `Dictionary&lt;string, int&gt;`, and so on.

You can specify many options that control the look and style of your diagram, e.g.:

```csharp
var pumlOptions = new PumlOptions
{
    DiagramElements =
    {
        Title = "My Title\nThis is 2nd line of a title",
        Header = "My Header",
        Footer = new PumlString("~ at %date(\"yyyy-MM-dd\")", EscapeType.None),
    },
    StyleOptions =
    {
        IndentType = IndentType.TwoSpaces,
        Direction = Direction.LeftToRight,
        MemberDeclarationOptions =
        {
            Style = MemberDeclarationStyle.DotNet,
        },
    },
    IncludeOptions =
    {
        IncludeNamespace = true,
        IncludeInternal = true,
        IncludeOverridenMethods = false,
    }
};
```

Then, use these options to generate the diagram. Currently there are **2** main workflows supported by this library.


## 1. Generate PlantUML markup for a single class *(or struct, record, interface, enum, etc.)*

This approach will generate PlantUML markup for a single class. It will include all the classes that are referenced by the class you are generating markup for.

```csharp
var pumlOptions = new PumlOptions
{
    // ...
};

string diagram = Puml.GeneratePumlDiagram<Test>(pumlOptions);
```

## 2. Generate PlantUML markup for all classes in an assembly

This approach will generate PlantUML markup for all classes in an assembly. It will include all the classes that are referenced by the classes you are generating markup for.

```csharp
var pumlOptions = new PumlOptions
{
    // ...
};

string diagram = Puml.GeneratePumlDiagram(typeof(Test).Assembly, pumlOptions);
```

The above code will generate the following PUML:

```plantuml
@startuml
' Generated by PumlNet - https://github.com/KifoPL/PumlNet
left to right direction
hide empty members

title
My Title
This is 2nd line of a title
end title

header
My Header
end header

footer
~ at %date("yyyy-MM-dd")
end footer


interface "ITest" as PumlNet.Tests.Console.ITest  {
  +string TestProp
  +void TestMethod()
}

interface "ISpecificTest" as PumlNet.Tests.Console.ISpecificTest  {
  +string Info
}


class "TestGeneric<TList, TItem>" as PumlNet.Tests.Console.TestGeneric0TList1TItem {
}

class "Test" as PumlNet.Tests.Console.Test {
  +{static} string? PublicProp
  -string[] PrivateProp
  #List<string> ProtectedProp
  ~string InternalProp
  +string Info
  +string TestProp
  +void DoStuff()
  ~string? AnotherMethod(string param1, int[] param2)
  #string[]? LastTest(IEnumerable<string> test)
}

abstract class "AbstractTest" as PumlNet.Tests.Console.AbstractTest {
  +string TestProp
  +void TestMethod()
}

abstract class "StaticTest" as PumlNet.Tests.Console.StaticTest << (S,#DAA520) >> {
  +{static} ITest Test
  +{static} TestGeneric<string[], string>? TestGeneric
}

class "Test1" as PumlNet.Tests.Console.OtherNamespace.Test1 {
  +List<Test> Tests
  +AbstractTest[] AbstractTests
  #List<string> ProtectedProp
  ~string InternalProp
  +string Info
  +string TestProp
}


PumlNet.Tests.Console.AbstractTest <|-- PumlNet.Tests.Console.Test
PumlNet.Tests.Console.Test <|-- PumlNet.Tests.Console.OtherNamespace.Test1
PumlNet.Tests.Console.ISpecificTest <|.. PumlNet.Tests.Console.Test
PumlNet.Tests.Console.ITest <|.. PumlNet.Tests.Console.AbstractTest
PumlNet.Tests.Console.ITest <|-- PumlNet.Tests.Console.ISpecificTest
PumlNet.Tests.Console.StaticTest "1" -- "1" PumlNet.Tests.Console.ITest
PumlNet.Tests.Console.StaticTest "1" -- "0..1" PumlNet.Tests.Console.TestGeneric0TList1TItem
PumlNet.Tests.Console.OtherNamespace.Test1 "1" *-- "1..3" PumlNet.Tests.Console.Test
PumlNet.Tests.Console.OtherNamespace.Test1 "1" o-- "0..*" PumlNet.Tests.Console.AbstractTest

@enduml
```

The above `PUML` can be drawn using [PlantUML](https://plantuml.com/) or [PlantUML Server](https://plantuml.com/server), from where you can easily paste them into `README` files, like below.

![PlantUML diagram drawn from the above code](https://www.plantuml.com/plantuml/svg/jLHHRzem47xthpYbcWOTXBRJNYO4HUkqePKE2TwcFPZuqbXAR6GVbL3N_lRPZji6f15u64An_TtTTv_lBhcsn0rjgfAzXs-eq7120Qijp1rsXmHzA8ZMzlDWS2-fsApIJ5U37pBNyzj1z64bvWIamSZxWa18Wnb9hLWX1G9MQzf2XTKAZMMC99N8PbjOXisoa1RS_qe9AAL2q3bmg9rGYRXZ1NA1neTzhtVU5a6MQqrkVGPEy4vmmawoTR_-RDON8ka6t-Z3c5HkpNc6a4oNQ2a1Rbykcth0fb-qihh4DDW17Xd01qj6gdlmoDpejOSUj1G1c24LMdIwx6cNVh76JEOoQqlJzDjDDrMvzgmiAxcra7YFq2MP3PUtqb8FbbF2QdI0lX5p4M8kGuZByxJ3sq9Hwtgqn4bcJr0B7Bk0LIcpM99-ZVxw3NCZ7pXXnCzysc5j78432JD24QtFhvSbD8gN4MxMu507RlI2DddUwRvHZM6YD1LeOcTYw9eRNbtsG2fo4iFfoaUTlSWUmostv3Dqfb_LfaB3LoM-2YTd24tcAmVnZ22MRnBFBMLikfqsLVi95yIfTQxUd60uXCwYTtOpcNowkkZ2QBJNpNhI_MC7RexRIurw4CinRfhtPkdoWAoVlZ5tl4AxvXccmRsM4wR4dqSGGZpMg9blMlDe_zTa-HVkY0uE__Jx1rwOVr0qGuyj3sl_S7ZED3rTphGPstRb5Wv_ftQXh7LQ4pS-Ziil1n8UptEHfcvtqjUHdJ2xFirvq9kc7m_bEPLKH_7doJ6zOTUeX7DZVm40)