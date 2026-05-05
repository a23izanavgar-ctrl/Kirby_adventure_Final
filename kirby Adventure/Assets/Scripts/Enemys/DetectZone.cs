using UnityEngine;

public class DetectZone : MonoBehaviour
{
    private Sparky sparky;

    void Start()
    {
        sparky = GetComponentInParent<Sparky>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            sparky.PlayerDetected();
        }
    }
}