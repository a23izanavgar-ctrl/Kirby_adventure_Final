using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PuertaFinal : MonoBehaviour
{
    [SerializeField] string debug;
    [SerializeField] string mapa; /** --> nombre del mapa*/
    [SerializeField] Vector2 NextPosition;

    [SerializeField] float newMinX;
    [SerializeField] float newMaxX;
    [SerializeField] float newMinY;
    [SerializeField] float newMaxY;

    [SerializeField] InputActionAsset actions;
    InputAction use_action;

    bool KirbyTrigger = false;
    public static PuertaFinal lastUsedDoor;

    // Start is called before the first frame update
    void Start()
    {
        actions.Enable();
        use_action = actions.FindActionMap("Movement").FindAction("Use");

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (lastUsedDoor != this) return;

        Kirby.instance.transform.position = NextPosition;

        /** ajustar limites camara despues de cargar la escena */
        CameraTargetFollow cam = Kirby.instance.GetComponentInChildren<CameraTargetFollow>();
        cam.maxX = newMaxX;
        cam.maxY = newMaxY;
        cam.minX = newMinX;
        cam.minY = newMinY;
    }


    // Update is called once per frame
    void Update()
    {
        if (use_action.WasPressedThisFrame() && KirbyTrigger)
        {
            Debug.Log(debug);

            lastUsedDoor = this;
            SceneManager.LoadScene(mapa); /** cambiar a la escena de niveles*/

        }
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
