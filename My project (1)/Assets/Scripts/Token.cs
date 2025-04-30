using UnityEngine;
using TMPro;

public class Token : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI valueText; // Para mostrar el número
    private int value;

    // Método llamado por TokenSpawner al crear el token
    public void Initialize(int tokenValue)
    {
        value = tokenValue;
        if (valueText != null)
            valueText.text = value.ToString(); // Muestra el valor
    }

    // Cuando un jugador toca el token
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                player.CollectToken(value); // Notifica al jugador
                Destroy(gameObject); // Elimina el token
            }
        }
    }
}
