using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Absorber_colider : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            Destroy(other.gameObject); // o absorber lógica
            Debug.Log("Enemigo absorbido");
        }
    }
}
