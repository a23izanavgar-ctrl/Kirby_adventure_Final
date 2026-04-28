using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WeedleDee : Enemycontroller
{
    private Rigidbody2D rb;

    private SpriteRenderer sr;

    

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        Debug.Log(damage);
    }

    void Flip()
    {
        speed *= -1;
        sr.flipX = !sr.flipX;
        
    }
    void FixedUpdate()
    {
        rb.velocity = new Vector2(speed, rb.velocity.y);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {

        if (collision.gameObject.CompareTag("Player"))
        {
            Kirby.instance.TakeDamage(damage);
        }
        Debug.Log("COLISIÓN CON: " + collision.gameObject.name);

        Flip();
    }



    
}

