using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZonaDetectarKirby : MonoBehaviour
{
    private Rocky rocky;

    void Start()
    {
        rocky = GetComponentInParent<Rocky>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            rocky.PlayerEntered();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            rocky.PlayerExited();
        }
    }
}
