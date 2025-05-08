using UnityEngine;
using TMPro;

public class Token : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI valueText;
    private int value;

    public void Initialize(int tokenValue)
    {
        value = tokenValue;
        if (valueText != null) valueText.text = value.ToString();
    }

    // Método modificado
    public void Collect(int collectorId)
    {
        if (TreeManager.Instance != null)
        {
            Debug.Log("Collect");
            TreeManager.Instance.InsertValue(collectorId, value);
            Destroy(gameObject);
            // ✅ También sumamos puntos al puntaje del jugador
            if (GameManager.Instance != null)
            {
                GameManager.Instance.AddScore(collectorId, value);  // O el valor que decidas
                
            }


            
        }
        
    }
}
