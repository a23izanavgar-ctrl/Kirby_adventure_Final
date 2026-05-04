using System;
using UnityEngine;
using UnityEngine.InputSystem;

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
    public int HP = 3;

    public event Action OnDamageTaken;
    public event Action OnDeadStart;
    public event Action OnGoalGoaled;
    public event Action OnByeBye;

    // =========================
    // ABSORCIÓN
    // =========================
    [SerializeField] GameObject RangoAbsoreber;
    bool haAbsorbido;

    // =========================
    // ESTRELLA
    // =========================
    [SerializeField] GameObject estrella;
    [SerializeField] float velocidadEstrella = 10f;

    // =========================
    // ESTADOS
    // =========================
    enum MovementState { Walking, Jumping, Falling, Floating }
    enum AbilityState { Idle, Absorbing, HasPower }

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

    void Update()
    {
        UpdateMovement();
        UpdateAbility();
        UpdateAnimator();
    }

    void FixedUpdate()
    {
        Moverse();
    }

    // =========================
    // MOVIMIENTO
    // =========================
    void UpdateMovement()
    {
        switch (moveState)
        {
            case MovementState.Walking:
                UpdateWalking_state();
                if (!isGrounded) moveState = MovementState.Falling;
                break;

            case MovementState.Jumping:
                if (rgb.velocity.y < 0)
                {
                    moveState = MovementState.Falling;
                    ator.SetTrigger("Voltereta");
                }
                break;

            case MovementState.Falling:
                if (isGrounded) moveState = MovementState.Walking;
                else if (flotar_action.IsPressed()) moveState = MovementState.Floating;
                timeFalling += Time.deltaTime;
                break;

            case MovementState.Floating:
                UpdateFlotar_State();
                break;
        }
    }

    void Moverse()
    {
        float dir = move_action.ReadValue<float>();
        rgb.velocity = new Vector2(dir * speed, rgb.velocity.y);

        if (dir != 0)
            transform.localScale = new Vector3(Mathf.Sign(dir), 1, 1);
    }

    void UpdateWalking_state()
    {
        if (jump_action.WasPressedThisFrame())
        {
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
        }
    }

    void UpdateFlotar_State()
    {
        isGrounded = false;

        if (flotar_action.IsPressed() && floatTimer < maxFloatTime)
        {
            floatTimer += Time.deltaTime;
            rgb.velocity = new Vector2(rgb.velocity.x, flotarImpulse);
        }
        else
        {
            ResetTriggers();
            ator.SetTrigger("Dejaflotar");
            moveState = MovementState.Falling;
        }
    }

    // =========================
    // ABILITIES
    // =========================
    void UpdateAbility()
    {
        switch (abilityState)
        {
            case AbilityState.Idle:
                if (Absorber_action.WasPressedThisFrame())
                    abilityState = AbilityState.Absorbing;
                break;

            case AbilityState.Absorbing:
                if (Absorber_action.IsPressed())
                    RangoAbsoreber.SetActive(true);
                else
                {
                    RangoAbsoreber.SetActive(false);
                    abilityState = AbilityState.Idle;
                }
                break;

            case AbilityState.HasPower:
                if (haAbsorbido && Absorber_action.WasPressedThisFrame())
                {
                    DispararEstrella();
                    haAbsorbido = false;
                    abilityState = AbilityState.Idle;
                }
                break;
        }
    }

    // =========================
    // VIDA
    // =========================
    public void TakeDamage(int amount)
    {
        HP -= amount;
        OnDamageTaken?.Invoke();

        if (HP <= 0)
        {
            OnDeadStart?.Invoke();
            Destroy(gameObject);
        }
    }

    public void GoalReached() => OnGoalGoaled?.Invoke();
    public void ByeByeState() => OnByeBye?.Invoke();

    // =========================
    // ESTRELLA
    // =========================
    void DispararEstrella()
    {
        Vector3 offset = new Vector3(transform.localScale.x, 0, 0);
        GameObject e = Instantiate(estrella, transform.position + offset, Quaternion.identity);

        float dir = transform.localScale.x;
        e.GetComponent<Rigidbody2D>().velocity = new Vector2(dir * velocidadEstrella, 0);
    }

    // =========================
    // COLLISION
    // =========================
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

    public void OnAbsorbSuccess()
    {
        abilityState = AbilityState.HasPower;
        haAbsorbido = true;
    }
    void UpdateAnimator()
    {
        ator.SetFloat("SpeedX", Mathf.Abs(move_action.ReadValue<float>()));
        ator.SetFloat("SpeedY", rgb.velocity.y);
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
    public bool IsAbsorbing()
    {
        return abilityState == AbilityState.Absorbing;
    }
}