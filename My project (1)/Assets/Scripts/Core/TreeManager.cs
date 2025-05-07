using UnityEngine;
using System;

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

    // Evento para actualización de árboles
    public event Action<int, ITree> OnTreeUpdated;

    [SerializeField] private TreeType initialTreeType = TreeType.BST;
    private PlayerTree[] playerTrees;
    private ChallengeSystem challengeSystem;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
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
            case TreeType.BTree: return new BTree(3); // Grado 3 para B-Tree
            default: return new BST();
        }
    }

    public void InsertValue(int playerId, int value)
    {
        if (playerId < 0 || playerId >= playerTrees.Length) return;

        playerTrees[playerId].tree.Insert(value);
        OnTreeUpdated?.Invoke(playerId, playerTrees[playerId].tree);

        // Verificar desafío
        if (challengeSystem != null)
        {
            bool completed = challengeSystem.CheckChallenge(playerId, playerTrees[playerId].tree);
            if (completed)
            {
                Debug.Log($"Player {playerId + 1} completed the challenge!");
                GameManager.Instance.AddScore(playerId, 50); // Bonus por reto completado
                challengeSystem.GenerateRandomChallenge();
            }
        }
    }

    public ITree GetPlayerTree(int playerId)
    {
        if (playerId < 0 || playerId >= playerTrees.Length)
        {
            return null;
        }
        return playerTrees[playerId].tree;
    }

    public TreeType GetPlayerTreeType(int playerId)
    {
        if (playerId < 0 || playerId >= playerTrees.Length)
        {
            return TreeType.BST;
        }
        return playerTrees[playerId].type;
    }

    public void SwitchTreeType(int playerId, TreeType newType)
    {
        if (playerId >= 0 && playerId < playerTrees.Length)
        {
            playerTrees[playerId].type = newType;
            playerTrees[playerId].tree = CreateTree(newType);
            OnTreeUpdated?.Invoke(playerId, playerTrees[playerId].tree);
        }
    }
}