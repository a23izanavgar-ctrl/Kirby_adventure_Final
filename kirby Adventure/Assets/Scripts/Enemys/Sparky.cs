using System.Collections;
using UnityEngine;

public class Sparky : Enemycontroller
{
    private Animator ator;
    private Rigidbody2D rb;

    private bool isAttacking = false;
    private bool isDead = false;
    private bool canMove = true;

    public float jumpForce = 3f;
    public float moveInterval = 1.5f;
    
    [Header("Hitbox")]
    [SerializeField] private GameObject hitboxObject;

    private SpriteRenderer hitboxSprite;
    private Collider2D hitboxCollider;

    void Start()
    {
        ator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();

        // Obtener componentes de la hitbox
        hitboxSprite = hitboxObject.GetComponent<SpriteRenderer>();
        hitboxCollider = hitboxObject.GetComponent<Collider2D>();

        // Asegurar que empieza desactivada
        if (hitboxSprite != null)
            hitboxSprite.enabled = false;

        if (hitboxCollider != null)
            hitboxCollider.enabled = false;

        StartCoroutine(MoveRoutine());
    }

    public void PlayerDetected()
    {
        if (!isAttacking && !isDead)
        {
            StartCoroutine(AttackRoutine());
        }
    }

    IEnumerator AttackRoutine()
    {
        isAttacking = true;
        canMove = false;

        ator.SetTrigger("Idle");
        yield return new WaitForSeconds(0.3f);

        ator.SetTrigger("Charge");
        yield return new WaitForSeconds(0.8f);

        ator.SetTrigger("Attack");

        // Activar hitbox + sprite
        SetHitbox(true);

        yield return new WaitForSeconds(0.3f);

        SetHitbox(false);

        yield return new WaitForSeconds(0.2f);

        isAttacking = false;
        canMove = true;
    }

    void SetHitbox(bool state)
    {
        if (hitboxSprite != null)
            hitboxSprite.enabled = state;

        if (hitboxCollider != null)
            hitboxCollider.enabled = state;
    }

    public void TakeDamage(int amount)
    {
        if (isDead) return;

        health -= amount;

        if (health <= 0)
        {
            isDead = true;
            canMove = false;
            StopAllCoroutines();

            ator.SetTrigger("Die");
            StartCoroutine(DieDelay());
        }
    }

    IEnumerator DieDelay()
    {
        yield return new WaitForSeconds(1f);
        die();
    }

    IEnumerator MoveRoutine()
    {
        while (!isDead)
        {
            if (canMove && !isAttacking)
            {
                Jump();
            }

            yield return new WaitForSeconds(moveInterval);
        }
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("COLISIÓN CON: " + collision.gameObject.name);

        if (collision.gameObject.CompareTag("Player"))
        {

            Kirby.instance.TakeDamage(damage);

            health -= 1;

            ator.SetBool("Hit", true);
            die();
        }

        if (collision.gameObject.CompareTag("Estrella"))
        {
            Debug.Log("Estrlla");
            ator.SetBool("Hit", true);
            die();
        }
    }
    void Jump()
    {
        Vector2 jumpDir = new Vector2(Random.Range(-1f, 1f), 1f).normalized;

        rb.velocity = Vector2.zero;
        rb.AddForce(jumpDir * jumpForce, ForceMode2D.Impulse);
    }
}