using UnityEngine;
using System;
using System.Linq;

public class ChallengeSystem : MonoBehaviour
{
    public enum ChallengeType
    {
        ReachDepth,
        BalanceTree,
        CompleteStructure,
        CollectValues
    }

    [Serializable]
    public class Challenge
    {
        public ChallengeType type;
        public int targetValue;
        public string description;
    }

    // Evento para actualización de retos
    public event Action OnChallengeUpdated;

    private Challenge currentChallenge;
    private float challengeStartTime;
    private float challengeDuration = 60f;

    private void Start()
    {
        GenerateRandomChallenge();
    }

    public void GenerateRandomChallenge()
    {
        currentChallenge = new Challenge
        {
            type = (ChallengeType)UnityEngine.Random.Range(0, Enum.GetValues(typeof(ChallengeType)).Length),
            targetValue = UnityEngine.Random.Range(3, 7)
        };

        currentChallenge.description = GetChallengeDescription(currentChallenge);
        challengeStartTime = Time.time;
        OnChallengeUpdated?.Invoke();
    }

    private string GetChallengeDescription(Challenge challenge)
    {
        return challenge.type switch
        {
            ChallengeType.ReachDepth => $"Build a tree with depth {challenge.targetValue}",
            ChallengeType.BalanceTree => "Keep the AVL tree balanced",
            ChallengeType.CompleteStructure => $"Build a complete B-Tree with {challenge.targetValue} levels",
            ChallengeType.CollectValues => $"Collect {challenge.targetValue} values in sequence",
            _ => "Unknown challenge"
        };
    }

    public Challenge GetCurrentChallenge() => currentChallenge;

    public float GetCurrentChallengeRemainingTime()
    {
        return Mathf.Max(0f, challengeDuration - (Time.time - challengeStartTime));
    }

    public string GetCurrentChallengeDescription()
    {
        return currentChallenge?.description ?? "No active challenge";
    }

    public bool CheckChallenge(int playerId, ITree tree)
    {
        if (currentChallenge == null) return false;

        return currentChallenge.type switch
        {
            ChallengeType.ReachDepth => tree.Depth() >= currentChallenge.targetValue,
            ChallengeType.BalanceTree => tree is AVL avlTree && avlTree.IsValid(),
            ChallengeType.CompleteStructure => tree is BTree bTree && bTree.IsComplete(),
            ChallengeType.CollectValues => tree.Traversal().Count >= currentChallenge.targetValue,
            _ => false
        };
    }
}