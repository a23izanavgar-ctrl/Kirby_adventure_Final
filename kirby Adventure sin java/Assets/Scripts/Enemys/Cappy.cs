using MongoDB.Driver;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cappy : Enemycontroller
{
    Animator ator;

    private Rigidbody2D rb;

    public float jumpForce = 5f;
    public float moveDistance = 2f;
    public float jumpCooldown = 2f;

    private Transform player;
    private bool canJump = true;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        ator = GetComponent<Animator>();
    }

    void Update()
    {
        if (canJump)
        {
            JumpTowardsPlayer();
            StartCoroutine(JumpCooldown());
        }
    }

    void JumpTowardsPlayer()
    {
        Vector2 direction = (player.position - transform.position).normalized;
        Vector2 jumpVector = new Vector2(direction.x * moveDistance, jumpForce);

        rb.velocity = Vector2.zero;
        rb.AddForce(jumpVector, ForceMode2D.Impulse);
    }

    IEnumerator JumpCooldown()
    {
        canJump = false;
        yield return new WaitForSeconds(jumpCooldown);
        canJump = true;
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
            Damaged(1);
            ator.SetBool("Hit", true);
            die();
        }
    }

    
}
