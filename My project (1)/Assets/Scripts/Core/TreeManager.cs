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
        Debug.Log("Insert");
        if (playerId < 0 || playerId >= playerTrees.Length) return;

        var tree = playerTrees[playerId].tree;

        tree.Insert(value);

        Debug.Log($"Player {playerId + 1} tree: {tree.Traversal()}");

        // Verificar desafío si el sistema está presente
        if (challengeSystem != null)
        {
            bool completed = challengeSystem.CheckChallenge(playerId, tree);
            if (completed)
            {
                Debug.Log($"🎉 Player {playerId + 1} COMPLETED the challenge: {challengeSystem.GetCurrentChallengeDescription()}");
                challengeSystem.GenerateRandomChallenge();
                if (GameManager.Instance != null)
                {
                    GameManager.Instance.AddScore(playerId, 50); // Bonus de ejemplo
                }
            }
        }
    }

    public ITree GetPlayerTree(int playerId)
    {
        if (playerId < 0 || playerId >= playerTrees.Length)
        {
            Debug.LogWarning("Jugador no encontrado");
            return null;
        }
        return playerTrees[playerId].tree;  // Devuelve el árbol del jugador
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




