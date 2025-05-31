using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public TextMeshProUGUI gameTimeText;
    public TextMeshProUGUI challengeTimeText;
    public TextMeshProUGUI challengeDescriptionText;
    public TextMeshProUGUI player1ScoreText;
    public TextMeshProUGUI player2ScoreText;
    private GameManager gameManager;
    private ChallengeSystem challengeSystem;

    private void Start()
    {
        gameManager = GameManager.Instance;
        challengeSystem = FindObjectOfType<ChallengeSystem>();

        if (gameManager == null)
            Debug.LogError("GameManager no encontrado");
        if (challengeSystem == null)
            Debug.LogError("ChallengeSystem no encontrado");
    }

    private void Update()
    {
        if (gameManager != null)
        {
            float remainingGameTime = gameManager.GetRemainingTime();
            gameTimeText.text = $"Tiempo Restante: {FormatTime(remainingGameTime)}";

            int p1Score = gameManager.GetPlayerScore(0);
            int p2Score = gameManager.GetPlayerScore(1);

            player1ScoreText.text = $"Jugador 1: {p1Score} pts";
            player2ScoreText.text = $"Jugador 2: {p2Score} pts";
        }

        if (challengeSystem != null)
        {
            float remainingChallengeTime = challengeSystem.GetRemainingChallengeTime();
            challengeTimeText.text = $"Cambio de desafío en: {FormatTime(remainingChallengeTime)}";

            string desc = challengeSystem.GetCurrentChallengeDescription();
            challengeDescriptionText.text = $"Desafío: {desc}";
        }
    }

    private string FormatTime(float time)
    {
        time = Mathf.Max(0, time);
        int minutes = Mathf.FloorToInt(time / 60);
        int seconds = Mathf.FloorToInt(time % 60);
        return $"{minutes:00}:{seconds:00}";
    }
}


