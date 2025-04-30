using System;

public class AVL : ITree
{
    private class AVLNode
    {
        public int Value;
        public AVLNode Left;
        public AVLNode Right;
        public int Height;

        public AVLNode(int value)
        {
            Value = value;
            Height = 1;
        }
    }

    private AVLNode root;
    public int NodeCount { get; private set; }

    public void Insert(int value)
    {
        root = Insert(root, value);
        NodeCount++;
    }

    private AVLNode Insert(AVLNode node, int value)
    {
        if (node == null)
            return new AVLNode(value);

        if (value < node.Value)
            node.Left = Insert(node.Left, value);
        else if (value > node.Value)
            node.Right = Insert(node.Right, value);
        else
            return node; // No se permiten valores duplicados

        // Actualizar altura del nodo
        node.Height = 1 + Math.Max(GetHeight(node.Left), GetHeight(node.Right));

        // Balancear el árbol
        int balance = GetBalance(node);

        // Casos de rotación
        // Left Left
        if (balance > 1 && value < node.Left.Value)
            return RightRotate(node);

        // Right Right
        if (balance < -1 && value > node.Right.Value)
            return LeftRotate(node);

        // Left Right
        if (balance > 1 && value > node.Left.Value)
        {
            node.Left = LeftRotate(node.Left);
            return RightRotate(node);
        }

        // Right Left
        if (balance < -1 && value < node.Right.Value)
        {
            node.Right = RightRotate(node.Right);
            return LeftRotate(node);
        }

        return node;
    }

    private AVLNode RightRotate(AVLNode y)
    {
        AVLNode x = y.Left;
        AVLNode T2 = x.Right;

        // Realizar rotación
        x.Right = y;
        y.Left = T2;

        // Actualizar alturas
        y.Height = 1 + Math.Max(GetHeight(y.Left), GetHeight(y.Right));
        x.Height = 1 + Math.Max(GetHeight(x.Left), GetHeight(x.Right));

        return x;
    }

    private AVLNode LeftRotate(AVLNode x)
    {
        AVLNode y = x.Right;
        AVLNode T2 = y.Left;

        // Realizar rotación
        y.Left = x;
        x.Right = T2;

        // Actualizar alturas
        x.Height = 1 + Math.Max(GetHeight(x.Left), GetHeight(x.Right));
        y.Height = 1 + Math.Max(GetHeight(y.Left), GetHeight(y.Right));

        return y;
    }

    private int GetHeight(AVLNode node)
    {
        return node?.Height ?? 0;
    }

    private int GetBalance(AVLNode node)
    {
        if (node == null)
            return 0;
        return GetHeight(node.Left) - GetHeight(node.Right);
    }

    public int Depth()
    {
        return GetHeight(root);
    }

    public bool IsValid()
    {
        return IsBalanced(root) && IsBST(root, int.MinValue, int.MaxValue);
    }

    private bool IsBalanced(AVLNode node)
    {
        if (node == null)
            return true;

        int balance = GetBalance(node);
        return Math.Abs(balance) <= 1 && IsBalanced(node.Left) && IsBalanced(node.Right);
    }

    private bool IsBST(AVLNode node, int min, int max)
    {
        if (node == null)
            return true;

        if (node.Value < min || node.Value > max)
            return false;

        return IsBST(node.Left, min, node.Value - 1) &&
               IsBST(node.Right, node.Value + 1, max);
    }

    public string GetTreeType()
    {
        return "AVL";
    }

    public string Traversal()
    {
        return InOrderTraversal(root);
    }

    private string InOrderTraversal(AVLNode node)
    {
        if (node == null)
            return "";

        return InOrderTraversal(node.Left) + node.Value + " " + InOrderTraversal(node.Right);
    }

    public bool CheckChallenge(ChallengeType challenge)
    {
        return challenge switch
        {
            ChallengeType.DepthReached => Depth() >= 5,
            ChallengeType.BalancedTree => IsBalanced(root),
            _ => false
        };
    }
}
