using UnityEngine;

public class TreeManager : MonoBehaviour
{
    public static TreeManager Instance { get; private set; }

    [System.Serializable]
    public class PlayerTree
    {
        public ITree tree;
        public TreeType type;
    }

    public enum TreeType { BST, AVL, BTree }

    [SerializeField] private TreeType initialTreeType = TreeType.BST;
    private PlayerTree[] playerTrees;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        InitializeTrees();
    }

    private void InitializeTrees()
    {
        playerTrees = new PlayerTree[GameManager.Instance.GetPlayersCount()];

        for (int i = 0; i < playerTrees.Length; i++)
        {
            playerTrees[i] = new PlayerTree
            {
                type = initialTreeType,
                tree = CreateTree(initialTreeType)
            };
        }
    }

    private ITree CreateTree(TreeType type)
    {
        switch (type)
        {
            case TreeType.BST: return new BST();
            case TreeType.AVL: return new AVL();
            case TreeType.BTree: return new BTree(2); // Grado 2 para B-Tree
            default: return new BST();
        }
    }

    public void InsertValue(int playerId, int value)
    {
        if (playerId < 0 || playerId >= playerTrees.Length) return;

        playerTrees[playerId].tree.Insert(value);
        Debug.Log($"Player {playerId + 1} tree: {playerTrees[playerId].tree.Traversal()}");

        // Aquí luego se verificará si se completó un reto
    }

    // Para cambiar el tipo de árbol durante el juego
    public void SwitchTreeType(int playerId, TreeType newType)
    {
        if (playerId >= 0 && playerId < playerTrees.Length)
        {
            playerTrees[playerId].type = newType;
            playerTrees[playerId].tree = CreateTree(newType);
        }
    }
}