using UnityEngine;

public class Absorber_colider : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            Enemycontroller enemy = other.GetComponent<Enemycontroller>();

            if (enemy != null && enemy.CanBeAbsorbed())
            {
                Kirby.instance.OnEnemyAbsorbed(other.gameObject);
            }
        }
    }
}