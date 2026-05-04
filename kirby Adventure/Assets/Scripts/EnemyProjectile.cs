using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyProjectile : MonoBehaviour
{
    public int damage = 10;
    public float lifeTime = 6f;
    public GameObject hitVFX;

    Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
            rb = gameObject.AddComponent<Rigidbody2D>();

        rb.gravityScale = 0f;
    }

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other == null) return;

        // Si colisiona con Kirby, aplicamos daño
        var k = other.GetComponent<Kirby>();
        if (k != null)
        {
            k.TakeDamage(damage);
            if (hitVFX != null)
                Instantiate(hitVFX, transform.position, Quaternion.identity);
            Destroy(gameObject);
            return;
        }

        // Si colisiona con el suelo u otros, destruir
        if (other.gameObject.layer == LayerMask.NameToLayer("Ground") || other.CompareTag("Ground"))
        {
            if (hitVFX != null)
                Instantiate(hitVFX, transform.position, Quaternion.identity);
            Destroy(gameObject);
            return;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Manejar colisiones no-trigger
        var k = collision.collider.GetComponent<Kirby>();
        if (k != null)
        {
            k.TakeDamage(damage);
            if (hitVFX != null)
                Instantiate(hitVFX, transform.position, Quaternion.identity);
            Destroy(gameObject);
            return;
        }

        if (collision.collider.gameObject.layer == LayerMask.NameToLayer("Ground") || collision.collider.CompareTag("Ground"))
        {
            if (hitVFX != null)
                Instantiate(hitVFX, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }
}
