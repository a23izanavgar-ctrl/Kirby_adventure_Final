using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerFrutas : MonoBehaviour
{
    [SerializeField] private GameObject applePrefab;

    [SerializeField] private float lineLength = 10f;

    public void Spawn()
    {
        if (applePrefab == null)
            return;

        float randomX = Random.Range(-lineLength / 2f, lineLength / 2f);

        Vector3 spawnPos = transform.position + new Vector3(randomX, 0f, 0);

        Instantiate(applePrefab, spawnPos, Quaternion.identity);
    }
}
