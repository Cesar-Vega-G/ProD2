using System;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

public class BTree : MonoBehaviour
{
    private class BTreeNode
    {
        public List<int> Keys;
        public List<BTreeNode> Children;
        public bool IsLeaf;
        public Vector2 Position;  // Para la posición del nodo

        public BTreeNode(bool isLeaf)
        {
            Keys = new List<int>();
            Children = new List<BTreeNode>();
            IsLeaf = isLeaf;
            Position = Vector2.zero;  // Inicializa la posición
        }
    }

    private BTreeNode root;
    private readonly int degree;
    private GameObject nodePrefab;  // Prefab del nodo

    public BTree(int degree, GameObject prefab)
    {
        this.degree = degree;
        nodePrefab = prefab;
        root = new BTreeNode(true);
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
        InsertNonFull(root, key, Vector2.zero);  // Posición inicial
    }
    public bool IsPerfect()
    {
        return IsPerfectRec(root, 0);
    }

    // Método recursivo que verifica si el árbol B es perfecto
    private bool IsPerfectRec(BTreeNode node, int currentDepth)
    {
        if (node == null)
            return true;

        // Si es una hoja, verificamos si está en la misma profundidad que las demás hojas
        if (node.IsLeaf)
        {
            return currentDepth == GetLeafDepth();
        }

        // Verificamos que todos los nodos internos estén llenos
        if (node.Keys.Count != (2 * degree - 1))
            return false;

        // Verificamos recursivamente en los hijos
        bool isPerfect = true;
        foreach (var child in node.Children)
        {
            isPerfect &= IsPerfectRec(child, currentDepth + 1);
        }
        return isPerfect;
    }
    public int Depth()
    {
        return GetLeafDepth();
    }


    // Obtiene la profundidad de las hojas
    private int GetLeafDepth()
    {
        // La profundidad de las hojas es la cantidad de nodos en el camino más largo desde la raíz a una hoja
        int depth = 0;
        BTreeNode node = root;
        while (node != null && !node.IsLeaf)
        {
            node = node.Children[0]; // Siempre baja por el primer hijo
            depth++;
        }
        return depth;
    }
    private void InsertNonFull(BTreeNode node, int key, Vector2 position)
    {
        int i = node.Keys.Count - 1;

        if (node.IsLeaf)
        {
            while (i >= 0 && key < node.Keys[i])
            {
                i--;
            }
            node.Keys.Insert(i + 1, key);
            node.Position = position;  // Establecemos la posición del nodo en la escena
            CreateNodePrefab(node);    // Crear el prefab del nodo en la escena
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
            InsertNonFull(node.Children[i], key, new Vector2(position.x + 2, position.y - 2));
        }
    }

    private void SplitChild(BTreeNode parentNode, int childIndex, BTreeNode childNode)
    {
        var newNode = new BTreeNode(childNode.IsLeaf);

        // Mover las claves al nuevo nodo
        for (int j = 0; j < degree - 1; j++)
        {
            newNode.Keys.Add(childNode.Keys[j + degree]);
        }

        // Mover los hijos si no es una hoja
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

        // Actualizamos la posición del nuevo nodo
        newNode.Position = new Vector2(parentNode.Position.x + 2, parentNode.Position.y - 2);
        CreateNodePrefab(newNode);
    }

    private void CreateNodePrefab(BTreeNode node)
    {
        // Crear el prefab en la posición correspondiente
        GameObject newNodeObject = Instantiate(nodePrefab, new Vector3(node.Position.x, node.Position.y, 0), Quaternion.identity);
        newNodeObject.name = string.Join(",", node.Keys);  // Nombre del nodo es el conjunto de claves
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
}


