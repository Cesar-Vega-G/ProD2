using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
public class PlayerController2 : MonoBehaviour
{
    // Configuración de movimiento
    [Header("Movement Settings")]
    [SerializeField] private float velocidadMovimiento = 5f;
    [SerializeField] private float fuerzaSalto = 5f;
    [SerializeField] private float distanciaRaycastSuelo = 0.2f;
    [SerializeField] private Vector2 offsetRaycastSuelo = new Vector2(0f, -0.1f);
    [SerializeField] private float distanciaRaycastLateral = 0.2f;
    [SerializeField] private Vector2 offsetRaycastLateral = new Vector2(0f, 0f);

    // Identificación del jugador
    [Header("Player Settings")]
    [SerializeField] private int playerId = 0; // 0 para P1, 1 para P2
    public int PlayerId => playerId;


    [SerializeField] private float fuerzaEmpuje = 150f;
    [SerializeField] private float distanciaGolpe = 51f;
    [SerializeField] private LayerMask capaJugador;

    // Componentes
    private Rigidbody2D cuerpoJugador;
    private Animator animator;
    private Vector2 direccionMovimiento;
    private bool Grounded;
    private bool hayParedIzquierda;
    private bool hayParedDerecha;

    void Awake()
    {
        cuerpoJugador = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        HandleInput();
        HandleAnimation();
        CheckGrounded();
    }

    private void HandleInput()
    {
        direccionMovimiento = Vector2.zero;

        if (Keyboard.current != null)
        {
            DetectarParedes();

            // Movimiento (usando Input System)
            if (Keyboard.current.jKey.isPressed && !hayParedIzquierda)
                direccionMovimiento.x = -1.3f;
            if (Keyboard.current.lKey.isPressed && !hayParedDerecha)
                direccionMovimiento.x = 1.3f;

            // Salto
            if (Keyboard.current.iKey.wasPressedThisFrame && Grounded)
            {
                Jump();
            }

            if (Keyboard.current.oKey.wasPressedThisFrame)
            {
                animator.SetTrigger("Golpear");
            }
        }
    }

    private void HandleAnimation()
    {
        // Flip sprite
        if (direccionMovimiento.x < 0)
            transform.localScale = new Vector3(-1.3f, 1.3f, 1.3f);
        else if (direccionMovimiento.x > 0)
            transform.localScale = new Vector3(1.3f, 1.3f, 1.3f);

        animator.SetBool("Running", Mathf.Abs(cuerpoJugador.linearVelocity.x) > 0.1f);
    }

    private void Jump()
    {
        cuerpoJugador.AddForce(Vector2.up * fuerzaSalto, ForceMode2D.Impulse);
    }

    private void CheckGrounded()
    {
        Vector2 origenRaycast = (Vector2)transform.position + offsetRaycastSuelo;
        Grounded = Physics2D.Raycast(origenRaycast, Vector2.down, distanciaRaycastSuelo);
        Debug.DrawRay(origenRaycast, Vector2.down * distanciaRaycastSuelo, Color.red);
    }

    private void DetectarParedes()
    {
        Vector2 origenIzquierda = (Vector2)transform.position + offsetRaycastLateral;
        Vector2 origenDerecha = (Vector2)transform.position + offsetRaycastLateral;

        RaycastHit2D hitIzquierda = Physics2D.Raycast(origenIzquierda, Vector2.left, distanciaRaycastLateral);
        RaycastHit2D hitDerecha = Physics2D.Raycast(origenDerecha, Vector2.right, distanciaRaycastLateral);

        // Detectar colisión con objetos que no son jugadores
        hayParedIzquierda = hitIzquierda.collider != null && !hitIzquierda.collider.CompareTag("Player 1") && !hitIzquierda.collider.CompareTag("Player 2");
        hayParedDerecha = hitDerecha.collider != null && !hitDerecha.collider.CompareTag("Player 1") && !hitDerecha.collider.CompareTag("Player 2");

        // Empujar al jugador que esté al lado izquierdo
        if (hitIzquierda.collider != null && (hitIzquierda.collider.CompareTag("Player 1") || hitIzquierda.collider.CompareTag("Player 2")))
        {
            Rigidbody2D rb = hitIzquierda.collider.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.AddForce(Vector2.left * -fuerzaEmpuje, ForceMode2D.Impulse);
            }
        }

        // Empujar al jugador que esté al lado derecho
        if (hitDerecha.collider != null && (hitDerecha.collider.CompareTag("Player 1") || hitDerecha.collider.CompareTag("Player 2")))
        {
            Rigidbody2D rb = hitDerecha.collider.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.AddForce(Vector2.right * fuerzaEmpuje, ForceMode2D.Impulse);
            }
        }

        Debug.DrawRay(origenIzquierda, Vector2.left * distanciaRaycastLateral, Color.blue);
        Debug.DrawRay(origenDerecha, Vector2.right * distanciaRaycastLateral, Color.green);
    }

    void FixedUpdate()
    {
        cuerpoJugador.linearVelocity = new Vector2(direccionMovimiento.x * velocidadMovimiento, cuerpoJugador.linearVelocity.y);
    }

    
}

