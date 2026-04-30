using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDKirby : MonoBehaviour
{
    [SerializeField] Image ouch_image;
    [SerializeField] float image_time = 1.5f;

    float timer = 0f;
    bool mostrar = false;

    // Start is called before the first frame update
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

    // Update is called once per frame
    void Update()
    {
        if (mostrar)
        {
            timer -= Time.deltaTime; /** tiempo - deltaTime */

            if (timer <= 0f)
            {
                mostrar=false;
                ouch_image.enabled = false;
            }
        }
    }
}
