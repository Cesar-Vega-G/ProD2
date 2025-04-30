using UnityEngine;
using TMPro;

public class Token : MonoBehaviour
{
    public TextMeshPro numeroTexto;

    public void AsignarNumero(int numero)
    {
        if (numeroTexto != null)
        {
            numeroTexto.text = numero.ToString();
        }
    }
}
