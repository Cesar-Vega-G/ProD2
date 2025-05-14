using System;
using UnityEngine;

public class AVL : MonoBehaviour
{
    private class AVLNode : MonoBehaviour
    {
        public int Value;
        public AVLNode Left;
        public AVLNode Right;
        public int Height;
        public Vector2 Position;  // Para la posición del nodo

        public AVLNode(int value)
        {
            Value = value;
            Height = 1;
            Left = Right = null;
            Position = Vector2.zero; // Inicializa la posición
        }
    }

    private AVLNode root;
    private GameObject nodePrefab;  // Prefab del nodo

    public AVL(GameObject prefab)
    {
        nodePrefab = prefab;
        root = null;
    }
    public int Depth()
    {
        return GetDepth(root);
    }

    // Método recursivo para obtener la profundidad de un nodo
    private int GetDepth(AVLNode node)
    {
        if (node == null)
            return 0;  // Un nodo nulo tiene profundidad 0

        // La profundidad de un nodo es 1 + la mayor profundidad de sus hijos
        return 1 + Math.Max(GetDepth(node.Left), GetDepth(node.Right));
    }
    public void Insert(int value)
    {
        root = InsertRec(root, value, Vector2.zero);  // Posición inicial
    }

    private AVLNode InsertRec(AVLNode node, int value, Vector2 position)
    {
        if (node == null)
        {
            AVLNode newNode = new AVLNode(value) { Position = position };
            CreateNodePrefab(newNode);  // Crear el prefab del nodo
            return newNode;
        }

        if (value < node.Value)
            node.Left = InsertRec(node.Left, value, new Vector2(position.x - 2, position.y - 2));  // Ajustar posiciones
        else if (value > node.Value)
            node.Right = InsertRec(node.Right, value, new Vector2(position.x + 2, position.y - 2));

        // Actualizar altura
        node.Height = 1 + Math.Max(GetHeight(node.Left), GetHeight(node.Right));

        // Balancear el árbol
        int balance = GetBalance(node);

        // Casos de rotación
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
    public bool IsValid()
    {
        return IsBalanced(root) && IsBST(root, int.MinValue, int.MaxValue);
    }

    // Verifica si el árbol está balanceado (la diferencia de alturas no debe ser mayor a 1)
    private bool IsBalanced(AVLNode node)
    {
        if (node == null)
            return true;

        int balance = GetBalance(node);
        return Math.Abs(balance) <= 1 && IsBalanced(node.Left) && IsBalanced(node.Right);
    }

    // Calcula el balance de un nodo (diferencia entre las alturas de los subárboles izquierdo y derecho)
    private int GetBalance(AVLNode node)
    {
        if (node == null)
            return 0;
        return GetHeight(node.Left) - GetHeight(node.Right);
    }



    // Verifica si el árbol cumple con las reglas del BST
    private bool IsBST(AVLNode node, int min, int max)
    {
        if (node == null)
            return true;

        if (node.Value < min || node.Value > max)
            return false;

        return IsBST(node.Left, min, node.Value - 1) &&
               IsBST(node.Right, node.Value + 1, max);
    }
    private int GetHeight(AVLNode node)
    {
        return node?.Height ?? 0;
    }



    private void CreateNodePrefab(AVLNode node)
    {
        // Crear el prefab en la posición correspondiente
        GameObject newNodeObject = Instantiate(nodePrefab, new Vector3(node.Position.x, node.Position.y, 0), Quaternion.identity);
        newNodeObject.name = node.Value.ToString();  // Nombre del nodo es el valor
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
}


