using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class HUDKirby : MonoBehaviour
{
    [SerializeField] Image ouch_image;
    [SerializeField] Image miss_image;
    [SerializeField] float image_time = 1.5f;

    [SerializeField] Animator animate;

    /** ---[HP]----------------------------------- */

    [Header("HP - HUD")]
    [SerializeField] Image lifePoint_1;
    [SerializeField] Image lifePoint_2;
    [SerializeField] Image lifePoint_3;
    [SerializeField] Image lifePoint_4;
    [SerializeField] Image lifePoint_5;
    [SerializeField] Image lifePoint_6;

    [SerializeField] Image[] lifePoints; /** array de lifePoints*/

    float timer = 0f;
    bool mostrar = false;

    // Start is called before the first frame update
    void Start()
    {
        animate.enabled = false;

        ouch_image.enabled = false;
        miss_image.enabled = false;

        /** subscribirse al evento */
        Kirby.instance.OnDamageTaken += MostrarOuch;
        Kirby.instance.OnDeadStart += MostrarGameOver;
    }

    void MostrarOuch()
    {
        mostrar = true;
        timer = image_time;
        ouch_image.enabled = true;
        animate.enabled = true;
    }

    void MostrarGameOver()
    {
        mostrar = true;
        timer = 10f;
        miss_image.enabled = true;
        animate.enabled = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (mostrar)
        {
            timer -= Time.deltaTime; /** tiempo - deltaTime */

            if (timer <= 0f)
            {
                animate.enabled = false;
                mostrar = false;
                ouch_image.enabled = false;
                miss_image.enabled = false;
            }
        }

        UpdateKirbyLifeState();
    }

    void UpdateKirbyLifeState()
    {
        for (int i = 0; i < lifePoints.Length; i++)
        {
            //i-> 0 lifePoints[0] (el primer punto de vida) tiene enabled = Kirby.instance.HP > 0;
            //Es decir, si tiene 0 de vida, está desactivado porque  Kirby.instance.HP > 0--> False , en caso contrario 
            lifePoints[i].enabled = Kirby.instance.HP > i;
        }
    }
}
