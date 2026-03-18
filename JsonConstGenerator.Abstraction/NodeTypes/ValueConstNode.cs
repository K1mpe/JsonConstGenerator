namespace JsonConstGenerator;

/// <summary>
/// A const node which contains a value
/// </summary>
/// <typeparam name="T"></typeparam>
public struct ValueConstNode<T> : IValueConstNode, IConstNode
{
    public ValueConstNode(string path, T value)
    {
        NodePath = path;
        NodeValue = value;
    }
    public T NodeValue { get; }

    public string NodePath { get; }

    object IValueConstNode.NodeValue => NodeValue;

    public override string ToString() => NodePath;
    public static implicit operator T(ValueConstNode<T> node) => node.NodeValue;
}
