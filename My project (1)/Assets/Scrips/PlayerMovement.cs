using UnityEngine;
using UnityEngine.InputSystem;

public class MovimientoJugador : MonoBehaviour
{
    private Rigidbody2D cuerpoJugador;
    private Vector2 direccionMovimiento;
    private bool Grounded;
    private Animator animator;

    public float velocidadMovimiento = 5f; // Velocidad que tendrá el jugador
    public float fuerzaSalto = 5f;         // Fuerza con la que el jugador salta

    void Awake()
    {
        // Obtener el componente Rigidbody2D del jugador
        cuerpoJugador = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // Reiniciar la dirección de movimiento en cada frame
        direccionMovimiento = Vector2.zero;

        // Detectar si se presiona una tecla (solo si hay teclado conectado)
        if (Keyboard.current != null)
        {
            // Actualizar el estado de la animación según la velocidad en el eje X del Rigidbody2D
            animator.SetBool("Running", Mathf.Abs(cuerpoJugador.linearVelocity.x) > 0.1f);

            // Movimiento a la izquierda y derecha
            if (Keyboard.current.aKey.isPressed) direccionMovimiento.x = -1f; // Mover a la izquierda
            if (Keyboard.current.dKey.isPressed) direccionMovimiento.x = 1f;  // Mover a la derecha

            // Dibuja una línea hacia abajo para depuración
            Debug.DrawRay(transform.position, Vector3.down * 0.1f, Color.red);

            // Detectar si el jugador está en el suelo usando un raycast
            if (Physics2D.Raycast(transform.position, Vector2.down, 0.1f))
            {
                Grounded = true; // El jugador está en el suelo
            }
            else
            {
                Grounded = false; // El jugador no está en el suelo
            }

            // Detectar si el jugador presiona "W" para saltar, solo si está en el suelo
            if (Keyboard.current.wKey.wasPressedThisFrame && Grounded)
            {
                Jump();
            }
        }
    }

    private void Jump()
    {
        // Aplicar una fuerza al Rigidbody2D para saltar
        cuerpoJugador.AddForce(new Vector2(0f, fuerzaSalto), ForceMode2D.Impulse);
    }

    void FixedUpdate()
    {
        // Aplicar el movimiento al Rigidbody2D
        cuerpoJugador.linearVelocity = new Vector2(direccionMovimiento.x * velocidadMovimiento, cuerpoJugador.linearVelocity.y);
    }
}



