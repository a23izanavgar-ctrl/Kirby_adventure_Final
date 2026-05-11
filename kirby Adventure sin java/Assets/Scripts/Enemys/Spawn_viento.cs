using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawn_viento : MonoBehaviour
{
    [SerializeField] private GameObject windPrefab;

    public void SpawnWind()
    {
        Spawn(Vector2.left, Vector3.zero);
        Spawn(new Vector2(-1f, -0.3f), Vector3.down * 1f);
        Spawn(new Vector2(-1f, -0.6f), Vector3.down * 2f);
    }

    void Spawn(Vector2 dir, Vector3 offset)
    {
        GameObject w = Instantiate(windPrefab, transform.position + offset, Quaternion.identity);

        Viento wp = w.GetComponent<Viento>();

        wp.Init(dir);
    }
}
