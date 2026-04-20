using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PuertaFinal : MonoBehaviour
{
    [SerializeField] GameObject kirby;
    [SerializeField] InputActionAsset actions;

    InputAction use_action;

    bool KirbyTrigger = false;

    // Start is called before the first frame update
    void Start()
    {
        actions.Enable();
        use_action = actions.FindActionMap("Movement").FindAction("Use");
    }

    // Update is called once per frame
    void Update()
    {
        if (use_action.WasPressedThisFrame() && KirbyTrigger)
        {
            Debug.Log("Final del nivel");

        }

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            KirbyTrigger = true;
        }
    }

}
