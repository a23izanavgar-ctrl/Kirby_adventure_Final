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

    void Start()
    {
        ator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();

        StartCoroutine(MoveRoutine());
    }

    void Update()
    {
        // NO TOCAR TU LÓGICA
    }

    // ---------------------------
    // TU LÓGICA ORIGINAL
    // ---------------------------

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
        yield return new WaitForSeconds(0.5f);

        isAttacking = false;
        canMove = true;
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

    void Jump()
    {
        Vector2 jumpDir = new Vector2(Random.Range(-1f, 1f), 1f).normalized;

        rb.velocity = Vector2.zero;
        rb.AddForce(jumpDir * jumpForce, ForceMode2D.Impulse);
    }
}