    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using System.Linq;

    namespace JsonConstGenerator;

internal static class Validation
{
    public static bool TryValidateClass(
        SourceProductionContext context,
        ClassDeclarationSyntax classSyntax,
        INamedTypeSymbol classSymbol,
        AttributeData attribute)
    {
        bool isValid = true;
        // 1. Must be partial
        if (!classSyntax.Modifiers.Any(m => m.Text == "partial"))
        {
            context.ReportDiagnostic(
                Diagnostic.Create(
                    Diagnostics.ClassMustBePartial,
                    classSyntax.Identifier.GetLocation(),
                    classSymbol.Name));
            isValid = false;
        }

        // 2. Must not be generic
        if (classSymbol.TypeParameters.Length > 0)
        {
            context.ReportDiagnostic(
                Diagnostic.Create(
                    Diagnostics.GenericClassNotSupported,
                    classSyntax.Identifier.GetLocation(),
                    classSymbol.Name));
            isValid = false;
        }

        // 3. Must have at least one file
        if (attribute.ConstructorArguments.Length == 0 ||
            attribute.ConstructorArguments[0].Values.Length == 0)
        {
            context.ReportDiagnostic(
                Diagnostic.Create(
                    Diagnostics.NoFilesProvided,
                    classSyntax.Identifier.GetLocation(),
                    classSymbol.Name));
            isValid = false;
        }
        return isValid;
    }
}
