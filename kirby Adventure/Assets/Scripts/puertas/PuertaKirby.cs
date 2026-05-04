using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.HID;

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

    [SerializeField] bool autoTP = false;

    bool KirbyTrigger;
    bool entrando = false;

    HUDKirby hud;
    [SerializeField] float tiempo_transicion = 0.8f;

    private void Start()
    {
        actions.Enable();
        use_action = actions.FindActionMap("Movement").FindAction("Use");

        hud = FindObjectOfType<HUDKirby>();
    }

    private void Update()
    {
        if (!entrando && KirbyTrigger && (use_action.WasPressedThisFrame() || autoTP))
        {
            Debug.Log("Entrando...");
            GameObject kirbyObj = Kirby.instance.gameObject; /** gracias al singleton de kirby*/

            entrando = true;
            StartCoroutine(TransicionPuerta(kirbyObj));
        }
    }

    /** corrutina del fade */
    IEnumerator TransicionPuerta(GameObject kirbyObj)
    {
        /** fade out */
        yield return StartCoroutine(hud.FadeOut());

        /** teletransportar al kirby (el singleton) */
        Rigidbody2D rb = kirbyObj.GetComponent<Rigidbody2D>();
        rb.position = NextDoor.transform.position;

        EntraKirbyPorLaPuerta(kirbyObj); /** ajustar camara */

        yield return new WaitForSeconds(tiempo_transicion);

        /** fade in */
        yield return StartCoroutine(hud.FadeIn());
        entrando = false;
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