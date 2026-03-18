using JsonConstGenerator;
using JsonConstGenerator.Helpers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using MiniJSON;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading;
using System.Xml.Linq;

[Generator]
internal class JsonConstIncrementalGenerator : IIncrementalGenerator
{

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        //if (!Debugger.IsAttached) Debugger.Launch();
        var provider = context.SyntaxProvider
            .ForAttributeWithMetadataName(
                "JsonConstGenerator.JsonConstGeneratorAttribute",
                IsTargetClass,
                TransformAttributesToGeneratorInput)
            .SelectMany((inputs, _) => inputs);

        context.RegisterSourceOutput(provider, ProcessGeneratorInput);
    }

    // Determines if the node is a class we care about
    private static bool IsTargetClass(SyntaxNode node, CancellationToken ct)
    {
        return node is ClassDeclarationSyntax;
    }


    // Transform each attribute on a class into a GeneratorInput
    private static IEnumerable<GeneratorInput> TransformAttributesToGeneratorInput(GeneratorAttributeSyntaxContext ctx, CancellationToken ct)
    {
        // Extract class info
        var classSyntax = (ClassDeclarationSyntax)ctx.TargetNode;
        var classSymbol = (INamedTypeSymbol)ctx.TargetSymbol;

        // Optional debug log
        File.AppendAllText(@"C:\temp\gen_debug.txt",
            $"Found class {classSymbol.Name} with {ctx.Attributes.Length} attribute(s)\n");

        // Loop over each attribute separately
        foreach (var attribute in ctx.Attributes)
        {
            yield return new GeneratorInput(classSyntax, classSymbol, attribute);
        }
    }

    // Process each valid GeneratorInput
    private static void ProcessGeneratorInput(SourceProductionContext spc, GeneratorInput input)
    {
        // Attach debugger if needed (optional)
        //if (!Debugger.IsAttached) Debugger.Launch();

        // Validate the class
        if (!Validation.TryValidateClass(spc, input.ClassSyntax, input.ClassSymbol, input.Attribute))
        {
            // Stop processing this attribute only
            return;
        }

        // Optional debug log
        File.AppendAllText(@"C:\temp\gen_debug.txt",
            $"Validated class {input.ClassSymbol.Name}, ready for generation\n");

        var modifiers = GeneratorHelper.GetClassModifiers(input.ClassSymbol);
        var namespaceName = GeneratorHelper.GetNamespace(input.ClassSymbol);
        var className = input.ClassSymbol.Name;

        var projectDir = Path.GetDirectoryName(input.ClassSymbol.ContainingAssembly.Locations.FirstOrDefault()?.SourceTree?.FilePath)
                 ?? Directory.GetCurrentDirectory();

        var fileNames = input.Attribute.ConstructorArguments.Length > 0
            ? input.Attribute.ConstructorArguments[0].Values.Select(v => v.Value?.ToString() ?? "").ToArray()
            : Array.Empty<string>();


        var filePaths = GetJsonFilePaths(input, fileNames, spc, projectDir);

        var relativePaths = filePaths.Select(fp => fp.Substring(projectDir.Length+1)).ToArray();

        var dict = JsonMerger.MergeJsonFiles(filePaths);

        var sb = new StringBuilder();
        var sb2 = new StringBuilder();
        sb.AppendLine($"namespace {namespaceName ?? "global"}");
        sb.AppendLine($"{{");

        // ⚡ Add comment about generated files
        sb.AppendLine("    /// <summary>");
        sb.AppendLine($"    /// Generated from {string.Join(", ", relativePaths)}");
        sb.AppendLine("    /// </summary>");
        sb.AppendLine($"    {modifiers} partial class {className}");
        sb.AppendLine($"    {{");


        sb2.AppendLine($"namespace JsonConstGenerator.{className}");
        sb2.AppendLine($"{{");
        GenerateFromDict(dict, sb, sb2, 2, string.Empty);

        sb.AppendLine($"    }}");
        sb.AppendLine($"}}");
        sb2.AppendLine($"}}");


        spc.AddSource($"{className}_JsonConst.g.cs", SourceText.From(sb.ToString(), Encoding.UTF8));
    }

    private static string[] GetJsonFilePaths(GeneratorInput input, string[] filePaths, SourceProductionContext spc, string projectDir)
    {
        var resolvedFiles = new List<string>();

        foreach (var path in filePaths)
        {
            //if (!Debugger.IsAttached) Debugger.Launch();

            // absolute path from project root
            var absolutePath = Path.Combine(projectDir, path);

            // wildcard support
            if (absolutePath.Contains("*") || absolutePath.Contains("?"))
            {
                var dir = Path.GetDirectoryName(absolutePath) ?? projectDir;
                var pattern = Path.GetFileName(absolutePath);
                var matches = Directory.GetFiles(dir, pattern);
                if(matches.Length == 0)
                {
                    ReportMissingFile(path);
                    continue;
                }
                resolvedFiles.AddRange(matches);
            }
            else if (File.Exists(absolutePath))
            {
                resolvedFiles.Add(absolutePath);
            }
            else
            {
                ReportMissingFile(path);
            }
        }
        return resolvedFiles.Distinct().ToArray();

        void ReportMissingFile(string path)
        {
            var attrSyntax = input.Attribute.ApplicationSyntaxReference?.GetSyntax() as AttributeSyntax;
            Location location = attrSyntax?.GetLocation() ?? Location.None;
            // Report diagnostic for missing file
            var diag = Diagnostic.Create(
                Diagnostics.MissingJsonFileDescriptor,
                location,
                path);
            spc.ReportDiagnostic(diag);
        }
    }

    private Dictionary<string, object> LoadDictionaryFromFiles(SourceProductionContext spc, string[] filePaths)
    {
        var combinedDict = new Dictionary<string, object>();
        foreach (var path in filePaths)
        {
            var json = File.ReadAllText(path);
            var dict = Json.Deserialize(json) as Dictionary<string, object>;
            if (dict != null)
            {
                MergeDictionaries(combinedDict, dict);
            }
        }
        return combinedDict;
    }

    private static void GenerateFromValue(string name, object element, StringBuilder sb1, StringBuilder sb2, int indentLevel, string seperator, string path)
    {
        string indent = new string(' ', indentLevel * 4);
        if (element is Dictionary<string, object> dict && dict.Any())
        {
            if(!string.IsNullOrEmpty(path))
            {
                // create the class in the sb2
                string className = path.Replace(seperator, string.Empty);
                sb2.AppendLine($"   public static class {className}");
                sb2.AppendLine($"   {{");
                //generate class content here
                sb2.AppendLine($"   }}");
            }
            foreach (var item in dict)
            {
                var childClass = item.Key;
                var value = item.Value;
                GenerateFromValue(childClass, value, sb1, sb2, indentLevel+1, seperator, $"{path}{seperator}{childClass}");
            }
            sb1.AppendLine($"{indent}}}");
        }
        else if (element is List<object> list)
        {
            string className = path.Replace(seperator, string.Empty);
            //Generate class content in sb2 here again
            sb1.AppendLine($"{indent}public static readonly {className} ");
            sb1.AppendLine($"{indent}{{");
            foreach (var item in list)
            {
                sb1.AppendLine($"{indent}    public static PermissionTag {item} = new (\"{path}.{item}\");");
            }
            sb1.AppendLine($"{indent}}}");
        }
        else
        {
        }
    }


    private string GenerateFromJson(string json, string fileName, string @namespace, string jsonFileName, string seperator)
    {
        var doc = Json.Deserialize(json);

        var sb = new StringBuilder();

        sb.AppendLine("using JsonConstGenerator;");
        sb.AppendLine($"namespace {@namespace};");
        sb.AppendLine(Environment.NewLine);
        sb.AppendLine($"//Generated by {jsonFileName}");
        sb.AppendLine($"public static partial class {fileName}");
        sb.AppendLine("{");


        var paths = new List<string>();
        if(doc is Dictionary<string, object> dict)
        {
            foreach(var item in dict)
            {
                GenerateRecursive(item.Value, sb, item.Key, 1, item.Key, paths, seperator);
            }

        }
        sb.AppendLine(Environment.NewLine);
        sb.AppendLine(Environment.NewLine);
        sb.AppendLine("    public IEnumerable<JsonConstNode>> GetAll()");
        sb.AppendLine("    {");
        for(int i=0;i<paths.Count; i++)
        {
            sb.Append($"       yield return {paths[i]};");
        }
        sb.AppendLine("    }");
        sb.AppendLine("}");

        return sb.ToString();
    }

    private void GenerateRecursive(object element, StringBuilder sb, string className, int indentLevel, string path, List<string> endpaths, string seperator)
    {
        string indent = new string(' ', indentLevel * 4);

        if(element is Dictionary<string, object> dict && dict.Any())
        {
            sb.AppendLine($"{indent}public static class {className} : JsonConstNode");
            sb.AppendLine($"{indent}{{");
            sb.AppendLine($"{indent}    public static JsonConstNode self = new(\"{path}\");");
            foreach (var item in dict)
            {
                var childClass = item.Key;
                var value = item.Value;
                GenerateRecursive(item.Value, sb, childClass, indentLevel+1, $"{path}.{childClass}", endpaths, seperator);
            }
            sb.AppendLine($"{indent}}}");
        }
        else if(element is List<object> list)
        {
            sb.AppendLine($"{indent}public static class {className}");
            sb.AppendLine($"{indent}{{");
            foreach (var item in list)
            {
                sb.AppendLine($"{indent}    public static JsonConstNode {item} = new (\"{path}{seperator}{item}\");");
                endpaths.Add($"{path}.{item}");
            }
            sb.AppendLine($"{indent}}}");
        }
        else
        {
            sb.AppendLine($"{indent}public static JsonConstNode {className} = new (\"{path}\");");
            endpaths.Add(path);
        }
    }
}
