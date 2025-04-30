using System;
using System.Text;

public class BST : ITree
{
    public BSTNode Root { get; private set; }
    public int NodeCount { get; private set; }

    public void Insert(int value)
    {
        Root = InsertRec(Root, value);
        NodeCount++;
    }

    private BSTNode InsertRec(BSTNode node, int value)
    {
        if (node == null) return new BSTNode(value);

        if (value < node.Value)
            node.Left = InsertRec(node.Left, value);
        else if (value > node.Value)
            node.Right = InsertRec(node.Right, value);

        return node;
    }

    public int Depth() => DepthRec(Root);

    private int DepthRec(BSTNode node)
    {
        if (node == null) return 0;
        return 1 + Math.Max(DepthRec(node.Left), DepthRec(node.Right));
    }

    public bool IsValid() => IsValidBST(Root, int.MinValue, int.MaxValue);

    private bool IsValidBST(BSTNode node, int min, int max)
    {
        if (node == null) return true;
        if (node.Value < min || node.Value > max) return false;
        return IsValidBST(node.Left, min, node.Value - 1) &&
               IsValidBST(node.Right, node.Value + 1, max);
    }

    public string GetTreeType() => "BST";

    public string Traversal() => InOrder(Root, new StringBuilder()).ToString();

    private StringBuilder InOrder(BSTNode node, StringBuilder sb)
    {
        if (node == null) return sb;
        InOrder(node.Left, sb);
        sb.Append(node.Value + " ");
        InOrder(node.Right, sb);
        return sb;
    }

    public bool CheckChallenge(ChallengeType challenge)
    {
        return challenge switch
        {
            ChallengeType.DepthReached => Depth() >= 6,
            ChallengeType.CompleteStructure => IsComplete(Root),
            _ => false
        };
    }

    private bool IsComplete(BSTNode node)
    {
        if (node == null) return true;
        if (node.Left == null && node.Right == null) return true;
        if (node.Left != null && node.Right != null)
            return IsComplete(node.Left) && IsComplete(node.Right);
        return false;
    }
}
