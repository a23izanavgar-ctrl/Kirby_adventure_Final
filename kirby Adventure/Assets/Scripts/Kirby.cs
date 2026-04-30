using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
        
    }

    void FixedUpdate()
    {
        Moverse();
    }

    public void Moverse()
    {
        float sign = move_action.ReadValue<float>();

        // Movimiento
        rgb.velocity = new Vector2(sign * speed, rgb.velocity.y);

        // Animación (CLAVE)
        ator.SetFloat("SpeedX", Mathf.Abs(sign));
        if (sign > 0)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
        else if (sign < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
    }

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
       

    }

    public void TakeDamage(int amount)
    {
        HP -= amount;
        Debug.Log("HP restante: " + HP);

        OnDamageTaken?.Invoke(); /** notificar al HUD */

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
        if (Absorber_action.IsPressed())
        {
            RangoAbsoreber.SetActive(true);
        }
        else
        {
            RangoAbsoreber.SetActive(false);
            currentState = KIRBY_STATES.WALKING;
        }
    }
}