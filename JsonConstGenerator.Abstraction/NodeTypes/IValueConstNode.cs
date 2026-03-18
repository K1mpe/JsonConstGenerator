namespace JsonConstGenerator;

/// <summary>
/// A value node is a <see cref="IConstNode"/> that also contains a value
/// </summary>
interface IValueConstNode : IConstNode
{
    /// <summary>
    /// The current value of this node
    /// </summary>
    public object NodeValue { get; }
}
