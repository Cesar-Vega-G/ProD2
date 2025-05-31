using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;  


[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(BoxCollider2D))]
public abstract class BasePlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] protected float velocidadMovimiento = 5f;
    [SerializeField] protected float fuerzaSalto = 5f;
    [SerializeField] protected float distanciaRaycastSuelo = 0.2f;
    [SerializeField] protected Vector2 offsetRaycastSuelo = new Vector2(0f, -0.1f);

    [Header("Player Settings")]
    [SerializeField] protected int playerId = 1;
    public int PlayerId => playerId;

    [SerializeField] protected float fuerzaGolpe = 10f;
    [SerializeField] protected float distanciaGolpe = 1.5f;
    [SerializeField] protected LayerMask capaJugador;

    protected Rigidbody2D cuerpoJugador;
    protected Animator animator;
    protected BoxCollider2D boxCollider;
    protected Vector2 direccionMovimiento;
    protected bool Grounded;
    protected bool empujadoTemporalmente = false;
    protected Vector3 initialPosition;


    protected abstract Key GetLeftKey();
    protected abstract Key GetRightKey();
    protected abstract Key GetJumpKey();
    protected abstract Key GetAttackKey();


    protected abstract string GetOpponentTag();
    protected abstract System.Type GetOpponentType();

    public bool EsInmune { get; set; } = false;
    protected bool fuerzaGolpeAumentada = false; 

    protected bool saltoEmergenciaDisponible = false;
    protected bool saltoEmergenciaUsado = false;



    protected virtual void Awake()
    {
        initialPosition = transform.position;
        cuerpoJugador = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider2D>();
    }

    protected virtual void Update()
    {
        if (transform.position.y < -15f)
            Respawn();

        CheckGrounded();
        HandleInput();
        HandleAnimation();
        BloquearSiHayObstaculoNoJugador();
        

        if (!Grounded && saltoEmergenciaDisponible && !saltoEmergenciaUsado && transform.position.y > 0)
        {
            Debug.Log($"Jugador {playerId} cumple condiciones para salto de emergencia");
            cuerpoJugador.linearVelocity = new Vector2(cuerpoJugador.linearVelocity.x, 0f);
            cuerpoJugador.AddForce(Vector2.up * fuerzaSalto * 2f, ForceMode2D.Impulse); // salto alto automático
            saltoEmergenciaUsado = true;
            saltoEmergenciaDisponible = false;
            Debug.Log($"Jugador {playerId} realizó salto automático por poder air jump.");
        }


    }

    protected virtual void FixedUpdate()
    {
        if (!empujadoTemporalmente)
        {
            if (PuedeMover(direccionMovimiento.x))
                cuerpoJugador.linearVelocity = new Vector2(direccionMovimiento.x * velocidadMovimiento, cuerpoJugador.linearVelocity.y);
            else
                cuerpoJugador.linearVelocity = new Vector2(0f, cuerpoJugador.linearVelocity.y);
        }
    }

    protected virtual void HandleInput()
    {
        direccionMovimiento = Vector2.zero;

        if (Keyboard.current != null)
        {
            if (Keyboard.current[GetLeftKey()].isPressed)
                direccionMovimiento.x = -1.3f;
            if (Keyboard.current[GetRightKey()].isPressed)
                direccionMovimiento.x = 1.3f;

            if (Keyboard.current[GetJumpKey()].wasPressedThisFrame)
            {
                if (Grounded)
                {
                    Jump();
                }
                if (saltoEmergenciaDisponible && !saltoEmergenciaUsado && transform.position.y < -1)
                {
                    cuerpoJugador.linearVelocity = new Vector2(cuerpoJugador.linearVelocity.x, 0f);
                    cuerpoJugador.AddForce(Vector2.up * fuerzaSalto * 2f, ForceMode2D.Impulse); // salto alto automático
                    saltoEmergenciaUsado = true;
                    saltoEmergenciaDisponible = false;
                    Debug.Log($"Jugador {playerId} realizó salto automático por poder air jump.");
                }

            }



            if (Keyboard.current[GetAttackKey()].wasPressedThisFrame)
            {
                animator.SetTrigger("Golpear");
                Golpear();
            }
        }
    }

    
    // Método para activar la fuerza de golpe aumentada
    public void ActivarFuerzaGolpeAumentada(float tiempo)
    {
        if (!fuerzaGolpeAumentada)
        {
            fuerzaGolpeAumentada = true;
            fuerzaGolpe *= 2;  // Aumentar la fuerza de golpe, por ejemplo, duplicándola
            Debug.Log($"Jugador {playerId} ahora tiene fuerza de golpe aumentada!");

            StartCoroutine(DesactivarFuerzaGolpeAumentada(tiempo));
        }
    }

    private IEnumerator DesactivarFuerzaGolpeAumentada(float segundos)
    {
        yield return new WaitForSeconds(segundos);
        fuerzaGolpeAumentada = false;
        fuerzaGolpe /= 2;  // Restaurar la fuerza de golpe
        Debug.Log($"Jugador {playerId} ya no tiene fuerza de golpe aumentada.");
    }

    protected virtual void Golpear()
    {
        Vector2 direccionGolpe = transform.localScale.x > 0 ? Vector2.right : Vector2.left;
        Vector2 origen = transform.position;

        RaycastHit2D hit = Physics2D.Raycast(origen, direccionGolpe, distanciaGolpe, capaJugador);

        if (hit.collider != null && hit.collider.CompareTag(GetOpponentTag()))
        {
            Rigidbody2D rbEnemigo = hit.collider.GetComponent<Rigidbody2D>();
            var controlador = hit.collider.GetComponent(GetOpponentType());

            if (rbEnemigo != null)
                rbEnemigo.AddForce(direccionGolpe * fuerzaGolpe, ForceMode2D.Impulse);

            if (controlador is BasePlayerController baseController && !baseController.EsInmune)
                baseController.DesactivarMovimientoTemporalmente();
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

    protected virtual void HandleAnimation()
    {
        if (direccionMovimiento.x < 0)
            transform.localScale = new Vector3(-1.3f, 1.3f, 1.3f);
        else if (direccionMovimiento.x > 0)
            transform.localScale = new Vector3(1.3f, 1.3f, 1.3f);

        animator.SetBool("Running", Mathf.Abs(cuerpoJugador.linearVelocity.x) > 0.1f);
    }

    protected virtual void Jump()
    {
        cuerpoJugador.AddForce(Vector2.up * fuerzaSalto, ForceMode2D.Impulse);
    }

    protected virtual void CheckGrounded()
    {
        Vector2 origenRaycast = (Vector2)transform.position + offsetRaycastSuelo;
        Grounded = Physics2D.Raycast(origenRaycast, Vector2.down, distanciaRaycastSuelo);
        Debug.DrawRay(origenRaycast, Vector2.down * distanciaRaycastSuelo, Color.red);

        //if (Grounded)
        //{
            //saltoEmergenciaDisponible = false;
            //saltoEmergenciaUsado = false;
        //}

    }

    protected virtual bool PuedeMover(float direccionX)
    {
        if (direccionX == 0)
            return true;

        Vector2 direccion = direccionX > 0 ? Vector2.right : Vector2.left;
        Vector2 origen = transform.position;
        float distanciaDeteccion = 0.15f;

        RaycastHit2D hit = Physics2D.Raycast(origen, direccion, distanciaDeteccion);

        if (hit.collider == null)
            return true;

        if (hit.collider.CompareTag("Player 1") || hit.collider.CompareTag("Player 2"))
            return true;

        return false;
    }

    protected virtual void BloquearSiHayObstaculoNoJugador()
    {
        if (!empujadoTemporalmente && !PuedeMover(direccionMovimiento.x))
        {
            cuerpoJugador.linearVelocity = new Vector2(0f, cuerpoJugador.linearVelocity.y);
        }
    }

    protected virtual void Respawn()
    {
        Debug.Log($"Player {playerId} cayó al vacío");
        transform.position = initialPosition;
        cuerpoJugador.linearVelocity = Vector2.zero;
        int otroJugadorId = (playerId == 0) ? 1 : 0;
        GameManager.Instance.AddScore(otroJugadorId, 30);
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Entró en Trigger con: " + collision.name);
        if (collision.CompareTag("Token"))
        {
            Token token = collision.GetComponent<Token>();
            if (token != null)
                token.Collect(playerId);
        }
    }

    public void ActivarSaltoEmergencia()
    {
        if (!saltoEmergenciaDisponible )
        {
            saltoEmergenciaDisponible = true;
            saltoEmergenciaUsado = false;
            Debug.Log($"Jugador {playerId} activó el salto de emergencia.");
        }
    }






}


