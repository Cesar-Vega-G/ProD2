using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public TextMeshProUGUI gameTimeText;
    public TextMeshProUGUI challengeTimeText;
    public TextMeshProUGUI challengeDescriptionText;

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


