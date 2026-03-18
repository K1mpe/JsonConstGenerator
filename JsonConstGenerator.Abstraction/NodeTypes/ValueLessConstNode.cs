namespace JsonConstGenerator;

/// <summary>
/// A endnode which does not contain a value
/// </summary>
public struct ValueLessConstNode : IConstNode
{
    public ValueLessConstNode(string path)
    {
        NodePath = path;
    }
    public string NodePath { get; }

    public override string ToString() => NodePath;
    public static implicit operator string(ValueLessConstNode node) => node.NodePath;
}
