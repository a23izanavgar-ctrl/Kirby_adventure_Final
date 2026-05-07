using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rocky : Enemycontroller
{
    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer sr;

    private bool isHidden;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
    }

    void FixedUpdate()
    {
        if (isHidden)
        {
            rb.velocity = Vector2.zero;
            return;
        }

        rb.velocity = new Vector2(speed, rb.velocity.y);

        if (speed < 0)
            sr.flipX = false;
        else if (speed > 0)
            sr.flipX = true;

    }

    // ?? ENTRA PLAYER
    public void PlayerEntered()
    {
        Hide();
    }

    // ?? SALE PLAYER
    public void PlayerExited()
    {
        Unhide();
    }

    void Hide()
    {
        if (isHidden) return;

        isHidden = true;
        animator.SetBool("SeePlayer", true);
    }

    void Unhide()
    {
        if (!isHidden) return;

        isHidden = false;
        animator.SetBool("SeePlayer", false);
    }

    public override bool CanBeAbsorbed()
    {
        return !isHidden;
    }

    // ?? COLISIÓN PRINCIPAL (daño)
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (!isHidden)
            {
                Kirby.instance.TakeDamage(damage);
            }
        }
        else
        {
            // rebote o cambio de dirección si quieres
            speed *= -1;
        }
    }
}
