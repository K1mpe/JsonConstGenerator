using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace JsonConstGenerator.Helpers
{

    internal static class JsonMerger
    {
        /// <summary>
        /// Reads and merges multiple JSON files into a single dictionary.
        /// Later files override earlier files in case of conflicts.
        /// </summary>
        /// <param name="filePaths">Absolute paths to JSON files</param>
        /// <returns>Merged dictionary</returns>
        public static Dictionary<string, object> MergeJsonFiles(IEnumerable<string> filePaths, SourceProductionContext spc, AttributeData attributeData)
        {
            var merged = new Dictionary<string, object>();

            foreach (var file in filePaths)
            {
                string jsonText;

                try
                {
                    jsonText = File.ReadAllText(file);
                }
                catch
                {
                    continue;
                }
                var deserialised = MiniJSON.Json.Deserialize(jsonText);
                // parse JSON using MiniJSON
                if (deserialised is not Dictionary<string, object> dict)
                {
                    var attrSyntax = attributeData.ApplicationSyntaxReference?.GetSyntax() as AttributeSyntax;
                    Location location = attrSyntax?.GetLocation() ?? Location.None;

                    spc.ReportDiagnostic(Diagnostic.Create(
                        Diagnostics.JsonParseError,
                        location,
                        file));
                    // optionally report invalid JSON
                    continue;
                }

                MergeDictionary(dict, merged);
            }

            return merged;
        }

        /// <summary>
        /// Recursively merges src into target. 
        /// If a key exists in both, src overrides.
        /// </summary>
        private static void MergeDictionary(Dictionary<string, object> src, Dictionary<string, object> target)
        {
            foreach (var kv in src)
            {
                if (kv.Value is Dictionary<string, object> srcDict &&
                    target.TryGetValue(kv.Key, out var existing) &&
                    existing is Dictionary<string, object> targetDict)
                {
                    // merge nested objects recursively
                    MergeDictionary(srcDict, targetDict);
                }
                else
                {
                    // override with later file value
                    target[kv.Key] = kv.Value;
                }
            }
        }
    }
}
