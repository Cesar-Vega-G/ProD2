using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ScoreboardUI : MonoBehaviour
{
    [System.Serializable]
    public class PlayerDisplay
    {
        public TextMeshProUGUI nameText;
        public TextMeshProUGUI scoreText;
        public Image treeTypeIcon;
    }

    [Header("References")]
    [SerializeField] private PlayerDisplay[] playerDisplays;
    [SerializeField] private TextMeshProUGUI timerText;

    [Header("Settings")]
    [SerializeField] private Sprite[] treeTypeSprites; // 0=BST, 1=AVL, 2=BTree

    private void Start()
    {
        GameManager.Instance.OnScoreChanged += UpdateScores;
        UpdateScores();
    }

    public void UpdateScores()
    {
        for (int i = 0; i < playerDisplays.Length; i++)
        {
            if (i < GameManager.Instance.GetPlayersCount())
            {
                playerDisplays[i].nameText.text = $"Player {i + 1}";
                playerDisplays[i].scoreText.text = GameManager.Instance.GetPlayerScore(i).ToString();

                // Actualizar icono según tipo de árbol
                TreeManager.TreeType type = TreeManager.Instance.GetPlayerTreeType(i);
                playerDisplays[i].treeTypeIcon.sprite = treeTypeSprites[(int)type];
            }
        }
    }

    private void Update()
    {
        float remainingTime = GameManager.Instance.GetRemainingTime();
        int minutes = Mathf.FloorToInt(remainingTime / 60f);
        int seconds = Mathf.FloorToInt(remainingTime % 60f);
        timerText.text = $"{minutes:00}:{seconds:00}";
    }

    private void OnDestroy()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnScoreChanged -= UpdateScores;
        }
    }
}