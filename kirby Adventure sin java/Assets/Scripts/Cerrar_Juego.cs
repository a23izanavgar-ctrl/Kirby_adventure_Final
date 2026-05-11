using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cerrar_Juego : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("Cerrando el juego...");
            Application.Quit();
        }
    }
}
