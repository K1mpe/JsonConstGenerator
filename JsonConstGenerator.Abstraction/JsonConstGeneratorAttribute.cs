using System;

namespace JsonConstGenerator
{

    /// <summary>
    /// Place this attribute on a Partial class. This will generate all the properties from the provided Json file as constant properties
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class JsonConstGeneratorAttribute : Attribute
    {
        public JsonConstGeneratorAttribute(params string[] filepaths)
        {
            FilePaths = filepaths;
        }
        public string[] FilePaths { get; set; }
        public string Separator { get; set; } = ".";
        public bool UpperCamelCase { get; set; } = false;
    }
}
