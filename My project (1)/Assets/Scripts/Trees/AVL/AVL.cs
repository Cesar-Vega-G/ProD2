using System;
using UnityEngine;

public class AVL : MonoBehaviour
{
    private class AVLNode
    {
        public int Value;
        public AVLNode Left;
        public AVLNode Right;
        public int Height;
        public Vector2 Position;
        public GameObject visualNodeObject;

        public AVLNode(int value)
        {
            Value = value;
            Height = 1;
            Left = Right = null;
            Position = Vector2.zero;
        }
    }
    public Vector2 rootStartPosition = new Vector2(2f, 2f);
    private AVLNode root;
    public GameObject nodePrefab;
    public GameObject linePrefab;
    private readonly System.Collections.Generic.List<GameObject> lines = new System.Collections.Generic.List<GameObject>();

    public int Depth()
    {
        return GetDepth(root);
    }

    private int GetDepth(AVLNode node)
    {
        if (node == null)
            return 0;

        return 1 + Math.Max(GetDepth(node.Left), GetDepth(node.Right));
    }
    
    public void Insert(int value)
    {
        root = InsertRec(root, value, rootStartPosition);
        float initialHorizontalSpacing =0.5f;
        UpdateNodePositions(root, rootStartPosition, initialHorizontalSpacing);
        UpdateAllLines();
    }

    private AVLNode InsertRec(AVLNode node, int value, Vector2 position)
    {
        if (node == null)
        {
            AVLNode newNode = new AVLNode(value) { Position = position };
            CreateNodePrefab(newNode);
            return newNode;
        }

        float offsetX = 0.5f; // ajusta según quieras distancia menor o mayor
        float offsetY = 0.5f;

        if (value < node.Value)
        {
            Vector2 leftPos = new Vector2(position.x - offsetX, position.y - offsetY);
            node.Left = InsertRec(node.Left, value, leftPos);
            CreateLine(node.Position, leftPos);
        }
        else if (value > node.Value)
        {
            Vector2 rightPos = new Vector2(position.x + offsetX, position.y - offsetY);
            node.Right = InsertRec(node.Right, value, rightPos);
            CreateLine(node.Position, rightPos);
        }
        else
        {
            Debug.Log($"Valor {value} ya existe en el árbol.");
        }

        node.Height = 1 + Math.Max(GetHeight(node.Left), GetHeight(node.Right));

        int balance = GetBalance(node);

        // Rotaciones AVL
        if (balance > 1 && value < node.Left.Value)
            return RightRotate(node);

        if (balance < -1 && value > node.Right.Value)
            return LeftRotate(node);

        if (balance > 1 && value > node.Left.Value)
        {
            node.Left = LeftRotate(node.Left);
            return RightRotate(node);
        }

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

        x.Right = y;
        y.Left = T2;

        y.Height = 1 + Math.Max(GetHeight(y.Left), GetHeight(y.Right));
        x.Height = 1 + Math.Max(GetHeight(x.Left), GetHeight(x.Right));

        return x;
    }

    private AVLNode LeftRotate(AVLNode x)
    {
        AVLNode y = x.Right;
        AVLNode T2 = y.Left;

        y.Left = x;
        x.Right = T2;

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
        if (node == null) return 0;
        return GetHeight(node.Left) - GetHeight(node.Right);
    }

    private void CreateNodePrefab(AVLNode node)
    {
        GameObject newNodeObject = Instantiate(nodePrefab, new Vector3(node.Position.x, node.Position.y, 0), Quaternion.identity, this.transform);
        newNodeObject.name = node.Value.ToString();

        var token = newNodeObject.GetComponent<Token>();
        if (token != null)
            token.Initialize(node.Value);

        node.visualNodeObject = newNodeObject;
    }
    private void UpdateNodePositions(AVLNode node, Vector2 position, float horizontalSpacing)
    {
        if (node == null) return;

        node.Position = position;

        if (node.visualNodeObject != null)
            node.visualNodeObject.transform.position = new Vector3(position.x, position.y, 0);


        float offsetY = 0.3f;

        float childSpacing = horizontalSpacing / 2f;

        UpdateNodePositions(node.Left, new Vector2(position.x - horizontalSpacing, position.y - offsetY), childSpacing);
        UpdateNodePositions(node.Right, new Vector2(position.x + horizontalSpacing, position.y - offsetY), childSpacing);
    }


    private void CreateLine(Vector2 startPos, Vector2 endPos)
    {
        if (linePrefab == null) return;

        GameObject lineObj = Instantiate(linePrefab, Vector3.zero, Quaternion.identity, this.transform);
        lines.Add(lineObj);

        LineRenderer lr = lineObj.GetComponent<LineRenderer>();
        if (lr != null)
        {
            lr.positionCount = 2;
            lr.SetPosition(0, new Vector3(startPos.x, startPos.y, 0));
            lr.SetPosition(1, new Vector3(endPos.x, endPos.y, 0));
        }
    }
    private void ClearLines()
    {
        foreach (var line in lines)
        {
            if (line != null)
                Destroy(line);
        }
        lines.Clear();
    }
    private void UpdateAllLines()
    {
        ClearLines();
        CreateLinesRec(root);
    }

    private void CreateLinesRec(AVLNode node)
    {
        if (node == null) return;

        if (node.Left != null)
        {
            CreateLine(node.Position, node.Left.Position);
            CreateLinesRec(node.Left);
        }
        if (node.Right != null)
        {
            CreateLine(node.Position, node.Right.Position);
            CreateLinesRec(node.Right);
        }
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

    public string Traversal()
    {
        return InOrderTraversal(root);
    }

    private string InOrderTraversal(AVLNode node)
    {
        if (node == null) return "";
        return InOrderTraversal(node.Left) + node.Value + " " + InOrderTraversal(node.Right);
    }
}



