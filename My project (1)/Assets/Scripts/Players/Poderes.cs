using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Poderes : MonoBehaviour
{
    private BasePlayerController jugador;

    private void Awake()
    {
        jugador = GetComponent<BasePlayerController>();
    }

    private void Update()
    {
        if (Keyboard.current != null)
        {
            if (jugador.PlayerId == 1 && Keyboard.current.zKey.wasPressedThisFrame)
            {
                ActivarInmunidad();
            }
            else if (jugador.PlayerId == 0 && Keyboard.current.vKey.wasPressedThisFrame)
            {
                ActivarInmunidad();
            }
            if (jugador.PlayerId == 1 && Keyboard.current.xKey.wasPressedThisFrame)
            {
                jugador.ActivarFuerzaGolpeAumentada(3f); // Activar la fuerza por 3 segundos
            }
            else if (jugador.PlayerId == 0 && Keyboard.current.bKey.wasPressedThisFrame)
            {
                jugador.ActivarFuerzaGolpeAumentada(3f); // Activar la fuerza por 3 segundos
            }

            if (jugador.PlayerId == 1 && Keyboard.current.cKey.wasPressedThisFrame)
            {
                jugador.ActivarSaltoEmergencia();
            }
            else if (jugador.PlayerId == 0 && Keyboard.current.nKey.wasPressedThisFrame)
            {
                jugador.ActivarSaltoEmergencia();
            }


        }
    }

    public void ActivarInmunidad()
    {
        if (!jugador.EsInmune)
        {
            jugador.EsInmune = true;
            Debug.Log($"Jugador {jugador.PlayerId} ahora es INMUNE por 3 segundos.");
            jugador.StartCoroutine(DesactivarInmunidadDespuesDeTiempo(3f));
        }
    }

    private IEnumerator DesactivarInmunidadDespuesDeTiempo(float segundos)
    {
        yield return new WaitForSeconds(segundos);
        jugador.EsInmune = false;
        Debug.Log($"Jugador {jugador.PlayerId} ya no es inmune.");
    }




   
}

