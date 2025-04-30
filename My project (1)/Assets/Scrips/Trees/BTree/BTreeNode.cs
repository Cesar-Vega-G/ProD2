using System.Collections.Generic;

public class BTreeNode
{
    public List<int> Keys { get; }
    public List<BTreeNode> Children { get; }
    public bool IsLeaf { get; }

    public BTreeNode(bool isLeaf)
    {
        Keys = new List<int>();
        Children = new List<BTreeNode>();
        IsLeaf = isLeaf;
    }
}
