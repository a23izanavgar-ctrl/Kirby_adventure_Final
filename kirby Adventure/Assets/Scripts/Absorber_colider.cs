using UnityEngine;

public class Absorber_colider : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        if (!Kirby.instance) return;

        // Solo enemigos
        if (!other.CompareTag("Enemy")) return;

        // Comprobamos estado REAL del Kirby
        if (Kirby.instance.IsAbsorbing())
        {
            Destroy(other.gameObject);
            Kirby.instance.OnAbsorbSuccess();
        }
    }
}