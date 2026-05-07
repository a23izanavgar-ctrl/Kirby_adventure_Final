using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField]
    public GameObject enemyPrefab;

    private GameObject currentEnemy;
    private Camera kirbyCamera;

    private bool wasVisible = false; // ?? clave

    void Update()
    {
        if (kirbyCamera == null)
        {
            kirbyCamera = Camera.main;
            return;
        }

        bool isVisible = IsVisibleToKirbyCamera();


        if (isVisible && !wasVisible)
        {
            if (currentEnemy == null)
            {
                currentEnemy = Instantiate(enemyPrefab, transform.position, Quaternion.identity);
            }
        }

   
        if (currentEnemy == null)
        {
            
        }

        wasVisible = isVisible;
    }

    bool IsVisibleToKirbyCamera()
    {
        Vector3 viewportPos = kirbyCamera.WorldToViewportPoint(transform.position);

        return viewportPos.x > 0 && viewportPos.x < 1 &&
               viewportPos.y > 0 && viewportPos.y < 1;
    }
}