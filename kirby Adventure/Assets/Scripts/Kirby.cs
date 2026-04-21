using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

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

    float timeFalling = 0;

    /** MOVIMIENTO **/
    [SerializeField] float speed;
    [SerializeField] float jumpImpulse;

    Rigidbody2D rgb;
    Animator ator;

    enum KIRBY_STATES
    {
        WALKING,
        JUMPING,
        FALLING
    };

    KIRBY_STATES currentState;

    void Start()
    {
        actions.Enable();

        move_action = actions.FindActionMap("Movement").FindAction("Move");
        jump_action = actions.FindActionMap("Movement").FindAction("Jump");

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
                
        }

        /*Debug.Log(rgb.velocity.y);*/
    }

    void UpdateWalking_state()
    {
        if (jump_action.WasPressedThisFrame())
        {
            currentState = KIRBY_STATES.JUMPING;
            rgb.AddForce(Vector2.up * jumpImpulse, ForceMode2D.Impulse);
            ator.SetTrigger("HasJumped");
           

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
        if(rgb.velocity.y < 0)
        {
            ator.SetFloat("SpeedY", -1);
        }

        

        timeFalling += Time.deltaTime;
            ator.SetFloat("TimeFalling", timeFalling);       

    }
}