using System;
using UnityEngine;

public class ChallengeSystem : MonoBehaviour
{
    public enum ChallengeType
    {
        ReachDepth,     // Ej: "Alcanza profundidad 5"
        BalanceTree,    // Solo para AVL
        CompleteStructure // Para B-Tree
    }

    [Serializable]
    public class Challenge
    {
        public ChallengeType type;
        public int targetValue; // Ej: profundidad requerida
        public string description;
    }

    private Challenge currentChallenge;

    private void Start()
    {
        GenerateRandomChallenge();
    }

    public void GenerateRandomChallenge()
    {
        // Lógica para generar un reto aleatorio
        currentChallenge = new Challenge
        {
            type = (ChallengeType)UnityEngine.Random.Range(0, Enum.GetValues(typeof(ChallengeType)).Length),
            targetValue = UnityEngine.Random.Range(3, 7) // Valores ajustables
        };

        currentChallenge.description = GetChallengeDescription(currentChallenge);
        Debug.Log($"New Challenge: {currentChallenge.description}");
    }

    private string GetChallengeDescription(Challenge challenge)
    {
        return challenge.type switch
        {
            ChallengeType.ReachDepth => $"Build a tree with depth {challenge.targetValue}",
            ChallengeType.BalanceTree => "Keep the AVL tree balanced",
            ChallengeType.CompleteStructure => $"Build a complete B-Tree with {challenge.targetValue} levels",
            _ => "Unknown challenge"
        };
    }
    public string GetCurrentChallengeDescription()
    {
        return currentChallenge != null ? currentChallenge.description : "No active challenge";
    }
    public bool CheckChallenge(int playerId, ITree tree)
    {
        if (currentChallenge == null) return false;

        return currentChallenge.type switch
        {
            ChallengeType.ReachDepth => tree.Depth() >= currentChallenge.targetValue,
            ChallengeType.BalanceTree => tree is AVL avlTree && avlTree.IsValid(),
            ChallengeType.CompleteStructure => tree is BTree bTree && bTree.IsPerfect(),
            _ => false
        };
    }
}