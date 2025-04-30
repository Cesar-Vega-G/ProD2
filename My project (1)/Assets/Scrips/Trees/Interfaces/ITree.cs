public interface ITree
{
    void Insert(int value);
    int Depth();
    bool IsValid();
    string GetTreeType();
    int NodeCount { get; }
    string Traversal();
    bool CheckChallenge(ChallengeType challenge);
}

public enum ChallengeType
{
    DepthReached,
    CompleteStructure,
    BalancedTree,
    NodeCount
}
