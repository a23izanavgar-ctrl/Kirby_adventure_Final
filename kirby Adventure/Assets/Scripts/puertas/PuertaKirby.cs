using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PuertaKirby : MonoBehaviour
{
    [SerializeField] GameObject NextDoor; /** referencia al objeto (proxima puerta) */

    [SerializeField] float newMinX;
    [SerializeField] float newMaxX;
    [SerializeField] float newMinY;
    [SerializeField] float newMaxY;

    [SerializeField]
    InputActionAsset actions;

    InputAction use_action;

    bool KirbyTrigger;

    private void Start()
    {
        actions.Enable();
        use_action = actions.FindActionMap("Movement").FindAction("Use");
    }

    private void Update()
    {
        if (use_action.WasPressedThisFrame() && KirbyTrigger)
        {
            Debug.Log("Entrando...");
            GameObject kirbyObj = Kirby.instance.gameObject; /** gracias al singleton de kirby*/

            /** teletransportar al kirby (el singleton) */
            Rigidbody2D rb = kirbyObj.GetComponent<Rigidbody2D>();
            rb.position = NextDoor.transform.position;


            EntraKirbyPorLaPuerta(kirbyObj); /** ajustar camara */


        }
    }

    public void EntraKirbyPorLaPuerta(GameObject kirbyObj)
    {
        CameraTargetFollow cam = kirbyObj.GetComponentInChildren<CameraTargetFollow>();
        cam.maxX = newMaxX;
        cam.maxY = newMaxY;
        cam.minX = newMinX;
        cam.minY = newMinY;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            KirbyTrigger = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            KirbyTrigger = false;
        }
    }

}