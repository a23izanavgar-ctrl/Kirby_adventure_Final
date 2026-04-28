using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Absorber_colider : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            WeedleDee enemy = other.GetComponent<WeedleDee>();

            if (enemy != null)
            {
                enemy.NotifyDeath();
            }

            Destroy(other.gameObject);
            Debug.Log("Enemigo absorbido");
        }
    }
}
