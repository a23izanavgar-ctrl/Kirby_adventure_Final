using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawnerweedledoo : MonoBehaviour
{
    [SerializeField]
    public GameObject enemyPrefab;

    private GameObject currentEnemy;


    private Camera kirbyCamera;


    private bool enemyDefeated = false;
    void Update()
    {
        if (kirbyCamera == null)
        {
            kirbyCamera = Camera.main;
            return;
        }

        if (enemyDefeated)
            return;

        if (IsVisibleToKirbyCamera())
        {
            if (currentEnemy == null)
            {
                currentEnemy = Instantiate(enemyPrefab, transform.position, Quaternion.identity);

                currentEnemy.GetComponent<WeedleDee>().SetSpawner(this);
            }
        }
        else
        {
            if (currentEnemy != null)
            {
                Destroy(currentEnemy);
                currentEnemy = null;
            }
        }
    }

    bool IsVisibleToKirbyCamera()
    {
        Vector3 viewportPos = kirbyCamera.WorldToViewportPoint(transform.position);

        return viewportPos.x > -0.2f && viewportPos.x < 1.2f &&
               viewportPos.y > -0.2f && viewportPos.y < 1.2f;
    }
    public void EnemyDefeated()
    {
        enemyDefeated = true;
    }
}

    