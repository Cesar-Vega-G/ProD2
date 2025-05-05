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

    // Identificación del jugador
    [Header("Player Settings")]
    [SerializeField] private int playerId = 1;
    public int PlayerId => playerId;

    [SerializeField] private float fuerzaGolpe = 10f;  // Fuerza del golpe
    [SerializeField] private float distanciaGolpe = 1.5f;
    [SerializeField] private LayerMask capaJugador;

    // Componentes
    private Rigidbody2D cuerpoJugador;
    private Animator animator;
    private Vector2 direccionMovimiento;
    private bool Grounded;
    private bool empujadoTemporalmente = false;

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
            if (Keyboard.current.jKey.isPressed) // Movimiento a la izquierda
                direccionMovimiento.x = -1.3f;
            if (Keyboard.current.lKey.isPressed) // Movimiento a la derecha
                direccionMovimiento.x = 1.3f;

            if (Keyboard.current.iKey.wasPressedThisFrame && Grounded) // Salto
                Jump();

            if (Keyboard.current.oKey.wasPressedThisFrame) // Golpe
                Golpear();
        }
    }

    private void Golpear()
    {
        Vector2 direccionGolpe = transform.localScale.x > 0 ? Vector2.right : Vector2.left;
        Vector2 origen = transform.position;

        RaycastHit2D hit = Physics2D.Raycast(origen, direccionGolpe, distanciaGolpe, capaJugador);

        if (hit.collider != null && hit.collider.CompareTag("Player 1"))
        {
            animator.SetTrigger("Golpear"); // Animación de golpe
            Rigidbody2D rbEnemigo = hit.collider.GetComponent<Rigidbody2D>();
            PlayerController controlador = hit.collider.GetComponent<PlayerController>();

            if (rbEnemigo != null)
                rbEnemigo.AddForce(direccionGolpe * fuerzaGolpe, ForceMode2D.Impulse);  // Aplica la fuerza de retroceso al golpeado

            if (controlador != null)
                controlador.DesactivarMovimientoTemporalmente(); // Desactiva movimiento temporalmente para el golpeado
        }

        Debug.DrawRay(origen, direccionGolpe * distanciaGolpe, Color.yellow, 0.5f);
    }

    public void DesactivarMovimientoTemporalmente()
    {
        empujadoTemporalmente = true;
        Invoke(nameof(ReactivarMovimiento), 0.2f); // Tiempo de stun
    }

    private void ReactivarMovimiento()
    {
        empujadoTemporalmente = false;
    }

    private void HandleAnimation()
    {
        if (direccionMovimiento.x < 0)
            transform.localScale = new Vector3(-1.3f, 1.3f, 1.3f); // Mirada a la izquierda
        else if (direccionMovimiento.x > 0)
            transform.localScale = new Vector3(1.3f, 1.3f, 1.3f); // Mirada a la derecha

        animator.SetBool("Running", Mathf.Abs(cuerpoJugador.linearVelocity.x) > 0.1f); // Animación de correr
    }

    private void Jump()
    {
        cuerpoJugador.AddForce(Vector2.up * fuerzaSalto, ForceMode2D.Impulse); // Salto
    }

    private void CheckGrounded()
    {
        Vector2 origenRaycast = (Vector2)transform.position + offsetRaycastSuelo;
        Grounded = Physics2D.Raycast(origenRaycast, Vector2.down, distanciaRaycastSuelo);
        Debug.DrawRay(origenRaycast, Vector2.down * distanciaRaycastSuelo, Color.red);
    }

    void FixedUpdate()
    {
        if (!empujadoTemporalmente)
        {
            cuerpoJugador.linearVelocity = new Vector2(direccionMovimiento.x * velocidadMovimiento, cuerpoJugador.linearVelocity.y); // Movimiento del jugador
        }
    }
}

