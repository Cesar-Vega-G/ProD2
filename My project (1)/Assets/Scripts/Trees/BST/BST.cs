using UnityEngine;
using System.Text;
using System;

public class BST : MonoBehaviour
{
    public GameObject nodePrefab;  // Prefab para crear nodos (asignado desde TreeManager)
    public GameObject linePrefab;
    public class BSTNode
    {
        public int Value;
        public BSTNode Left;
        public BSTNode Right;
        public Vector2 Position;

        public BSTNode(int value)
        {
            Value = value;
            Left = Right = null;
            Position = Vector2.zero;
        }
    }
    public Vector2 rootStartPosition = new Vector2(2f, 2f);
    private BSTNode root;

    public void Insert(int value)
    {
        root = InsertRec(root, value, rootStartPosition, 0);
    }

    private BSTNode InsertRec(BSTNode node, int value, Vector2 position, int depth)
    {
        if (node == null)
        {
            BSTNode newNode = new BSTNode(value) { Position = position };
            CreateNodePrefab(newNode);
            return newNode;
        }

        float initialHorizontalSpacing = 0.5f;  // distancia base
        float offsetX = initialHorizontalSpacing / Mathf.Pow(2, depth);
        float offsetY = 0.3f;  // vertical fijo 

        if (value < node.Value)
        {
            Vector2 leftPos = new Vector2(node.Position.x - offsetX, node.Position.y - offsetY);
            node.Left = InsertRec(node.Left, value, leftPos, depth + 1);
            CreateLine(node.Position, leftPos);
        }
        else if (value > node.Value)
        {
            Vector2 rightPos = new Vector2(node.Position.x + offsetX, node.Position.y - offsetY);
            node.Right = InsertRec(node.Right, value, rightPos, depth + 1);
            CreateLine(node.Position, rightPos);
        }
        else
        {
            Debug.Log($"Valor {value} ya existe en el árbol.");
        }

        return node;
    }
    private void CreateLine(Vector2 startPos, Vector2 endPos)
    {
        if (linePrefab == null) return;

        GameObject lineObj = Instantiate(linePrefab, Vector3.zero, Quaternion.identity, this.transform);
        LineRenderer lr = lineObj.GetComponent<LineRenderer>();
        if (lr != null)
        {
            lr.positionCount = 2;
            lr.SetPosition(0, new Vector3(startPos.x, startPos.y, 0));
            lr.SetPosition(1, new Vector3(endPos.x, endPos.y, 0));
        }
    }
    private void CreateNodePrefab(BSTNode node)
    {
        GameObject newNode = Instantiate(nodePrefab, new Vector3(node.Position.x, node.Position.y, 0), Quaternion.identity);
        newNode.name = node.Value.ToString();
        newNode.transform.parent = this.transform;

        Token tokenComponent = newNode.GetComponent<Token>();
        if (tokenComponent != null)
        {
            tokenComponent.Initialize(node.Value);
        }
        else
        {
            Debug.LogWarning("El prefab no tiene el componente Token.");
        }
    }
    public bool Contains(int value)
    {
        return ContainsRec(root, value);
    }

    private bool ContainsRec(BSTNode node, int value)
    {
        if (node == null) return false;
        if (node.Value == value) return true;
        if (value < node.Value)
            return ContainsRec(node.Left, value);
        else
            return ContainsRec(node.Right, value);
    }

    public int Depth()
    {
        return GetDepth(root);
    }

    private int GetDepth(BSTNode node)
    {
        if (node == null) return 0;
        return 1 + Mathf.Max(GetDepth(node.Left), GetDepth(node.Right));
    }
    public bool IsBalanced()
    {
        return CheckBalance(root) != -1;
    }

    private int CheckBalance(BSTNode node)
    {
        if (node == null) return 0;

        int leftHeight = CheckBalance(node.Left);
        if (leftHeight == -1) return -1;  // No balanceado en subárbol izquierdo

        int rightHeight = CheckBalance(node.Right);
        if (rightHeight == -1) return -1; // No balanceado en subárbol derecho

        if (Math.Abs(leftHeight - rightHeight) > 1)
            return -1;  // Diferencia mayor a 1, no balanceado

        return 1 + Mathf.Max(leftHeight, rightHeight);
    }
    public string Traversal()
    {
        StringBuilder sb = new StringBuilder();
        InOrder(root, sb);
        return sb.ToString();
    }

    private void InOrder(BSTNode node, StringBuilder sb)
    {
        if (node == null) return;
        InOrder(node.Left, sb);
        sb.Append(node.Value + " ");
        InOrder(node.Right, sb);
    }
}




