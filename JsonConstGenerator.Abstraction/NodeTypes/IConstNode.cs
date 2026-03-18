using System.Runtime.CompilerServices;

namespace JsonConstGenerator
{
    /// <summary>
    /// The base interface that is applied on all the const nodes
    /// </summary>
    public interface IConstNode
    {
        /// <summary>
        /// Get the absolute path of this node
        /// </summary>
        public string NodePath { get; }
    }
}