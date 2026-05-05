using UnityEngine;

public class SparkyHitbox : MonoBehaviour
{
    public int damage = 1;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Kirby.instance.TakeDamage(damage);
        }
    }
}
