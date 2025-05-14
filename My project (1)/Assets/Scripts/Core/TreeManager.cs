using UnityEngine;

public class TreeManager : MonoBehaviour
{
    public static TreeManager Instance { get; private set; }

    [System.Serializable]
    public class PlayerTree
    {
        public object tree;  // Aquí podemos almacenar directamente los tipos de árboles (AVL, BST, BTree)
        public TreeType type;
    }

    public enum TreeType { BST, AVL, BTree }

    [SerializeField] private TreeType initialTreeType = TreeType.BST;
    [SerializeField] private GameObject nodePrefab;  // Prefab para los nodos
    private PlayerTree[] playerTrees;
    private ChallengeSystem challengeSystem;

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
        challengeSystem = FindObjectOfType<ChallengeSystem>();
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

    private object CreateTree(TreeType type)
    {
        // Al crear el árbol, se pasa el prefab para los nodos
        switch (type)
        {
            case TreeType.BST: return new BST(nodePrefab);  // BST con prefab
            case TreeType.AVL: return new AVL(nodePrefab);  // AVL con prefab
            case TreeType.BTree: return new BTree(2, nodePrefab);  // BTree con grado 2 y prefab
            default: return new BST(nodePrefab);  // Default a BST
        }
    }

    public void InsertValue(int playerId, int value)
    {
        if (playerId < 0 || playerId >= playerTrees.Length) return;

        // Trabajar con el árbol directamente (en vez de ITree, usar el tipo concreto)
        if (playerTrees[playerId].tree is BST bst)
        {
            bst.Insert(value);
        }
        else if (playerTrees[playerId].tree is AVL avl)
        {
            avl.Insert(value);
        }
        else if (playerTrees[playerId].tree is BTree bTree)
        {
            bTree.Insert(value);
        }

        Debug.Log($"Player {playerId + 1} tree: {GetTraversal(playerId)}");

        if (challengeSystem != null)
        {
            bool completed = challengeSystem.CheckChallenge(playerId, playerTrees[playerId].tree);
            if (completed)
            {
                Debug.Log($"🎉 Player {playerId + 1} COMPLETED the challenge: {challengeSystem.GetCurrentChallengeDescription()}");
                challengeSystem.GenerateRandomChallenge();
                GameManager.Instance?.AddScore(playerId, 50); // Bonus de ejemplo
            }
        }
    }

    public string GetTraversal(int playerId)
    {
        if (playerId < 0 || playerId >= playerTrees.Length)
            return string.Empty;

        var tree = playerTrees[playerId].tree;
        if (tree is BST bst)
            return bst.Traversal();
        else if (tree is AVL avl)
            return avl.Traversal();
        else if (tree is BTree bTree)
            return bTree.Traversal();

        return string.Empty;
    }

    public object GetPlayerTree(int playerId)
    {
        if (playerId < 0 || playerId >= playerTrees.Length)
        {
            Debug.LogWarning("Jugador no encontrado");
            return null;
        }
        return playerTrees[playerId].tree;
    }

    public void SwitchTreeType(int playerId, TreeType newType)
    {
        if (playerId >= 0 && playerId < playerTrees.Length)
        {
            playerTrees[playerId].type = newType;
            playerTrees[playerId].tree = CreateTree(newType);
        }
    }
}







