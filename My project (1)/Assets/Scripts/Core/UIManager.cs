using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("References")]
    [SerializeField] private ScoreboardUI scoreboard;
    [SerializeField] private ChallengeUI challengeUI;

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

    public void UpdateAllUI()
    {
        scoreboard.UpdateScores();
        challengeUI.UpdateChallengeDisplay();
    }
}