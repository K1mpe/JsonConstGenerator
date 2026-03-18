using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace JsonConstGenerator
{
    internal sealed class GeneratorInput
    {
        public ClassDeclarationSyntax ClassSyntax { get; }
        public INamedTypeSymbol ClassSymbol { get; }
        public AttributeData Attribute { get; }

        public GeneratorInput(
            ClassDeclarationSyntax classSyntax,
            INamedTypeSymbol classSymbol,
            AttributeData attribute)
        {
            ClassSyntax = classSyntax;
            ClassSymbol = classSymbol;
            Attribute = attribute;
        }
    }
}
