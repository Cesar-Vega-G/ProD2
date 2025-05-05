using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
public class PlayerController : MonoBehaviour
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

    [SerializeField] private float fuerzaGolpe = 10f;
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
            if (Keyboard.current.aKey.isPressed)
                direccionMovimiento.x = -1.3f;
            if (Keyboard.current.dKey.isPressed)
                direccionMovimiento.x = 1.3f;

            if (Keyboard.current.wKey.wasPressedThisFrame && Grounded)
                Jump();

            if (Keyboard.current.eKey.wasPressedThisFrame)
            {
                animator.SetTrigger("Golpear");
                Golpear();
            }
        }
    }

    private void Golpear()
    {
        Vector2 direccionGolpe = transform.localScale.x > 0 ? Vector2.right : Vector2.left;
        Vector2 origen = transform.position;

        RaycastHit2D hit = Physics2D.Raycast(origen, direccionGolpe, distanciaGolpe, capaJugador);

        if (hit.collider != null && hit.collider.CompareTag("Player 2"))
        {
            Rigidbody2D rbEnemigo = hit.collider.GetComponent<Rigidbody2D>();
            PlayerController2 controlador = hit.collider.GetComponent<PlayerController2>();

            if (rbEnemigo != null)
                rbEnemigo.AddForce(direccionGolpe * fuerzaGolpe, ForceMode2D.Impulse);

            if (controlador != null)
                controlador.DesactivarMovimientoTemporalmente();
        }

        Debug.DrawRay(origen, direccionGolpe * distanciaGolpe, Color.yellow, 0.5f);
    }

    public void DesactivarMovimientoTemporalmente()
    {
        empujadoTemporalmente = true;
        Invoke(nameof(ReactivarMovimiento), 0.2f);
    }

    private void ReactivarMovimiento()
    {
        empujadoTemporalmente = false;
    }

    private void HandleAnimation()
    {
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

    void FixedUpdate()
    {
        if (!empujadoTemporalmente)
        {
            if (PuedeMover(direccionMovimiento.x))
            {
                cuerpoJugador.linearVelocity = new Vector2(direccionMovimiento.x * velocidadMovimiento, cuerpoJugador.linearVelocity.y);
            }
            else
            {
                cuerpoJugador.linearVelocity = new Vector2(0f, cuerpoJugador.linearVelocity.y);
            }
        }
    }

    private bool PuedeMover(float direccionX)
    {
        if (direccionX == 0)
            return true;

        Vector2 direccion = direccionX > 0 ? Vector2.right : Vector2.left;
        Vector2 origen = transform.position;
        float distanciaDeteccion = 0.15f;

        RaycastHit2D hit = Physics2D.Raycast(origen, direccion, distanciaDeteccion);

        if (hit.collider == null)
            return true;

        // Solo permite avanzar si lo que hay delante es otro jugador
        if (hit.collider.CompareTag("Player 1") || hit.collider.CompareTag("Player 2"))
            return true;

        return false;
    }
}

