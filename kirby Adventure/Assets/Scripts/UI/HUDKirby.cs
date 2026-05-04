using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HUDKirby : MonoBehaviour
{
    [SerializeField] Image ouch_image;
    [SerializeField] Image miss_image;
    [SerializeField] Image goal_image;
    [SerializeField] Image byebye_image;

    [SerializeField] Animator anim;

    [Header("HP")]
    [SerializeField] Image[] lifePoints;

    [Header("FADE")]
    [SerializeField] Image fade_image;
    [SerializeField] float fadeDuration = 0.25f;

    float normal_image_time = 1.5f;
    float super_image_time = 10f;

    float timer;
    bool showing;

    void Start()
    {
        anim.enabled = false;

        ouch_image.enabled = false;
        miss_image.enabled = false;
        goal_image.enabled = false;
        byebye_image.enabled = false;

        if (Kirby.instance != null)
        {
            Kirby.instance.OnDamageTaken += ShowOuch;
            Kirby.instance.OnDeadStart += ShowGameOver;
            Kirby.instance.OnGoalGoaled += ShowGoal;
            Kirby.instance.OnByeBye += ShowByeBye;
        }
    }

    void OnDestroy()
    {
        if (Kirby.instance != null)
        {
            Kirby.instance.OnDamageTaken -= ShowOuch;
            Kirby.instance.OnDeadStart -= ShowGameOver;
            Kirby.instance.OnGoalGoaled -= ShowGoal;
            Kirby.instance.OnByeBye -= ShowByeBye;
        }
    }

    void ShowOuch()
    {
        showing = true;
        timer = normal_image_time;

        ouch_image.enabled = true;
        anim.enabled = true;
    }

    void ShowGameOver()
    {
        showing = true;
        timer = super_image_time;

        miss_image.enabled = true;
        anim.enabled = true;
    }

    void ShowGoal()
    {
        showing = true;
        timer = super_image_time;

        goal_image.enabled = true;
    }

    void ShowByeBye()
    {
        showing = true;
        timer = normal_image_time;

        byebye_image.enabled = true;
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
                goal_image.enabled = false;
                byebye_image.enabled = false;
            }
        }

        UpdateHP();
    }

    void UpdateHP()
    {
        if (Kirby.instance == null) return;

        int hp = Mathf.Max(0, Kirby.instance.HP);

        for (int i = 0; i < lifePoints.Length; i++)
        {
            lifePoints[i].enabled = hp > i;
        }
    }

    // =========================
    // FADE
    // =========================
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