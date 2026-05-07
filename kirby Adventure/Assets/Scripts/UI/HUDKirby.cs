using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class HUDKirby : MonoBehaviour
{
    [SerializeField] Image ouch_image;
    [SerializeField] Image miss_image;
    [SerializeField] Image goal_image;
    [SerializeField] Image byebye_image;

    [SerializeField] Animator animate;

    /** ---[SCORE]----------------------------------- */

    [Header("SCORE")]
    [SerializeField] TMP_Text scoreText;

    public int score = 0;

    /** ---[HP]----------------------------------- */

    [Header("HP - HUD")]
    [SerializeField] Image[] lifePoints; /** array de lifePoints*/

    /** -- fadein-fadeout ------------------------ */

    [Header("FADE")]
    [SerializeField] Image fade_image;
    [SerializeField] float fadeDuration = 0.25f;

    [Header("PAUSE")]
    [SerializeField] GameObject pausePanel;

    bool isPaused = false;

    /** ------------------------------------------ */


    float normal_image_time = 1.5f;
    float super_image_time = 10.0f;

    float timer = 0f;
    bool mostrar = false;

    // Start is called before the first frame update
    void Start()
    {
        animate.enabled = false;
        pausePanel.SetActive(false);

        ouch_image.enabled = false;
        miss_image.enabled = false;
        goal_image.enabled = false;
        byebye_image.enabled = false;

        /** subscribirse al evento */
        Kirby.instance.OnDamageTaken += MostrarOuch;
        Kirby.instance.OnDeadStart += MostrarGameOver;
        Kirby.instance.OnGoalGoaled += MostrarGoal;
        Kirby.instance.OnByeBye += MostrarByeBye;

        UpdateScoreUI();
    }

    void MostrarOuch()
    {
        mostrar = true;
        timer = normal_image_time;
        ouch_image.enabled = true;
        animate.enabled = true;
    }

    void MostrarGameOver()
    {
        mostrar = true;
        timer = super_image_time;
        miss_image.enabled = true;
        animate.enabled = true;
    }

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
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }

        if (mostrar)
        {
            timer -= Time.deltaTime; /** tiempo - deltaTime */

            if (timer <= 0f)
            {
                animate.enabled = false;
                mostrar = false;
                ouch_image.enabled = false;
                miss_image.enabled = false;
                goal_image.enabled = false;
                byebye_image.enabled = false;
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
    /** --- SCORE SYSTEM ---------------------------- */

    public void AddScore(int amount)
    {
        score += amount;

        UpdateScoreUI();
    }

    void UpdateScoreUI()
    {
        scoreText.text = " " + score;
    }

    void TogglePause()
    {
        isPaused = !isPaused;

        pausePanel.SetActive(isPaused);

        if (isPaused)
        {
            Time.timeScale = 0f;
        }
        else
        {
            Time.timeScale = 1f;
        }
    }

<<<<<<< HEAD
    public int GetScore()
    {
        return score;
    }
=======
    public void VolverAlInicio()
    {
        Time.timeScale = 1f;
        Application.Quit();
    }

>>>>>>> Izan
}
