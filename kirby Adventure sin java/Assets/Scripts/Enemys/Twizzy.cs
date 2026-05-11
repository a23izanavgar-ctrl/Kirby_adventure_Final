using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Twizzy : Enemycontroller
{
    private Rigidbody2D rb;
    private SpriteRenderer sr;

    [Header("Twizzy Settings")]
    public float detectionRange = 5f;
    public float attackSpeed = 8f;
    public float patrolHeightSpeed = 2f;

    private Transform player;

    private bool attacking = false;

    private Vector2 startPosition;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();

        player = GameObject.FindGameObjectWithTag("Player").transform;

        startPosition = transform.position;
    }

    void FixedUpdate()
    {
        if (player == null) return;

        float distance = Vector2.Distance(transform.position, player.position);

        // Detectar jugador
        if (distance <= detectionRange)
        {
            attacking = true;
        }

        if (attacking)
        {
            AttackPlayer();
        }
        else
        {
            Patrol();
        }

        Flip();
    }

    void Patrol()
    {
        // Movimiento horizontal
        rb.velocity = new Vector2(speed, Mathf.Sin(Time.time * patrolHeightSpeed));

    }

    void AttackPlayer()
    {
        Vector2 direction = (player.position - transform.position).normalized;

        rb.velocity = direction * attackSpeed;
    }

    void Flip()
    {
        if (rb.velocity.x > 0)
        {
            sr.flipX = false;
        }
        else if (rb.velocity.x < 0)
        {
            sr.flipX = true;
        }
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("COLISIÓN CON: " + collision.gameObject.name);

        if (collision.gameObject.CompareTag("Player"))
        {
            Kirby.instance.TakeDamage(damage);

            health = 0;

            die();
        }
        else
        {
            // Rebota al tocar paredes
            speed *= -1;
        }
    }
}