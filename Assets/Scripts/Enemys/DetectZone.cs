using System.Collections;
using UnityEngine;

public class DetectZone : MonoBehaviour
{
    private Sparky sparky;

    [Header("Tiempo")]
    [SerializeField]
    private float time = 0.5f;

    private bool detecting = false;

    void Start()
    {
        sparky = GetComponentInParent<Sparky>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !detecting)
        {
            StartCoroutine(DetectDelay());
        }
    }

    IEnumerator DetectDelay()
    {
        detecting = true;

        yield return new WaitForSeconds(time);

        sparky.PlayerDetected();

        detecting = false;
    }
}