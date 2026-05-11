using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColisionStart : MonoBehaviour
{
    [SerializeField] private BossArbol boss;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log("Kirby entró al boss");

            boss.StartBoss();

            gameObject.SetActive(false);
        }
    }

}
