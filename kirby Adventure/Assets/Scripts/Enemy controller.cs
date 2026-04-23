using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemycontroller : MonoBehaviour
{
    [SerializeField]
   protected int health = 0;

    [SerializeField]
    protected float speed = 0f;


    public int GetHealth ()
    {
        return health;
    }

    public float GetSpeed()
    {
        return speed;
    }

}
