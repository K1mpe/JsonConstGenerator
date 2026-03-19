namespace JsonConstGenerator;

/// <summary>
/// A parent node that contains child nodes
/// </summary>
public interface IParentConstNode : IConstNode
{
    /// <summary>
    /// Returns all the child nodes of this parent
    /// </summary>
    /// <returns></returns>
    public IEnumerable<IConstNode> GetChildNodes();


    /// <summary>
    /// Returns all the child nodes of this parent, but also include sub children
    /// </summary>
    /// <returns></returns>
    public IEnumerable<IConstNode> GetChildNodesRecursive();

    /// <summary>
    /// Returns all the child end nodes of this parent, but also include sub children. An end node is a node that does not have any child nodes
    /// </summary>
    /// <returns></returns>
    public IEnumerable<IConstNode> GetChildEndNodesRecursive();
}
