using System;
using UnityEngine;

public class ChallengeSystem : MonoBehaviour
{
    public enum ChallengeType
    {
        ReachDepth,
        BalanceTree,         // desafío para BST con profundidad >= 2
        CompleteStructure,   // Para BTree
        InsertValue,         // Para BST
        BuildBalancedAVL,    // Para AVL (balance y profundidad mínima)
        MinRootChildren      // Árbol B con al menos 3 hijos en la raíz
    }

    [Serializable]
    public class Challenge
    {
        public ChallengeType type;
        public int targetValue;
        public string description;
    }

    private Challenge currentChallenge;
    private float challengeTimeLimit = 40f; // 40 segundos por desafío
    private float challengeTimer;

    public float GetRemainingChallengeTime()
    {
        return challengeTimer;
    }

    private void Start()
    {
        GenerateRandomChallenge();
    }

    private void Update()
    {
        if (currentChallenge == null) return;

        challengeTimer -= Time.deltaTime;

        if (challengeTimer <= 0)
        {
            Debug.Log("Tiempo agotado para el desafío. Generando uno nuevo.");
            OnChallengeTimeout();
        }
    }

    public void GenerateRandomChallenge()
    {
        Debug.Log("Generando nuevo desafío...");
        var types = Enum.GetValues(typeof(ChallengeType));
        currentChallenge = new Challenge();

        currentChallenge.type = (ChallengeType)UnityEngine.Random.Range(0, types.Length);
        TreeManager.TreeType requiredTreeType = GetTreeTypeForChallenge(currentChallenge.type);

        int playersCount = GameManager.Instance.GetPlayersCount();
        for (int playerId = 0; playerId < playersCount; playerId++)
        {
            TreeManager.Instance.SwitchTreeType(playerId, requiredTreeType);
        }

        TreeManager.Instance.ResetTrees();

        switch (currentChallenge.type)
        {
            case ChallengeType.ReachDepth:
                currentChallenge.targetValue = UnityEngine.Random.Range(3, 5);
                currentChallenge.description = $"Construye un Árbol de Profundidad: {currentChallenge.targetValue}";
                break;

            case ChallengeType.InsertValue:
                currentChallenge.targetValue = UnityEngine.Random.Range(1, 100);
                currentChallenge.description = $"Inserta el Valor: {currentChallenge.targetValue} ";
                break;

            case ChallengeType.BalanceTree:
                currentChallenge.targetValue = 3; // profundidad mínima 3
                currentChallenge.description = $"Mantén un BST balanceado con profundidad mínima {currentChallenge.targetValue}";
                break;

            case ChallengeType.BuildBalancedAVL:
                currentChallenge.targetValue = UnityEngine.Random.Range(3, 5);
                currentChallenge.description = $"  Construye un AVL de Profundidad: {currentChallenge.targetValue}";
                break;

            case ChallengeType.CompleteStructure:
                currentChallenge.targetValue = UnityEngine.Random.Range(3, 5);
                currentChallenge.description = $"Construye un Árbol B completo de {currentChallenge.targetValue} niveles";
                break;

            case ChallengeType.MinRootChildren:
                currentChallenge.description = "Construye un Árbol B con al menos 3 hijos en la raíz";
                break;
        }

        challengeTimer = challengeTimeLimit;

        Debug.Log($"Nuevo Desafío: {currentChallenge.description} con árbol tipo {requiredTreeType}");
    }

    private TreeManager.TreeType GetTreeTypeForChallenge(ChallengeType challengeType)
    {
        switch (challengeType)
        {
        
            case ChallengeType.ReachDepth:
            case ChallengeType.BalanceTree:       // BalanceTree es para BST
                return TreeManager.TreeType.BST;

            case ChallengeType.InsertValue:
            case ChallengeType.BuildBalancedAVL:
                return TreeManager.TreeType.AVL;

            case ChallengeType.CompleteStructure:
            case ChallengeType.MinRootChildren:   // MinRootChildren es para BTree
                return TreeManager.TreeType.BTree;

            default:
                return TreeManager.TreeType.BST; // Default a BST
        }
    }

    public string GetCurrentChallengeDescription()
    {
        return currentChallenge != null ? currentChallenge.description : "Sin desafío";
    }

    public bool CheckChallenge(int playerId, object tree)
    {
        if (currentChallenge == null) return false;

        switch (currentChallenge.type)
        {
            case ChallengeType.ReachDepth:
                return CheckReachDepth(tree);

            case ChallengeType.InsertValue:
                if (tree is BST bst)
                    return bst.Contains(currentChallenge.targetValue);
                return false;

            case ChallengeType.BalanceTree:
                if (tree is BST bst2)
                    return IsBalancedBST(bst2) && bst2.Depth() >= currentChallenge.targetValue;
                return false;

            case ChallengeType.BuildBalancedAVL:
                if (tree is AVL avl)
                    return avl.IsValid() && avl.Depth() >= currentChallenge.targetValue;
                return false;

            case ChallengeType.CompleteStructure:
                if (tree is BTree btree)
                    return btree.IsComplete();
                return false;

            case ChallengeType.MinRootChildren:
                if (tree is BTree btree2)
                    return btree2.RootHasMinChildren(3);
                return false;

            default:
                return false;
        }
    }

    private bool CheckReachDepth(object tree)
    {
        if (tree is BST bst)
            return bst.Depth() >= currentChallenge.targetValue;

        if (tree is AVL avl)
            return avl.Depth() >= currentChallenge.targetValue;

        if (tree is BTree bTree)
            return bTree.Depth() >= currentChallenge.targetValue;

        return false;
    }

    private bool IsBalancedBST(BST bst)
    {
        return bst.IsBalanced();
    }

    private void OnChallengeTimeout()
    {
        GenerateRandomChallenge();
    }
}

