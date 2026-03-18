# JsonConstGenerator

**JsonConstGenerator** is a C# Source Generator that converts JSON files into strongly-typed, compile-time constant structures.

It allows developers to define hierarchical data in JSON and access it in C# through generated static classes and typed nodes.

The generator produces a tree of static classes that mirrors the structure of the JSON file and exposes each value as a `JsonConstNode`.

This makes it easy to:

* Access JSON values in a **type-safe way**
* Reference **stable string paths**
* Avoid hardcoded strings throughout the codebase
* Keep configuration or constant definitions in JSON
* Easy way to maintain nested constant names

---

# Installation

Install the generator package:

```
dotnet add package JsonConstGenerator
```

---

# Basic Usage

1. Create a JSON file.

Example:

```json
{ 
    "Numbers": { 
        "IntValue": 42, 
        "LargeIntValue": 5000000000, 
        "DecimalValue": 3.1415 
    }, 
    "Strings": { 
        "Greeting": "Hello world" 
    }, 
    "Flags": { 
        "IsEnabled": true 
    }, 
    "EmptyObject": {}, 
    "NullValue": null, 
    "EmptyArray": [], 
    "Permissions": ["Read", "Write", "Delete"] 
}
```

2. Add the JSON file as an `AdditionalFiles` item in your project.

```xml
<ItemGroup>
  <AdditionalFiles Include="myJsonFile.json" />
</ItemGroup>
```

3. Create a partial class and annotate it with `JsonConstGenerator`.

```csharp
[JsonConstGenerator("myJsonFile.json")]
public static partial class MyConstants
{
}
```

---

# Generated Code

The generator produces a structure matching the JSON hierarchy.

```csharp
public static partial class MyConstants
{
    public static Parent1ConstNode = new();
    public class Parent1 : IParentConstNode
    {
        public static Parent1Group1ConstNode Group1 = new(); 
        public class Parent1Group1ConstNode : IParentConstNode
        {
            //A number value without decimal point is used as integer, unless the value is to large for int32.
            public static JsonConstNode<int> Child1 =
                new JsonConstNode<int>("Parent1.Group1.Child1", 1);

            //true / false is used as a boolean
            public static JsonConstNode<bool> Child2 =
                new JsonConstNode<bool>("Parent1.Group1.Child2", true);

            //a number value with a decimal point is used as a double
            public static JsonConstNode<double> Child3 = 
                new JsonConstNode<double>("Parent1.Group1.Child3", 10.0);

            //a text value is used as a string
            public static JsonConstNode<string> Child4 = 
                new JsonConstNode<double>("Parent1.Group1.Child4", "FixedName");
        }
        
        //A array of strings is implemented as nodes without a value, preserving their path
        public static class Item1
        {
            public static JsonConstNode Array1 = 
                new JsonConstNode("Paren1.Item1.Array1");
                
            public static JsonConstNode Array2 = 
                new JsonConstNode("Paren1.Item1.Array2");
                
            public static JsonConstNode Array3 = 
                new JsonConstNode("Paren1.Item1.Array3");
        }
    }

    // A node with a empty object is just read as a node without value
    public static JsonConstNode Item2 =
        new JsonConstNode("Item2");

    // A node with a null value is also read as a valuelessNode
    public static JsonConstNode Item3 = 
        new JsonConstNode("Item3");
}
```

---

# Using Generated Nodes

Each node contains:

* The **path** inside the JSON hierarchy
* The **typed value** (if present)

Example usage:

```csharp
var path = MyConstants.Parent1.Group1.Child1.Path;
var value = MyConstants.Parent1.Group1.Child1.Value;
```

---

# Implicit Conversions

`JsonConstNode` supports implicit conversion to the node's path.

```csharp
string path = MyConstants.Parent1.Group1.Child1;
```

`JsonConstNode<T>` may also support implicit conversion to its value.

```csharp
int value = MyConstants.Parent1.Group1.Child1;
```

---

# Supported JSON Types

Currently supported JSON value types:

* `string`
* `number`
* `boolean`
* `object`
* `array`
* `Null`

Objects are represented as nested static classes.

Empty objects produce nodes without values.

Example:

```json
{
  "FeatureFlags": {
    "EnableFeatureX": true
  }
}
```

Generated:

```csharp
public static class FeatureFlags
{
    public static JsonConstNode<bool> EnableFeatureX =
        new JsonConstNode<bool>("FeatureFlags.EnableFeatureX", true);
}
```

---

# Design Goals

JsonConstGenerator aims to:

* Remove hardcoded string identifiers
* Provide strongly typed access to JSON-defined constants
* Preserve the hierarchical structure of the original JSON
* Generate zero-runtime-cost structures
* Integrate seamlessly with modern C# projects

---

# Requirements

* .NET SDK with C# Source Generator support
* JSON files included as `AdditionalFiles`

---

# Example Use Cases

Common scenarios include:

* Permission identifiers
* Feature flags
* Event names
* Configuration keys
* API endpoint identifiers
* Localization keys

---

# Limitations

Current limitations include:

* JSON must contain valid object hierarchies
* Generated code depends on compile-time JSON files

---

# License

MIT License
