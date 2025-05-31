using UnityEngine;
using UnityEngine.UI;

public class PowerVisual : MonoBehaviour
{
    public Image iconImage;      // Asignar en inspector
    public Sprite defaultSprite; // Sprite por defecto cuando no hay poder
    public Sprite inmunidadSprite;
    public Sprite fuerzaSprite;
    public Sprite saltoSprite;

    private void Start()
    {
        // Mostrar el sprite por defecto al inicio
        if (iconImage != null && defaultSprite != null)
            iconImage.sprite = defaultSprite;

        // Asegurarse que la imagen esté activa siempre
        gameObject.SetActive(true);
    }

    public void ShowPower(int powerIndex)
    {
        switch (powerIndex)
        {
            case 0:
                iconImage.sprite = inmunidadSprite;
                break;
            case 1:
                iconImage.sprite = fuerzaSprite;
                break;
            case 2:
                iconImage.sprite = saltoSprite;
                break;
            default:
                iconImage.sprite = defaultSprite;
                break;
        }

        // Siempre activo, no ocultar
        gameObject.SetActive(true);
    }
}

