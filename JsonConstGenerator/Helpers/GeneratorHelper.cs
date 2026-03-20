using Microsoft.CodeAnalysis;
using System.Collections.Generic;

namespace JsonConstGenerator.Helpers
{
    static class GeneratorHelper
    {
        public static string GetNamespace(INamedTypeSymbol classSymbol)
        {
            return classSymbol.ContainingNamespace.IsGlobalNamespace
            ? null
            : classSymbol.ContainingNamespace.ToDisplayString();
        }
        public static string GetClassModifiers(INamedTypeSymbol classSymbol)
        {
            var modifiers = new List<string>();

            if (classSymbol.DeclaredAccessibility == Accessibility.Public)
                modifiers.Add("public");
            else if (classSymbol.DeclaredAccessibility == Accessibility.Internal)
                modifiers.Add("internal");
            else
                modifiers.Add("private"); // rare, but for completeness

            if (classSymbol.IsStatic)
                modifiers.Add("static");

            if (classSymbol.IsSealed && classSymbol.IsAbstract) // C# trick for static?
                modifiers.Add("static");

            return string.Join(" ", modifiers);
        }

        
        public static T GetOptionalPropertyFromAttribute<T>(AttributeData attribute, string propertyName, T defaultValue)
        {
            // Look for the named argument "Seperator"
            foreach (var namedArg in attribute.NamedArguments)
            {
                if (namedArg.Key == propertyName && namedArg.Value.Value is T value)
                {
                    return value;
                }
            }
            // Default value
            return defaultValue;
        }
    }
}
