using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Viento : Enemycontroller   
{
    private Vector2 direction;


    public void Init(Vector2 dir)
    {
        direction = dir.normalized;
    }

    void Update()
    {
        transform.position += (Vector3)(direction * speed * Time.deltaTime);
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
            Destroy(gameObject);
        }
    }

    public override bool CanBeAbsorbed()
    {
        return true;
    }
}
