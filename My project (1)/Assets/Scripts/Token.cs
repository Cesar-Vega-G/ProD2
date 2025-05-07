using UnityEngine;
using TMPro;

public class Token : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI valueText;
    [SerializeField] private GameObject collectEffect;
    
    private int value;

    public void Initialize(int tokenValue)
    {
        value = tokenValue;
        valueText.text = value.ToString();
    }

    public void Collect(int collectorId)
    {
        // Notificar TreeManager
        TreeManager.Instance.InsertValue(collectorId, value);
        
        // Notificar GameManager para puntaje
        GameManager.Instance.AddScore(collectorId, value);
        
        // Efecto visual
        if (collectEffect != null)
        {
            Instantiate(collectEffect, transform.position, Quaternion.identity);
        }
        
        Destroy(gameObject);
    }
}