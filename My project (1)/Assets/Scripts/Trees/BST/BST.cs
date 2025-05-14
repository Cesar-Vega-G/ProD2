using UnityEngine;
using System;
using System.Text;

public class BST : MonoBehaviour
{
    public class BSTNode : MonoBehaviour
    {
        public int Value;
        public BSTNode Left;
        public BSTNode Right;
        public Vector2 Position;  // Para la posición del nodo

        public BSTNode(int value)
        {
            Value = value;
            Left = Right = null;
            Position = Vector2.zero; // Inicializa la posición
        }
    }

    private BSTNode root;
    private GameObject nodePrefab;  // Prefab del nodo

    public BST(GameObject prefab)
    {
        nodePrefab = prefab;
        root = null;
    }
    public int Depth()
    {
        return GetDepth(root);
    }

    // Método recursivo para obtener la profundidad de un nodo

    public void Insert(int value)
    {
        root = InsertRec(root, value, Vector2.zero);  // Posición inicial
    }

    private BSTNode InsertRec(BSTNode node, int value, Vector2 position)
    {
        if (node == null)
        {
            BSTNode newNode = new BSTNode(value) { Position = position };
            CreateNodePrefab(newNode);  // Crear el prefab del nodo
            return newNode;
        }

        if (value < node.Value)
            node.Left = InsertRec(node.Left, value, new Vector2(position.x - 2, position.y - 2));  // Ajustar posiciones
        else if (value > node.Value)
            node.Right = InsertRec(node.Right, value, new Vector2(position.x + 2, position.y - 2));

        return node;
    }

    private void CreateNodePrefab(BSTNode node)
    {
        // Crear el prefab en la posición correspondiente
        GameObject newNodeObject = Instantiate(nodePrefab, new Vector3(node.Position.x, node.Position.y, 0), Quaternion.identity);
        newNodeObject.name = node.Value.ToString();  // Nombre del nodo es el valor
    }
    public bool IsPerfect()
    {
        // Verifica si el árbol tiene la misma profundidad en todas las hojas y si todos los nodos internos tienen dos hijos
        int depth = GetDepth(root);
        return IsPerfectRec(root, 0, depth);
    }

    // Método recursivo que verifica si el árbol es perfecto
    private bool IsPerfectRec(BSTNode node, int currentDepth, int expectedDepth)
    {
        if (node == null)
        {
            return true; // Si es null, no hay nada que verificar
        }

        // Si es una hoja (nodo sin hijos)
        if (node.Left == null && node.Right == null)
        {
            return currentDepth == expectedDepth;  // Verifica si la hoja está a la profundidad correcta
        }

        // Si no es hoja, asegurarse de que tenga ambos hijos
        if (node.Left == null || node.Right == null)
        {
            return false; // Si falta uno de los hijos, no es perfecto
        }

        // Verifica recursivamente en los subárboles izquierdo y derecho
        return IsPerfectRec(node.Left, currentDepth + 1, expectedDepth) &&
               IsPerfectRec(node.Right, currentDepth + 1, expectedDepth);
    }

    // Método auxiliar para obtener la profundidad del árbol
    private int GetDepth(BSTNode node)
    {
        int depth = 0;
        while (node != null)
        {
            depth++;
            node = node.Left; // Bajar hacia el lado izquierdo (todas las hojas deben estar a la misma profundidad)
        }
        return depth;
    }
    public string Traversal()
    {
        // Recorrido in-order para mostrar los valores de los nodos
        return InOrder(root, new StringBuilder()).ToString();
    }

    private StringBuilder InOrder(BSTNode node, StringBuilder sb)
    {
        if (node == null) return sb;
        InOrder(node.Left, sb);
        sb.Append(node.Value + " ");
        InOrder(node.Right, sb);
        return sb;
    }
}


