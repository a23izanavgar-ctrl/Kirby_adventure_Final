using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KirbyHUD : MonoBehaviour
{
    [SerializeField] Image ouch_image;
    [SerializeField] float image_time = 0.5f;

    float timer = 0f;
    bool mostrar = false;

    void Start()
    {
        ouch_image.enabled = false;

        /** subscribirse al evento */
        Kirby.instance.OnDamageTaken += MostrarOuch;
    }

    void MostrarOuch()
    {
        mostrar = true;
        timer = image_time;
        ouch_image.enabled = true;
    }

    void Update()
    {
        if (mostrar)
        {
            timer -= Time.deltaTime; /** tiempo - deltaTime */

            if (timer <= 0)
            {
                mostrar = false;
                ouch_image.enabled = false;
            }
        }
    }
}

