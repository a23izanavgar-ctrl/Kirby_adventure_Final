using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HUDKirby : MonoBehaviour
{
    [SerializeField] Image ouch_image;
    [SerializeField] Image miss_image;
    [SerializeField] Image goal_image;
    [SerializeField] Image byebye_image;

    [SerializeField] Animator anim;

<<<<<<< HEAD

    /** ---[HP]----------------------------------- */

    [Header("HP - HUD")]
    [SerializeField] Image[] lifePoints; /** array de lifePoints*/

    /** -- fadein-fadeout ------------------------ */

    [Header("FADE")]
    [SerializeField] Image fade_image;
    [SerializeField] float fadeDuration = 0.25f;

    /** ------------------------------------------ */

    float normal_image_time = 1.5f;
    float super_image_time = 10.0f;

    float timer = 0f;
    bool mostrar = false;

    // Start is called before the first frame update
=======
    [Header("HP")]
    [SerializeField] Image[] lifePoints;

    float timer;
    bool showing;

>>>>>>> 6365268f94123e9d0fe2793a401b6814e65b6591
    void Start()
    {
        anim.enabled = false;
        ouch_image.enabled = false;
        miss_image.enabled = false;
        goal_image.enabled = false;
        byebye_image.enabled = false;

<<<<<<< HEAD
        /** subscribirse al evento */
        Kirby.instance.OnDamageTaken += MostrarOuch;
        Kirby.instance.OnDeadStart += MostrarGameOver;
        Kirby.instance.OnGoalGoaled += MostrarGoal;
        Kirby.instance.OnByeBye += MostrarByeBye;
=======
        if (Kirby.instance != null)
        {
            Kirby.instance.OnDamageTaken += ShowOuch;
           
        }
>>>>>>> 6365268f94123e9d0fe2793a401b6814e65b6591
    }

    void OnDestroy()
    {
<<<<<<< HEAD
        mostrar = true;
        timer = normal_image_time;
=======
        if (Kirby.instance != null)
        {
            Kirby.instance.OnDamageTaken -= ShowOuch;
            
        }
    }

    void ShowOuch()
    {
        showing = true;
        timer = image_time;

>>>>>>> 6365268f94123e9d0fe2793a401b6814e65b6591
        ouch_image.enabled = true;
        anim.enabled = true;
    }

    void ShowGameOver()
    {
<<<<<<< HEAD
        mostrar = true;
        timer = super_image_time;
=======
        showing = true;
        timer = 10f;

>>>>>>> 6365268f94123e9d0fe2793a401b6814e65b6591
        miss_image.enabled = true;
        anim.enabled = true;
    }

<<<<<<< HEAD
    void MostrarGoal()
    {
        mostrar = true;
        timer = super_image_time;
        goal_image.enabled = true;
    }

    void MostrarByeBye()
    {
        mostrar = true;
        timer = normal_image_time;
        byebye_image.enabled = true;
    }

    // Update is called once per frame
=======
>>>>>>> 6365268f94123e9d0fe2793a401b6814e65b6591
    void Update()
    {
        if (showing)
        {
            timer -= Time.deltaTime;

            if (timer <= 0)
            {
                showing = false;
                anim.enabled = false;

                ouch_image.enabled = false;
                miss_image.enabled = false;
                goal_image.enabled = false;
                byebye_image.enabled = false;
            }
        }

        UpdateHP();
    }

    void UpdateHP()
    {
        int hp = Mathf.Max(0, Kirby.instance.HP);

        for (int i = 0; i < lifePoints.Length; i++)
        {
            lifePoints[i].enabled = hp > i;
        }
    }
<<<<<<< HEAD

    /** CORRUTINAS DE FADE */

    /**FADE OUT */
    public IEnumerator FadeOut()
    {
        float time = 0f;
        Color color = fade_image.color;

        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            color.a = time / fadeDuration;
            fade_image.color = color;
            yield return null;
        }
    }

    /**FADE IN */
    public IEnumerator FadeIn()
    {
        float time = 0f;
        Color color = fade_image.color;

        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            color.a = 1 - (time / fadeDuration);
            fade_image.color = color;
            yield return null;
        }
    }
}
=======
}
>>>>>>> 6365268f94123e9d0fe2793a401b6814e65b6591
