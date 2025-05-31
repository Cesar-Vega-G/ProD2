using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameOverUI : MonoBehaviour
{
    public Image playerImage;          // Asigna en inspector
    public TextMeshProUGUI scoreText; // Asigna en inspector

    public Sprite[] playerSprites;    // Array de imágenes de jugadores

    void Start()
    {
        int winnerId = GameConstants.WinnerPlayerId;
        int winnerScore = GameConstants.WinnerScore;

        if (winnerId < 0 || winnerId >= playerSprites.Length)
        {
            Debug.LogError("Jugador ganador inválido o sin imagen asignada.");
            return;
        }

        // Mostrar imagen y puntaje
        playerImage.sprite = playerSprites[winnerId];
        scoreText.text = $"Ha ganado el jugador {winnerId}: {winnerScore}pts";
    }
}
