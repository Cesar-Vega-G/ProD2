using UnityEngine;
using UnityEngine.InputSystem;

public class MovimientoJugador : MonoBehaviour
{
    private Rigidbody2D cuerpoJugador;
    private Vector2 direccionMovimiento;
    private bool Grounded;
    private Animator animator;

    public float velocidadMovimiento = 5f;
    public float fuerzaSalto = 5f;

    // Parámetros para mejorar el Raycast de suelo
    public float distanciaRaycastSuelo = 0.2f;
    public Vector2 offsetRaycastSuelo = new Vector2(0f, -0.1f);

    // Parámetros para los Raycasts laterales
    public float distanciaRaycastLateral = 0.2f;
    public Vector2 offsetRaycastLateral = new Vector2(0f, 0f);

    private bool hayParedIzquierda;
    private bool hayParedDerecha;

    void Awake()
    {
        cuerpoJugador = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        direccionMovimiento = Vector2.zero;

        if (Keyboard.current != null)
        {
            // Lanzar rayos laterales para detectar paredes
            DetectarParedes();

            // Movimiento a la izquierda y derecha solo si no hay pared
            if (Keyboard.current.aKey.isPressed && !hayParedIzquierda)
                direccionMovimiento.x = -1.3f;
            if (Keyboard.current.dKey.isPressed && !hayParedDerecha)
                direccionMovimiento.x = 1.3f;

            // Flipping (girar el sprite)
            if (direccionMovimiento.x < 0)
                transform.localScale = new Vector3(-1.3f, 1.3f, 1.3f);
            else if (direccionMovimiento.x > 0)
                transform.localScale = new Vector3(1.3f, 1.3f, 1.3f);

            // Actualizar animación de correr
            animator.SetBool("Running", Mathf.Abs(cuerpoJugador.linearVelocity.x) > 0.1f);

            // DETECTAR GOLPE (Letra E)
            if (Keyboard.current.eKey.wasPressedThisFrame)
            {
                animator.SetTrigger("Golpear");
            }

            // Raycast para detectar suelo
            Vector2 origenRaycast = (Vector2)transform.position + offsetRaycastSuelo;
            RaycastHit2D hitSuelo = Physics2D.Raycast(origenRaycast, Vector2.down, distanciaRaycastSuelo);

            Grounded = hitSuelo.collider != null;

            Debug.DrawRay(origenRaycast, Vector2.down * distanciaRaycastSuelo, Color.red);

            // Salto
            if (Keyboard.current.wKey.wasPressedThisFrame && Grounded)
            {
                Jump();
            }
        }
    }

    private void Jump()
    {
        cuerpoJugador.AddForce(new Vector2(0f, fuerzaSalto), ForceMode2D.Impulse);
    }

    void FixedUpdate()
    {
        cuerpoJugador.linearVelocity = new Vector2(direccionMovimiento.x * velocidadMovimiento, cuerpoJugador.linearVelocity.y);
    }

    private void DetectarParedes()
    {
        Vector2 origenIzquierda = (Vector2)transform.position + offsetRaycastLateral;
        Vector2 origenDerecha = (Vector2)transform.position + offsetRaycastLateral;

        RaycastHit2D hitIzquierda = Physics2D.Raycast(origenIzquierda, Vector2.left, distanciaRaycastLateral);
        RaycastHit2D hitDerecha = Physics2D.Raycast(origenDerecha, Vector2.right, distanciaRaycastLateral);

        hayParedIzquierda = hitIzquierda.collider != null;
        hayParedDerecha = hitDerecha.collider != null;

        // Dibuja los rayos para depuración
        Debug.DrawRay(origenIzquierda, Vector2.left * distanciaRaycastLateral, Color.blue);
        Debug.DrawRay(origenDerecha, Vector2.right * distanciaRaycastLateral, Color.green);
    }
}




