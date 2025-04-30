using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Singleton pattern para acceso global
    public static GameManager Instance { get; private set; }

    [Header("Game Settings")]
    [SerializeField] private float gameDuration = 180f; // 3 minutos
    [SerializeField] private int playersCount = 2;

    private float remainingTime;
    private bool isGameActive;
    private int[] playerScores;

    private void Awake()
    {
        // Configurar singleton
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        InitializeGame();
    }

    private void InitializeGame()
    {
        remainingTime = gameDuration;
        isGameActive = true;
        playerScores = new int[playersCount];
        Debug.Log("Game Started!");
    }

    private void Update()
    {
        if (!isGameActive) return;

        remainingTime -= Time.deltaTime;
        if (remainingTime <= 0)
        {
            EndGame();
        }
    }

    public void AddScore(int playerId, int points)
    {
        if (playerId >= 0 && playerId < playersCount)
        {
            playerScores[playerId] += points;
            Debug.Log($"Player {playerId + 1} scored! Total: {playerScores[playerId]}");
        }
    }

    private void EndGame()
    {
        isGameActive = false;
        Debug.Log("Game Over!");

        // Determinar ganador
        int winner = 0;
        for (int i = 1; i < playersCount; i++)
        {
            if (playerScores[i] > playerScores[winner])
                winner = i;
        }

        Debug.Log($"Player {winner + 1} wins with {playerScores[winner]} points!");
    }

    // Métodos de ayuda
    public float GetRemainingTime() => remainingTime;
    public int GetPlayerScore(int playerId) => playerScores[playerId];
}