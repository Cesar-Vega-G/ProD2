using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Poderes : MonoBehaviour
{
    private BasePlayerController jugador;

    [Header("Referencia a la UI del poder")]
    public PowerVisual powerVisual;  // Asignar en inspector

    private int poderAsignado = -1;  // -1 = ninguno, 0 = inmunidad, 1 = fuerza, 2 = salto

    private void Awake()
    {
        jugador = GetComponent<BasePlayerController>();
    }

    private void Update()
    {
        if (Keyboard.current != null)
        {
            if (jugador.PlayerId == 0 && Keyboard.current.qKey.wasPressedThisFrame)
            {
                ActivarPoder();
            }
            else if (jugador.PlayerId == 1 && Keyboard.current.uKey.wasPressedThisFrame)
            {
                ActivarPoder();
            }
        }
    }

    public void SetPoderAsignado(int poder)
    {
        poderAsignado = poder;
        Debug.Log($"SetPoderAsignado llamado con: {poder}");
        if (powerVisual != null)
            powerVisual.ShowPower(poderAsignado);
    }

    public void ActivarPoder()
    {
        switch (poderAsignado)
        {
            case 0:
                ActivarInmunidad();
                break;
            case 1:
                ActivarFuerzaGolpeAumentada(10f);
                break;
            case 2:
                ActivarSaltoEmergencia();
                break;
            default:
                Debug.Log("No hay poder asignado para activar.");
                break;
        }

        poderAsignado = -1;

        if (powerVisual != null)
            powerVisual.gameObject.SetActive(false); // Ocultar icono cuando se usa el poder
    }

    public void ActivarInmunidad()
    {
        if (!jugador.EsInmune)
        {
            jugador.EsInmune = true;
            Debug.Log($"Jugador {jugador.PlayerId} ahora es INMUNE por 3 segundos.");
            jugador.StartCoroutine(DesactivarInmunidadDespuesDeTiempo(10f));
        }
    }

    private IEnumerator DesactivarInmunidadDespuesDeTiempo(float segundos)
    {
        yield return new WaitForSeconds(segundos);
        jugador.EsInmune = false;
        Debug.Log($"Jugador {jugador.PlayerId} ya no es inmune.");
    }

    public void ActivarFuerzaGolpeAumentada(float duracion)
    {
        if (jugador != null)
            jugador.ActivarFuerzaGolpeAumentada(duracion);
    }

    public void ActivarSaltoEmergencia()
    {
        if (jugador != null)
            jugador.ActivarSaltoEmergencia();
    }
}



