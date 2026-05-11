using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemycontroller : MonoBehaviour
{
    [SerializeField]
    protected int health = 0;

    [SerializeField]
    protected float speed = 0f;

    [SerializeField]
    protected int damage = 0;
    [SerializeField]
    protected int Puntuacion = 0;

    public int GetHealth ()
    {
        return health;
    }
    public void setHealth(int health)
    {
        this.health = health;
    }

    public float GetSpeed()
    {
        return speed;
    }
    
    public int GetPoints()
    {
        return Puntuacion;
    }

    public virtual void  Damaged(int Damage)
    {
        health -= Damage;
    }

    public virtual void die()
    {
        if (health <= 0)
        {
            FindObjectOfType<HUDKirby>().AddScore(Puntuacion);
            OnDeath();
        }
    }

    protected virtual void OnDeath()
    {
        Destroy(gameObject);
    }

    public virtual bool CanBeAbsorbed()
    {
        return true;
    }

}
