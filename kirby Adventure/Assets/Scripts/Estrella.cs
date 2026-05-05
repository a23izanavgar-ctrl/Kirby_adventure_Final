using UnityEngine;

public class Estrella : MonoBehaviour
{
    [SerializeField] float lifeTime = 3f;

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            Enemycontroller enemy = collision.gameObject.GetComponent<Enemycontroller>();

            if (enemy != null)
            {
                enemy.Damaged(1);
                enemy.die();
            }

            Destroy(gameObject);
        }
    }
}