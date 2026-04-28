using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawnerweedledoo : MonoBehaviour
{
    [SerializeField]    
    public GameObject enemyPrefab;

    private GameObject currentEnemy;

    
    private Camera kirbyCamera;

    float spawnCooldown = 999f; // muy largo
    float timer = 0f;

    void Update()
    {
        // Buscar la cámara si aún no la tenemos
        if (kirbyCamera == null)
        {
            kirbyCamera = Camera.main;
            return; // esperamos al siguiente frame
        }

        if (IsVisibleToKirbyCamera())
        {
            if (currentEnemy == null)
            {
                currentEnemy = Instantiate(enemyPrefab, transform.position, Quaternion.identity);
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

        timer += Time.deltaTime;

        if (IsVisibleToKirbyCamera())
        {
            if (currentEnemy == null && timer >= spawnCooldown)
            {
                currentEnemy = Instantiate(enemyPrefab, transform.position, Quaternion.identity);
                timer = 0f;
            }
        }
    }

    bool IsVisibleToKirbyCamera()
    {
        Vector3 viewportPos = kirbyCamera.WorldToViewportPoint(transform.position);

        return viewportPos.x > -0.2f && viewportPos.x < 1.2f &&
               viewportPos.y > -0.2f && viewportPos.y < 1.2f;
    }

}

