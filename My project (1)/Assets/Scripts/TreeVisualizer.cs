using UnityEngine;
using System.Collections.Generic;

public class TreeVisualizer : MonoBehaviour
{
    [Header("Prefab & Settings")]
    public GameObject nodePrefab;               // Prefab para el nodo visual
    public float horizontalSpacing = 2.5f;      // Espacio horizontal entre nodos
    public float verticalSpacing = 2.5f;        // Espacio vertical entre niveles

    private List<GameObject> spawnedNodes = new List<GameObject>();

    
    // Llama este método para redibujar el árbol de un jugador
    public void DrawTree(ITree tree)
    {
        //ClearTree();  // 🔄 Primero limpiamos cualquier árbol previo

        if (tree == null)
        {
            Debug.LogWarning("TreeVisualizer: No tree to draw.");
            return;
        }

        // Detectamos el tipo de árbol (solo binarios por ahora)
        if (tree is AVL avl)
        {
            Debug.LogWarning("1");
            DrawBinaryTree(GetRoot(avl), transform.position, horizontalSpacing);
        }
        else if (tree is BST bst)
        {
            Debug.LogWarning("1");
            DrawBinaryTree(GetRoot(bst), transform.position, horizontalSpacing);
        }
        else
        {
            Debug.LogWarning("TreeVisualizer: Tipo de árbol no soportado todavía.");
        }
    }

    // 🔄 Borra los nodos actuales
    public void ClearTree()
    {
        foreach (var node in spawnedNodes)
        {
            Destroy(node);
        }
        spawnedNodes.Clear();
    }

    // 🎯 Dibuja un árbol binario (BST o AVL)
    private void DrawBinaryTree(object node, Vector3 position, float hSpacing)
    {
        if (node == null) return;

        int value = GetNodeValue(node);

        // 👉 Crear el nodo visual
        var nodeObj = Instantiate(nodePrefab, position, Quaternion.identity, transform);
        spawnedNodes.Add(nodeObj);

        var visual = nodeObj.GetComponent<TreeNodeVisual>();
        if (visual != null)
        {
            visual.SetValue(value);
        }

        // 🌿 Dibujar hijos
        object left = GetLeft(node);
        object right = GetRight(node);

        if (left != null)
        {
            Vector3 leftPos = position + new Vector3(-hSpacing, -verticalSpacing, 0);
            DrawBinaryTree(left, leftPos, hSpacing / 1.5f);
        }

        if (right != null)
        {
            Vector3 rightPos = position + new Vector3(hSpacing, -verticalSpacing, 0);
            DrawBinaryTree(right, rightPos, hSpacing / 1.5f);
        }
    }

    // Helpers: extraen valor, hijo izquierdo, hijo derecho (AVL y BST)
    private int GetNodeValue(object node)
    {
        if (node is AVLNode avlNode)
            return avlNode.Value;
        if (node is BSTNode bstNode)
            return bstNode.Value;
        return -1;
    }

    private object GetLeft(object node)
    {
        if (node is AVLNode avlNode)
            return avlNode.Left;
        if (node is BSTNode bstNode)
            return bstNode.Left;
        return null;
    }

    private object GetRight(object node)
    {
        if (node is AVLNode avlNode)
            return avlNode.Right;
        if (node is BSTNode bstNode)
            return bstNode.Right;
        return null;
    }

    // Helpers: obtener raíz del árbol
    private AVLNode GetRoot(AVL avl)
    {
        // Asumimos que tu clase AVL tiene una forma de exponer la raíz (agregá esto si no existe)
        return (AVLNode)GetPrivateField(avl, "root");
    }

    private BSTNode GetRoot(BST bst)
    {
        return (BSTNode)GetPrivateField(bst, "root");
    }

    private object GetPrivateField(object obj, string fieldName)
    {
        var type = obj.GetType();
        var field = type.GetField(fieldName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        return field?.GetValue(obj);
    }
}

