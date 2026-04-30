using UnityEngine;
using UnityEngine.UI;

public class HUDKirby : MonoBehaviour
{
    [SerializeField] Image ouch_image;
    [SerializeField] Image miss_image;
    [SerializeField] float image_time = 1.5f;

    [SerializeField] Animator anim;

    [Header("HP")]
    [SerializeField] Image[] lifePoints;

    float timer;
    bool showing;

    void Start()
    {
        anim.enabled = false;
        ouch_image.enabled = false;
        miss_image.enabled = false;

        if (Kirby.instance != null)
        {
            Kirby.instance.OnDamageTaken += ShowOuch;
           
        }
    }

    void OnDestroy()
    {
        if (Kirby.instance != null)
        {
            Kirby.instance.OnDamageTaken -= ShowOuch;
            
        }
    }

    void ShowOuch()
    {
        showing = true;
        timer = image_time;

        ouch_image.enabled = true;
        anim.enabled = true;
    }

    void ShowGameOver()
    {
        showing = true;
        timer = 10f;

        miss_image.enabled = true;
        anim.enabled = true;
    }

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
}