using TMPro;
using UnityEngine;

public class TreeNodeVisual : MonoBehaviour
{
    public TextMeshPro valueText;

    public void SetValue(int value)
    {
        valueText.text = value.ToString();
    }
}

