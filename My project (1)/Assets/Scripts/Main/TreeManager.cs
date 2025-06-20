﻿using UnityEngine;

public class TreeManager : MonoBehaviour
{
    public static TreeManager Instance { get; private set; }

    [System.Serializable]
    public class PlayerTree
    {
        public object tree;
        public TreeType type;
        public GameObject treeGameObject; // Guarda referencia para eliminar
    }

    public enum TreeType { BST, AVL, BTree }

    [SerializeField] private TreeType initialTreeType = TreeType.BST;
    [SerializeField] private GameObject nodePrefab;
    [SerializeField] private GameObject linePrefab;
    private PlayerTree[] playerTrees;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
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
            Vector2 startPos = (i == 0) ? new Vector2(3f, 2.7f) : new Vector2(3f, 1f);

            playerTrees[i] = new PlayerTree
            {
                type = initialTreeType,
                treeGameObject = CreateTreeObject(initialTreeType, startPos)
            };

            playerTrees[i].tree = playerTrees[i].treeGameObject.GetComponent(initialTreeType.ToString());
        }
    }

    private GameObject CreateTreeObject(TreeType type, Vector2 rootPos)
    {
        GameObject go = new GameObject(type.ToString());
        go.transform.parent = this.transform;

        switch (type)
        {
            case TreeType.BST:
                {
                    BST bst = go.AddComponent<BST>();
                    bst.nodePrefab = nodePrefab;
                    bst.linePrefab = linePrefab;
                    bst.rootStartPosition = rootPos;
                    break;
                }
            case TreeType.AVL:
                {
                    AVL avl = go.AddComponent<AVL>();
                    avl.nodePrefab = nodePrefab;
                    avl.linePrefab = linePrefab;
                    avl.rootStartPosition = rootPos;
                    break;
                }
            case TreeType.BTree:
                {
                    BTree btree = go.AddComponent<BTree>();
                    btree.nodePrefab = nodePrefab;
                    btree.linePrefab = linePrefab;
                    btree.degree = 2;
                    btree.rootStartPosition = rootPos;
                    break;
                }
        }
        return go;
    }

    public void ResetTrees()
    {
        for (int i = 0; i < playerTrees.Length; i++)
        {
            if (playerTrees[i].treeGameObject != null)
            {
                Destroy(playerTrees[i].treeGameObject);
            }

            Vector2 startPos = (i == 0) ? new Vector2(2.5f, 2.7f) : new Vector2(2.5f, 1f);

            playerTrees[i].treeGameObject = CreateTreeObject(playerTrees[i].type, startPos);
            playerTrees[i].tree = playerTrees[i].treeGameObject.GetComponent(playerTrees[i].type.ToString());
        }
    }

    public void SwitchTreeType(int playerId, TreeType newType)
    {
        if (playerId < 0 || playerId >= playerTrees.Length) return;

        playerTrees[playerId].type = newType;
    }

    public void InsertValue(int playerId, int value)
    {
        if (playerId < 0 || playerId >= playerTrees.Length) return;

        if (playerTrees[playerId].tree is BST bst)
            bst.Insert(value);
        else if (playerTrees[playerId].tree is AVL avl)
            avl.Insert(value);
        else if (playerTrees[playerId].tree is BTree bTree)
            bTree.Insert(value);

        Debug.Log($"Player {playerId + 1} arbol: {GetTraversal(playerId)}");

        ChallengeSystem challengeSystem = FindObjectOfType<ChallengeSystem>();
        if (challengeSystem != null)
        {
            bool completed = challengeSystem.CheckChallenge(playerId, playerTrees[playerId].tree);
            if (completed)
            {
                Debug.Log($"Player {playerId + 1} completó: {challengeSystem.GetCurrentChallengeDescription()}");
                challengeSystem.GenerateRandomChallenge();
                GameManager.Instance.AddScore(playerId, 50);
                GameObject playerObject = GameObject.FindWithTag($"Player {playerId + 1}");
                if (playerObject != null)
                {
                    Poderes poderes = playerObject.GetComponent<Poderes>();
                    if (poderes != null)
                    {
                        int poderAleatorio = UnityEngine.Random.Range(0, 3);
                        poderes.SetPoderAsignado(poderAleatorio);
                        Debug.Log($"Poder {poderAleatorio} asignado a jugador {playerId + 1}");
                    }
                }
            }
        }
    }

    public string GetTraversal(int playerId)
    {
        if (playerId < 0 || playerId >= playerTrees.Length) return string.Empty;

        var tree = playerTrees[playerId].tree;
        if (tree is BST bst)
            return bst.Traversal();
        else if (tree is AVL avl)
            return avl.Traversal();
        else if (tree is BTree bTree)
            return bTree.Traversal();

        return string.Empty;
    }
}











