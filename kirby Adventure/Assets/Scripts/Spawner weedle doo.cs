using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawnerweedledoo : MonoBehaviour
{
    [SerializeField] GameObject enemyPrefab;
    bool hasSpawned = false;
    void OnBecameVisible()
    {
        if (hasSpawned) return;

        Instantiate(enemyPrefab, transform.position, Quaternion.identity);
        hasSpawned = true;
    }

    

    
}
