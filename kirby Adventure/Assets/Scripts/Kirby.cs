using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Animations;

public class Kirby : MonoBehaviour
{
    // LOGICA SINGLETON KIRBY //
    private static Kirby kirby;
    public static Kirby instance
    {
        get
        {
            return RequestKirby();
        }
    }

    private static Kirby RequestKirby()
    {
        if (!kirby)
        {
            kirby = FindObjectOfType<Kirby>();
        }
        return kirby;
    }

    /** - - input actions - -*/

    [SerializeField]
    InputActionAsset actions;


    InputAction jump_action;
    InputAction move_action;
    InputAction use_action;

    /*
    InputAction left;
    InputAction right;
    */

    // CODIGO MOVIMIENTO SM //

    [SerializeField]
    float speed;
    [SerializeField]
    float jumpImpulse;
    [SerializeField]
    float floatingImpulse;
    [SerializeField]

    float default_gravity; /** 0.5 en el rigidBody */
    [SerializeField]
    float floating_gravity;

    /*bool left, right;*/

    Rigidbody2D rgb;
    /** Animator ator; --> (animaciones) */


    enum KIRBY_STATES
    {
        WALKING, JUMPING, FLOATING, /** USE --> en el futuro seguramente */
    };

    KIRBY_STATES currentState;

    // Start is called before the first frame update
    void Start()
    {
        actions.Enable();
        
        jump_action = actions.FindActionMap("Movement").FindAction("Jump");

        move_action = actions.FindActionMap("Movement").FindAction("Move");

        use_action = actions.FindActionMap("Movement").FindAction("Use");

        /*
        left = actions.FindActionMap("Movement").FindAction("Left");

        right = actions.FindActionMap("Movement").FindAction("Right");
        */


        rgb = GetComponent<Rigidbody2D>();

        /** ator = GetComponent<Animator>();  ---> para las animaciones */

        currentState = KIRBY_STATES.WALKING;
    }

    // Update is called once per frame
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
            case KIRBY_STATES.FLOATING:
                UpdateFloating_state();
                break;
        }
    }

    void UpdateWalking_state()
    {
        /*if (Input.GetKey(KeyCode.Space))*/
        if (jump_action.IsPressed())
        {
            currentState = KIRBY_STATES.JUMPING;
            rgb.AddForce(new Vector2(0, jumpImpulse), ForceMode2D.Impulse);
            return;
        }

        Moverse();
    }

    void UpdateJumping_state()
    {
        Moverse(); /** podemos movernos al saltar */

        /** si estamos en el aire y volvemos a pulsar salto --> flotar */
        if (jump_action.WasPressedThisFrame()) /** ---> mejor que IsPressed porque se activa por frame */
        {
            rgb.gravityScale = floating_gravity; /** bajar la gravedad (flotar) */
            currentState = KIRBY_STATES.FLOATING;
        }
    }

    void UpdateFloating_state()
    {
        Moverse();

        /** saltar --> volar con impulsos */
        if (jump_action.WasPressedThisFrame())
        {
            rgb.AddForce(new Vector2(0, floatingImpulse), ForceMode2D.Impulse);
        }

        /** salir del estado floating */
        if (use_action.WasPressedThisFrame())
        {
            currentState = KIRBY_STATES.JUMPING;
            rgb.gravityScale = default_gravity; /** restaurar la gravedad antes */
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        currentState = KIRBY_STATES.WALKING;
        rgb.gravityScale = default_gravity;
    }

    public void Moverse()
    {
        /*
        right = Input.GetKey(KeyCode.D);
        left = Input.GetKey(KeyCode.A);

        float sign = (right ? 1 : 0) - (left ? 1 : 0);
        
        rgb.velocity = new Vector3(sign * speed, rgb.velocity.y, 0);
        */

        float sign = move_action.ReadValue<float>();

        /** ator.SetFloat("speedX", sign); (animar) */

        rgb.velocity = new Vector2(sign * speed, rgb.velocity.y);
    }

    void FixedUpdate()
    {
        Moverse();
    }

}
