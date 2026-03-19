using Microsoft.CodeAnalysis;

namespace JsonConstGenerator;

internal static class Diagnostics
{
    public static readonly DiagnosticDescriptor ClassMustBePartial =
        new DiagnosticDescriptor(
            id: "JCG001",
            title: "Class must be partial",
            messageFormat: "Class '{0}' must be declared partial to use JsonConstGenerator",
            category: "JsonConstGenerator",
            DiagnosticSeverity.Error,
            isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor GenericClassNotSupported =
        new DiagnosticDescriptor(
            id: "JCG002",
            title: "Generic classes are not supported",
            messageFormat: "Class '{0}' cannot be generic when using JsonConstGenerator",
            category: "JsonConstGenerator",
            DiagnosticSeverity.Error,
            isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor NoFilesProvided =
        new DiagnosticDescriptor(
            id: "JCG003",
            title: "No JSON files provided",
            messageFormat: "JsonConstGeneratorAttribute on '{0}' must specify at least one JSON file",
            category: "JsonConstGenerator",
            DiagnosticSeverity.Error,
            isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor MissingJsonFileDescriptor =
        new DiagnosticDescriptor(
            id: "JCG004",
            title: "JSON file not found",
            messageFormat: "The JSON file '{0}' specified in [JsonConstGenerator] was not found in {1}",
            category: "JsonConstGenerator",
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor JsonParseError =
    new DiagnosticDescriptor(
        id: "JCG005",
        title: "JSON file cannot be parsed",
        messageFormat: "The JSON file '{0}' could not be parsed as a valid JSON object",
        category: "JsonConstGenerator",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);
}
