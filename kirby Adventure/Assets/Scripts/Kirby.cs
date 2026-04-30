using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class Kirby : MonoBehaviour
{
    // =========================
    // SINGLETON
    // =========================
    private static Kirby kirby;
    public static Kirby instance => kirby;

    void Awake()
    {
        if (kirby != null && kirby != this)
        {
            Destroy(gameObject);
            return;
        }

        kirby = this;
        DontDestroyOnLoad(gameObject);
    }

    // =========================
    // INPUT
    // =========================
    [SerializeField] InputActionAsset actions;

    InputAction move_action;
    InputAction jump_action;
    InputAction flotar_action;
    InputAction Absorber_action;

    // =========================
    // MOVIMIENTO
    // =========================
    [SerializeField] float speed;
    [SerializeField] float jumpImpulse;
    [SerializeField] float flotarImpulse;
    [SerializeField] float maxFloatTime = 3f;

    float floatTimer;
    float timeFalling;

    Rigidbody2D rgb;
    Animator ator;

    bool isGrounded;

    // =========================
    // VIDA
    // =========================
    public int HP = 0;
    public event Action OnDamageTaken;

<<<<<<< HEAD
    public event System.Action OnDamageTaken;
    public event System.Action OnDeadStart;
=======
    // =========================
    // ABSORCIÓN
    // =========================
    [SerializeField] GameObject RangoAbsoreber;
    bool haAbsorbido;
>>>>>>> Izan

    // =========================
    // ESTRELLA
    // =========================
    [SerializeField] GameObject estrella;
    [SerializeField] float velocidadEstrella = 10f;

<<<<<<< HEAD
    [SerializeField] string map_gameover;
    bool gameOverLoaded = false;


    enum KIRBY_STATES
    {
        WALKING,
        JUMPING,
        FALLING,
        FLOTAR,
        MUERTO,
        ABSORBER,
=======
    // =========================
    // ESTADOS
    // =========================
    enum MovementState { Walking, Jumping, Falling, Floating }
    enum AbilityState { Idle, Absorbing, HasPower }
>>>>>>> Izan

    MovementState moveState;
    AbilityState abilityState;

    // =========================
    // START
    // =========================
    void Start()
    {
        actions.Enable();

        move_action = actions.FindActionMap("Movement").FindAction("Move");
        jump_action = actions.FindActionMap("Movement").FindAction("Jump");
        flotar_action = actions.FindActionMap("Movement").FindAction("Flotar");
        Absorber_action = actions.FindActionMap("Movement").FindAction("Ability");

        rgb = GetComponent<Rigidbody2D>();
        ator = GetComponent<Animator>();

        moveState = MovementState.Walking;
        abilityState = AbilityState.Idle;
    }

    // =========================
    // UPDATE
    // =========================
    void Update()
    {
<<<<<<< HEAD
        switch (currentState)
        {
            case KIRBY_STATES.WALKING:
                UpdateWalking_state();
                break;

            case KIRBY_STATES.JUMPING:
                UpdateJumping_state();
                break;
            case KIRBY_STATES.FALLING:
                Update_Falling_State();
                break;

            case KIRBY_STATES.FLOTAR:
                UpdateFlotar_State();
                break;
            case KIRBY_STATES.MUERTO:
                Update_Muerto_state();
                break;
            case KIRBY_STATES.ABSORBER:
                Update_Absorber_State();
                break;



        }


    }

    void UpdateFlotar_State()
    {
        // Mantener flotación mientras se mantenga el botón
        if (flotar_action.IsPressed())
        {
            // OPCIÓN A: tipo Kirby (caída lenta)
            rgb.velocity = new Vector2(rgb.velocity.x, flotarImpulse);
            if (rgb.velocity.y > 0)
            {
                ator.SetFloat("SpeedYflotar", 1);
            }

            // OPCIÓN B: más tipo Flappy (impulsos)
            // rgb.AddForce(Vector2.up * flotarImpulse, ForceMode2D.Force); 
        }
        else
        {
            // Si suelta el botón, vuelve a caer
            currentState = KIRBY_STATES.FALLING;
            ator.SetTrigger("Dejaflotar");

        }
        if (flotar_action.IsPressed() && floatTimer < maxFloatTime)
        {
            floatTimer += Time.deltaTime;
            rgb.velocity = new Vector2(rgb.velocity.x, flotarImpulse);
        }
        else
        {
            currentState = KIRBY_STATES.FALLING;
        }
    }


    void UpdateWalking_state()
    {
        if (jump_action.WasPressedThisFrame())
        {
            currentState = KIRBY_STATES.JUMPING;
            rgb.AddForce(Vector2.up * jumpImpulse, ForceMode2D.Impulse);
            ator.SetTrigger("HasJumped");
        }
        if (flotar_action.WasPressedThisFrame())
        {
            currentState = KIRBY_STATES.FLOTAR;
            ator.SetTrigger("HasFloated");
        }
        if (Absorber_action.WasPressedThisFrame())
        {
            currentState = KIRBY_STATES.ABSORBER;
            // falta poner animacion
        }
    }

    void UpdateJumping_state()
    {
        if (rgb.velocity.y < 0)
        {
            ator.SetTrigger("Voltereta");
            currentState = KIRBY_STATES.FALLING;
        }


        //Mirar cuando la velocidad en Y empieze a ser negativa para dar la voltereta.
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        currentState = KIRBY_STATES.WALKING;
        ator.SetBool("IsGrounded", true);
        timeFalling = 0;

=======
        UpdateMovement();
        UpdateAbility();
        UpdateAnimator(); // ? CLAVE
>>>>>>> Izan
    }

    void FixedUpdate()
    {
        Moverse();
    }

    // ======================================================
    // MOVIMIENTO
    // ======================================================
    void UpdateMovement()
    {
        switch (moveState)
        {
            case MovementState.Walking:
                UpdateWalking_state();

                if (!isGrounded)
                    moveState = MovementState.Falling;
                break;

            case MovementState.Jumping:
                UpdateJumping_state();
                break;

            case MovementState.Falling:
                Update_Falling_State();

                if (isGrounded)
                    moveState = MovementState.Walking;
                break;

            case MovementState.Floating:
                UpdateFlotar_State();
                break;
        }
    }

<<<<<<< HEAD
    void Update_Falling_State()
    {

        if (flotar_action.IsPressed())
        {
            currentState = KIRBY_STATES.FLOTAR;
            return;
        }

        if (rgb.velocity.y < 0)
        {
            ator.SetFloat("SpeedY", -1);
        }

        timeFalling += Time.deltaTime;
        ator.SetFloat("TimeFalling", timeFalling);

=======
    void Moverse()
    {
        float dir = move_action.ReadValue<float>();
        rgb.velocity = new Vector2(dir * speed, rgb.velocity.y);
>>>>>>> Izan

        if (dir != 0)
            transform.localScale = new Vector3(Mathf.Sign(dir), 1, 1);
    }

    void UpdateWalking_state()
    {
        float dir = move_action.ReadValue<float>();

        if (jump_action.WasPressedThisFrame())
        {
<<<<<<< HEAD
            gameOverLoaded = true;
            Update_Muerto_state();
            SceneManager.LoadScene(map_gameover);
=======
            ResetTriggers();
            ator.SetTrigger("HasJumped");

            moveState = MovementState.Jumping;
            rgb.AddForce(Vector2.up * jumpImpulse, ForceMode2D.Impulse);
        }

        if (flotar_action.WasPressedThisFrame())
        {
            ResetTriggers();
            ator.SetTrigger("HasFloated");

            moveState = MovementState.Floating;
        }

        if (Absorber_action.WasPressedThisFrame())
        {
            abilityState = AbilityState.Absorbing;
>>>>>>> Izan
        }
    }

    void UpdateJumping_state()
    {
<<<<<<< HEAD
        OnDeadStart?.Invoke();
        currentState = KIRBY_STATES.MUERTO;
        Destroy(gameObject);
=======
        if (rgb.velocity.y < 0)
        {
            moveState = MovementState.Falling;
            ator.SetTrigger("Voltereta");
        }
    }

    void Update_Falling_State()
    {
        if (flotar_action.IsPressed())
        {
            moveState = MovementState.Floating;
            return;
        }

        timeFalling += Time.deltaTime;
    }

    void UpdateFlotar_State()
    {
        isGrounded = false;

        if (flotar_action.IsPressed() && floatTimer < maxFloatTime)
        {
            floatTimer += Time.deltaTime;
            rgb.velocity = new Vector2(rgb.velocity.x, flotarImpulse);

            ator.SetFloat("SpeedYflotar", 1);
        }
        else
        {
            ResetTriggers();
            ator.SetTrigger("Dejaflotar");

            moveState = MovementState.Falling;
        }
    }

    // ======================================================
    // ABILITIES
    // ======================================================
    void UpdateAbility()
    {
        switch (abilityState)
        {
            case AbilityState.Idle:
                if (Absorber_action.WasPressedThisFrame())
                    abilityState = AbilityState.Absorbing;
                break;

            case AbilityState.Absorbing:
                Update_Absorber_State();
                break;

            case AbilityState.HasPower:
                Update_Absorbido_State();
                break;
        }
>>>>>>> Izan
    }

    void Update_Absorber_State()
    {
        if (Absorber_action.IsPressed())
        {
            RangoAbsoreber.SetActive(true);
        }
        else
        {
            RangoAbsoreber.SetActive(false);
            abilityState = AbilityState.Idle;
        }
    }

    void Update_Absorbido_State()
    {
        if (!haAbsorbido) return;

        if (Absorber_action.WasPressedThisFrame())
        {
            DispararEstrella();
            haAbsorbido = false;
            abilityState = AbilityState.Idle;
        }
    }

    // ======================================================
    // ANIMATOR (?? CLAVE DEL SISTEMA)
    // ======================================================
    void UpdateAnimator()
    {
        float speedX = Mathf.Abs(move_action.ReadValue<float>());
        float speedY = rgb.velocity.y;

        ator.SetFloat("SpeedX", speedX);
        ator.SetFloat("SpeedY", speedY);
        ator.SetFloat("TimeFalling", timeFalling);

        ator.SetBool("IsGrounded", isGrounded);
    }

    void ResetTriggers()
    {
        ator.ResetTrigger("HasJumped");
        ator.ResetTrigger("HasFloated");
        ator.ResetTrigger("Dejaflotar");
        ator.ResetTrigger("Voltereta");
    }

    // ======================================================
    // VIDA
    // ======================================================
    public void TakeDamage(int amount)
    {
        HP -= amount;
        OnDamageTaken?.Invoke();

        if (HP <= 0)
            Destroy(gameObject);
    }

    // ======================================================
    // ESTRELLA
    // ======================================================
    void DispararEstrella()
    {
        Vector3 offset = new Vector3(transform.localScale.x * 1f, 0, 0);

        GameObject e = Instantiate(estrella, transform.position + offset, Quaternion.identity);

        float dir = transform.localScale.x;
        e.GetComponent<Rigidbody2D>().velocity = new Vector2(dir * velocidadEstrella, 0);
    }

    // ======================================================
    // COLLISION
    // ======================================================
    private void OnCollisionEnter2D(Collision2D collision)
    {
        isGrounded = true;
        floatTimer = 0;
        timeFalling = 0;

        moveState = MovementState.Walking;
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        isGrounded = false;
    }

    public bool IsAbsorbing()
    {
        return abilityState == AbilityState.Absorbing;
    }

    public void OnAbsorbSuccess()
    {
        abilityState = AbilityState.HasPower;
        haAbsorbido = true;
    }
}