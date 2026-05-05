using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class Kirby : MonoBehaviour
{
    // SINGLETON
    private static Kirby kirby;
    public static Kirby instance
    {
        get { return RequestKirby(); }
    }

    private static Kirby RequestKirby()
    {
        if (!kirby)
        {
            kirby = FindObjectOfType<Kirby>();
        }
        return kirby;
    }

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

    /** INPUT **/
    [SerializeField] InputActionAsset actions;

    InputAction move_action;
    InputAction jump_action;
    InputAction flotar_action;
    InputAction Absorber_action;

    float timeFalling = 0;

    /** MOVIMIENTO **/
    [SerializeField] float speed;
    [SerializeField] float jumpImpulse;
    [SerializeField] float flotarImpulse;
    [SerializeField] float maxFloatTime = 3f;
    [SerializeField] float floatTimer = 0f;

    Rigidbody2D rgb;
    Animator ator;

    [SerializeField]
    public int HP = 0;

    public event System.Action OnDamageTaken;
    public event System.Action OnDeadStart;
    public event System.Action OnGoalGoaled;
    public event System.Action OnByeBye;

    bool isAbsorbing = false;
    bool hasEnemyInside = false;

    [SerializeField]
    GameObject RangoAbsoreber;

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

    };

    KIRBY_STATES currentState;

    void Start()
    {
        actions.Enable();

        move_action = actions.FindActionMap("Movement").FindAction("Move");
        jump_action = actions.FindActionMap("Movement").FindAction("Jump");
        flotar_action = actions.FindActionMap("Movement").FindAction("Flotar");
        Absorber_action = actions.FindActionMap("Movement").FindAction("Ability");


        rgb = GetComponent<Rigidbody2D>();
        ator = GetComponent<Animator>();

        currentState = KIRBY_STATES.WALKING;
    }

    void Update()
    {
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

        if (rgb.velocity.y < -0.1f && currentState != KIRBY_STATES.FLOTAR)
        {
            ator.SetBool("IsGrounded", false);
        }


    }

    void UpdateFlotar_State()
    {
        if (flotar_action.IsPressed() && floatTimer < maxFloatTime)
        {
            floatTimer += Time.deltaTime;

            rgb.velocity = new Vector2(rgb.velocity.x, flotarImpulse);

            ator.SetFloat("SpeedYflotar", 1);
        }
        else
        {
            ator.SetFloat("SpeedYflotar", 0);

            currentState = KIRBY_STATES.FALLING;
            ator.SetTrigger("Dejaflotar");
        }
    }

    void UpdateWalking_state()
    {
        if (jump_action.WasPressedThisFrame())
        {
            currentState = KIRBY_STATES.JUMPING;
            rgb.AddForce(Vector2.up * jumpImpulse, ForceMode2D.Impulse);
            ator.SetTrigger("HasJumped");
            ator.SetBool("IsGrounded", false);
        }
        if (flotar_action.WasPressedThisFrame())
        {
            currentState = KIRBY_STATES.FLOTAR;
            ator.SetTrigger("HasFloated");
        }
        if (Absorber_action.WasPressedThisFrame() && !hasEnemyInside)
        {
            currentState = KIRBY_STATES.ABSORBER;
            isAbsorbing = true;

            ator.SetTrigger("Absorber"); 
        }
        if (hasEnemyInside && Absorber_action.WasPressedThisFrame())
        {
            ator.SetTrigger("ExplusaEstrella");

            hasEnemyInside = false;

            ator.SetBool("Gordo", false);
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

        floatTimer = 0f;
        ator.SetFloat("SpeedY", 0);
        currentState = KIRBY_STATES.WALKING;
        ator.SetBool("IsGrounded", true);
        timeFalling = 0;

    }

    void FixedUpdate()
    {
        Moverse();
    }

    public void Moverse()
    {
        float sign = move_action.ReadValue<float>();

        rgb.velocity = new Vector2(sign * speed, rgb.velocity.y);

        if (sign > 0)
            transform.localScale = new Vector3(1, 1, 1);
        else if (sign < 0)
            transform.localScale = new Vector3(-1, 1, 1);

        if (hasEnemyInside)
        {
            ator.SetFloat("SpeedGordoX", Mathf.Abs(sign));
            ator.SetFloat("SpeedX", 0); // 🔥 IMPORTANTE
        }
        else
        {
            ator.SetFloat("SpeedX", Mathf.Abs(sign));
            ator.SetFloat("SpeedGordoX", 0); // 🔥 IMPORTANTE
        }
    }

    void Update_Falling_State()
    {
        if (flotar_action.IsPressed())
        {
            currentState = KIRBY_STATES.FLOTAR;
            ator.SetTrigger("HasFloated");
            return;
        }

        // Velocidad vertical
        ator.SetFloat("SpeedY", rgb.velocity.y);

        timeFalling += Time.deltaTime;
        ator.SetFloat("TimeFalling", timeFalling);
    }

    public void GoalReached()
    {
        OnGoalGoaled?.Invoke();
    }

    public void ByeByeState()
    {
        OnByeBye?.Invoke();
    }

    public void TakeDamage(int amount)
    {
        HP -= amount;
        Debug.Log("HP restante: " + HP);

        OnDamageTaken?.Invoke();

        if (HP <= 0)
        {
            gameOverLoaded = true;
            Update_Muerto_state();
            SceneManager.LoadScene(map_gameover);
        }
    }

    void Update_Muerto_state()
    {
        OnDeadStart?.Invoke();
        currentState = KIRBY_STATES.MUERTO;
        Destroy(gameObject);
    }

    void Update_Absorber_State()
    {
        if (isAbsorbing)
        {
            RangoAbsoreber.SetActive(true);

            // ❗ NO repetir animación constantemente
            // solo mantener activa
        }

        // Si ya tiene enemigo dentro → salir
        if (hasEnemyInside)
        {
            RangoAbsoreber.SetActive(false);
            isAbsorbing = false;

            currentState = KIRBY_STATES.WALKING;

            ator.SetBool("Gordo", true); 
        }

        // Si suelta botón sin absorber nada
        if (!Absorber_action.IsPressed() && !hasEnemyInside)
        {
            RangoAbsoreber.SetActive(false);
            isAbsorbing = false;

            currentState = KIRBY_STATES.WALKING;
        }
    }
    public void OnEnemyAbsorbed(GameObject enemy)
    {
        Destroy(enemy);

        Debug.Log("Enemigo absorbido");

        hasEnemyInside = true;

        ator.SetBool("Gordo", true); 
    }
}