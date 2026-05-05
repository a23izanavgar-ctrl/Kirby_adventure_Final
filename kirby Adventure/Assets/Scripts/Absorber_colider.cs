using UnityEngine;

public class Absorber_colider : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            Kirby.instance.OnEnemyAbsorbed(other.gameObject);
        }
    }
}