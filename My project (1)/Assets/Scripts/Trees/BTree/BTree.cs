using System;
using System.Collections.Generic;
using System.Text;

public class BTree : ITree
{
    private class BTreeNode
    {
        public List<int> Keys;
        public List<BTreeNode> Children;
        public bool IsLeaf;

        public BTreeNode(bool isLeaf)
        {
            Keys = new List<int>();
            Children = new List<BTreeNode>();
            IsLeaf = isLeaf;
        }
    }

    private BTreeNode root;
    private readonly int degree;
    public int NodeCount { get; private set; }

    public BTree(int degree)
    {
        this.degree = degree;
        root = new BTreeNode(true);
        NodeCount = 0;
    }

    public void Insert(int key)
    {
        if (root.Keys.Count == (2 * degree - 1))
        {
            var newRoot = new BTreeNode(false);
            newRoot.Children.Add(root);
            SplitChild(newRoot, 0, root);
            root = newRoot;
        }
        InsertNonFull(root, key);
        NodeCount++;
    }

    private void InsertNonFull(BTreeNode node, int key)
    {
        int i = node.Keys.Count - 1;

        if (node.IsLeaf)
        {
            while (i >= 0 && key < node.Keys[i])
            {
                i--;
            }
            node.Keys.Insert(i + 1, key);
        }
        else
        {
            while (i >= 0 && key < node.Keys[i])
            {
                i--;
            }
            i++;

            if (node.Children[i].Keys.Count == (2 * degree - 1))
            {
                SplitChild(node, i, node.Children[i]);
                if (key > node.Keys[i])
                {
                    i++;
                }
            }
            InsertNonFull(node.Children[i], key);
        }
    }

    private void SplitChild(BTreeNode parentNode, int childIndex, BTreeNode childNode)
    {
        var newNode = new BTreeNode(childNode.IsLeaf);

        // Mover las claves
        for (int j = 0; j < degree - 1; j++)
        {
            newNode.Keys.Add(childNode.Keys[j + degree]);
        }

        // Mover los hijos si no es hoja
        if (!childNode.IsLeaf)
        {
            for (int j = 0; j < degree; j++)
            {
                newNode.Children.Add(childNode.Children[j + degree]);
            }
        }

        // Mover la clave media al padre
        parentNode.Keys.Insert(childIndex, childNode.Keys[degree - 1]);

        // Conectar el nuevo nodo al padre
        parentNode.Children.Insert(childIndex + 1, newNode);

        // Eliminar las claves e hijos movidos
        childNode.Keys.RemoveRange(degree - 1, degree);
        if (!childNode.IsLeaf)
        {
            childNode.Children.RemoveRange(degree, degree);
        }
    }

    public int Depth()
    {
        return CalculateDepth(root);
    }

    private int CalculateDepth(BTreeNode node)
    {
        if (node == null)
            return 0;

        if (node.IsLeaf)
            return 1;

        return 1 + CalculateDepth(node.Children[0]);
    }

    public bool IsValid()
    {
        return IsValidBTree(root, int.MinValue, int.MaxValue);
    }

    private bool IsValidBTree(BTreeNode node, int min, int max)
    {
        if (node == null)
            return true;

        // Verificar orden de las claves
        for (int i = 0; i < node.Keys.Count; i++)
        {
            if (i > 0 && node.Keys[i] <= node.Keys[i - 1])
                return false;

            if (node.Keys[i] <= min || node.Keys[i] >= max)
                return false;
        }

        // Verificar número de claves
        if (node != root && node.Keys.Count < degree - 1)
            return false;
        if (node.Keys.Count > 2 * degree - 1)
            return false;

        // Verificar hijos recursivamente
        if (!node.IsLeaf)
        {
            if (node.Children.Count != node.Keys.Count + 1)
                return false;

            for (int i = 0; i < node.Children.Count; i++)
            {
                int newMin = (i == 0) ? min : node.Keys[i - 1];
                int newMax = (i == node.Children.Count - 1) ? max : node.Keys[i];

                if (!IsValidBTree(node.Children[i], newMin, newMax))
                    return false;
            }
        }

        return true;
    }

    public string GetTreeType()
    {
        return $"B-Tree (Degree {degree})";
    }

    public string Traversal()
    {
        var sb = new StringBuilder();
        LevelOrderTraversal(root, sb);
        return sb.ToString();
    }

    private void LevelOrderTraversal(BTreeNode node, StringBuilder sb)
    {
        if (node == null)
            return;

        var queue = new Queue<BTreeNode>();
        queue.Enqueue(node);

        while (queue.Count > 0)
        {
            BTreeNode current = queue.Dequeue();
            sb.Append("[");
            foreach (int key in current.Keys)
            {
                sb.Append(key + " ");
            }
            sb.Append("] ");

            if (!current.IsLeaf)
            {
                foreach (var child in current.Children)
                {
                    queue.Enqueue(child);
                }
            }
        }
    }

    public bool CheckChallenge(ChallengeType challenge)
    {
        return challenge switch
        {
            ChallengeType.NodeCount => NodeCount >= 15,
            ChallengeType.CompleteStructure => IsPerfect(root),
            _ => false
        };
    }

    public bool IsPerfect()
    {
        return IsPerfect(root);
    }

    private bool IsPerfect(BTreeNode node)
    {
        if (node == null)
            return true;

        if (node.IsLeaf)
            return true;

        if (node.Children.Count != node.Keys.Count + 1)
            return false;

        foreach (var child in node.Children)
        {
            if (!IsPerfect(child))
                return false;
        }

        return true;
    }
}
