using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ChallengeUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TextMeshProUGUI challengeDescription;
    [SerializeField] private Image progressFill;
    [SerializeField] private Slider timeSlider;
    [SerializeField] private GameObject challengeCompletePrefab;

    [Header("Settings")]
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color completedColor = Color.green;

    private ChallengeSystem challengeSystem;

    private void Start()
    {
        challengeSystem = FindObjectOfType<ChallengeSystem>();
        if (challengeSystem != null)
        {
            challengeSystem.OnChallengeUpdated += UpdateChallengeDisplay;
            UpdateChallengeDisplay();
        }
    }

    private void Update()
    {
        if (challengeSystem == null) return;

        UpdateChallengeProgress();
        UpdateChallengeTimer();
    }

    private void UpdateChallengeDisplay()
    {
        challengeDescription.text = challengeSystem.GetCurrentChallengeDescription();
        progressFill.fillAmount = 0f;
        progressFill.color = normalColor;
    }

    private void UpdateChallengeProgress()
    {
        float maxProgress = 0f;

        for (int i = 0; i < GameManager.Instance.GetPlayersCount(); i++)
        {
            ITree tree = TreeManager.Instance.GetPlayerTree(i);
            if (challengeSystem.CheckChallenge(i, tree))
            {
                maxProgress = 1f;
                ShowChallengeCompleted(i);
                break;
            }
        }

        progressFill.fillAmount = maxProgress;
        progressFill.color = maxProgress >= 1f ? completedColor : normalColor;
    }

    private void UpdateChallengeTimer()
    {
        float remainingTime = challengeSystem.GetCurrentChallengeRemainingTime();
        timeSlider.value = remainingTime / challengeSystem.GetChallengeDuration();
    }

    private void ShowChallengeCompleted(int playerId)
    {
        GameObject completeUI = Instantiate(challengeCompletePrefab, transform);
        completeUI.GetComponentInChildren<TextMeshProUGUI>().text = $"Player {playerId + 1} completed the challenge!";
        Destroy(completeUI, 3f);
    }

    private void OnDestroy()
    {
        if (challengeSystem != null)
        {
            challengeSystem.OnChallengeUpdated -= UpdateChallengeDisplay;
        }
    }
}