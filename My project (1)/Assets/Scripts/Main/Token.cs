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
    public void Initialize2(string tokenValueText)
    {
        if (valueText != null)
            valueText.text = tokenValueText;
    }
    // Método cuando es recolectado
    public void Collect(int collectorId)
    {
        if (TreeManager.Instance != null)
        {
            Debug.Log("Collect");
            TreeManager.Instance.InsertValue(collectorId, value);
            Destroy(gameObject); 
        }
        
    }
}
