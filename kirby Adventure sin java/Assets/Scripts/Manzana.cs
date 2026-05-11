using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manzana : Enemycontroller
{
    private int bounceCount = 0;

    private void Start()
    {
        Destroy(gameObject, 10f);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Kirby.instance.TakeDamage(damage);

            Destroy(gameObject);
        }

        if (collision.gameObject.CompareTag("Ground"))
        {
            bounceCount++;

            if (bounceCount >= 2)
            {
                Destroy(gameObject);
            }
        }
    }

    public override bool CanBeAbsorbed()
    {
        return true;
    }
}
