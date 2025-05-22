using System;
using System.Collections.Generic;
using UnityEngine;

public class BTree : MonoBehaviour
{
    public GameObject nodePrefab;
    public GameObject linePrefab;
    public int degree = 2; // grado mínimo del árbol B
    public Vector2 rootStartPosition = new Vector2(2f, 2f);
    private class BTreeNode
    {
        public List<int> Keys = new List<int>();
        public List<BTreeNode> Children = new List<BTreeNode>();
        public bool IsLeaf;
        public Vector2 Position;
        public GameObject visualNodeObject;

        public BTreeNode(bool isLeaf)
        {
            IsLeaf = isLeaf;
        }
    }

    private BTreeNode root;

    private readonly List<GameObject> lines = new List<GameObject>();

    private void Start()
    {
        root = new BTreeNode(true);
    }

    public void Insert(int key)
    {
        if (root.Keys.Count == 2 * degree - 1)
        {
            BTreeNode newRoot = new BTreeNode(false);
            newRoot.Children.Add(root);
            SplitChild(newRoot, 0, root);
            root = newRoot;
        }

        InsertNonFull(root, key);

        float initialHorizontalSpacing = 0.5f;
        UpdateNodePositions(root, rootStartPosition, initialHorizontalSpacing);
        UpdateAllLines();
    }

    private void InsertNonFull(BTreeNode node, int key)
    {
        int i = node.Keys.Count - 1;

        if (node.IsLeaf)
        {
            // Insertar la clave en la posición correcta
            node.Keys.Add(0);
            while (i >= 0 && key < node.Keys[i])
            {
                node.Keys[i + 1] = node.Keys[i];
                i--;
            }
            node.Keys[i + 1] = key;

            // Crear el prefab visual para el nodo si no existe
            if (node.visualNodeObject == null)
                CreateNodePrefab(node);
            else
                UpdateNodeVisual(node);
        }
        else
        {
            while (i >= 0 && key < node.Keys[i])
                i--;

            i++;
            if (node.Children[i].Keys.Count == 2 * degree - 1)
            {
                SplitChild(node, i, node.Children[i]);
                if (key > node.Keys[i])
                    i++;
            }
            InsertNonFull(node.Children[i], key);
        }
    }

    private void SplitChild(BTreeNode parent, int index, BTreeNode child)
    {
        BTreeNode newNode = new BTreeNode(child.IsLeaf);

        for (int j = 0; j < degree - 1; j++)
            newNode.Keys.Add(child.Keys[j + degree]);

        if (!child.IsLeaf)
        {
            for (int j = 0; j < degree; j++)
                newNode.Children.Add(child.Children[j + degree]);
        }

        child.Keys.RemoveRange(degree, degree - 1);
        if (!child.IsLeaf)
            child.Children.RemoveRange(degree, degree);

        parent.Children.Insert(index + 1, newNode);
        parent.Keys.Insert(index, child.Keys[degree - 1]);
        child.Keys.RemoveAt(degree - 1);

        // Crear prefab visual para el nuevo nodo creado
        CreateNodePrefab(newNode);
    }

    private void CreateNodePrefab(BTreeNode node)
    {
        // Crea el objeto contenedor para el nodo completo
        GameObject nodeContainer = new GameObject("Node_" + string.Join("_", node.Keys));
        nodeContainer.transform.parent = this.transform;
        nodeContainer.transform.position = new Vector3(node.Position.x, node.Position.y, 0);

        node.visualNodeObject = nodeContainer;

        float spacing = 1.0f; // espacio horizontal entre claves

        // Crea un prefab Token para cada clave y lo posiciona al lado del anterior
        for (int i = 0; i < node.Keys.Count; i++)
        {
            GameObject keyTokenObj = Instantiate(nodePrefab, nodeContainer.transform);
            keyTokenObj.transform.localPosition = new Vector3(i * spacing, 0, 0);
            keyTokenObj.name = "Key_" + node.Keys[i];

            Token token = keyTokenObj.GetComponent<Token>();
            if (token != null)
            {
                token.Initialize2(node.Keys[i].ToString());
            }
        }
    }

    private void UpdateNodeVisual(BTreeNode node)
    {
        if (node.visualNodeObject == null) return;

        var token = node.visualNodeObject.GetComponent<Token>();
        if (token != null)
        {
            token.Initialize(node.Keys[0]);
        }
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

    private void CreateLinesRec(BTreeNode node)
    {
        if (node == null) return;

        if (!node.IsLeaf)
        {
            for (int i = 0; i < node.Children.Count; i++)
            {
                CreateLine(node.Position, node.Children[i].Position);
                CreateLinesRec(node.Children[i]);
            }
        }
    }

    private void UpdateNodePositions(BTreeNode node, Vector2 position, float horizontalSpacing)
    {
        if (node == null) return;

        node.Position = position;

        if (node.visualNodeObject != null)
            node.visualNodeObject.transform.position = new Vector3(position.x, position.y, 0);

        float offsetY = 0.5f;  // distancia vertical
        float childSpacing = horizontalSpacing / 2f;

        if (!node.IsLeaf)
        {
            int childCount = node.Children.Count;
            for (int i = 0; i < childCount; i++)
            {
                // Distribuye hijos horizontalmente centrados en el padre
                float xOffset = (i - (childCount - 1) / 2f) * horizontalSpacing;
                Vector2 childPos = new Vector2(position.x + xOffset, position.y - offsetY);
                UpdateNodePositions(node.Children[i], childPos, childSpacing);
            }
        }
    }

    public string Traversal()
    {
        return InOrderTraversal(root);
    }

    private string InOrderTraversal(BTreeNode node)
    {
        if (node == null) return "";
        string result = "";
        int keyCount = node.Keys.Count;

        for (int i = 0; i < keyCount; i++)
        {
            if (!node.IsLeaf)
                result += InOrderTraversal(node.Children[i]);

            result += node.Keys[i] + " ";
        }

        if (!node.IsLeaf)
            result += InOrderTraversal(node.Children[keyCount]);

        return result;
    }
}



