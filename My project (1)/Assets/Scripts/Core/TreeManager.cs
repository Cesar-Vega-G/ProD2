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
    [SerializeField] private GameObject linePrefab;
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
            
            Vector2 startPos = (i == 0) ? new Vector2(2f, 2.7f) : new Vector2(2f, 1f); // jugador 1 a la izquierda, jugador 2 a la derecha

            playerTrees[i] = new PlayerTree
            {
                type = initialTreeType,
                tree = CreateTree(initialTreeType, startPos)
            };
        }
    }

    private object CreateTree(TreeType type, Vector2 rootPos)
    {
        switch (type)
        {
            case TreeType.BST:
                {
                    GameObject go = new GameObject("BST");
                    BST bst = go.AddComponent<BST>();
                    bst.nodePrefab = nodePrefab;
                    bst.linePrefab = linePrefab;
                    bst.rootStartPosition = rootPos;
                    return bst;
                }
            case TreeType.AVL:
                {
                    GameObject go = new GameObject("AVL");
                    AVL avl = go.AddComponent<AVL>();
                    avl.nodePrefab = nodePrefab;
                    avl.linePrefab = linePrefab;
                    avl.rootStartPosition = rootPos;
                    return avl;
                }
            
            case TreeType.BTree:
                {
                    GameObject go = new GameObject("BTree");
                    BTree btree = go.AddComponent<BTree>();
                    btree.nodePrefab = nodePrefab;
                    btree.linePrefab = linePrefab;
                    btree.degree = 2; // min
                    btree.rootStartPosition = rootPos;
                    return btree;
                }
            
            default:
                return null;
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
        Vector2 rootPos;

        if (playerId == 0)
        {
            rootPos = new Vector2(2f, 2f);      // Posición para playerId 1
        }
        else if (playerId == 1)
        {
            rootPos = new Vector2(2f, 2f);    // Posición para playerId 2
        }
        else
        {
            // Posición por defecto para otros IDs, o salir si no quieres crear
            rootPos = new Vector2(0f, 0f);
        }

        playerTrees[playerId].tree = CreateTree(newType, rootPos);
    }
}








