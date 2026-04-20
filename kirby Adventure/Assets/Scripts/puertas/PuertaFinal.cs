using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PuertaFinal : MonoBehaviour
{
    [SerializeField] GameObject kirby;
    [SerializeField] GameObject camKirby; /** --> la virtual camera */
    [SerializeField] string mapa; /** --> mapa-niveles*/
    [SerializeField] Vector2 NextPosition;

    [SerializeField] float newMinX;
    [SerializeField] float newMaxX;
    [SerializeField] float newMinY;
    [SerializeField] float newMaxY;

    [SerializeField] InputActionAsset actions;
    InputAction use_action;

    bool KirbyTrigger = false;

    // Start is called before the first frame update
    void Start()
    {
        actions.Enable();
        use_action = actions.FindActionMap("Movement").FindAction("Use");

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (NextPosition != null)
            Kirby.instance.transform.position = NextPosition;
            camKirby.transform.position = NextPosition;

    }


    // Update is called once per frame
    void Update()
    {
        if (use_action.WasPressedThisFrame() && KirbyTrigger)
        {
            Debug.Log("Nivel cambiado");

            SceneManager.LoadScene(mapa); /** cambiar a la escena de niveles*/

            EntraKirbyPorLaPuerta(kirby); /** ajustar limites camara */
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
