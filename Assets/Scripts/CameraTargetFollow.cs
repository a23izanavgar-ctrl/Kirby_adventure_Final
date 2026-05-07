using System.Collections;
using System.Collections.Generic;
using System.Security.Claims;
using UnityEngine;
using UnityEngine.UIElements;

public class CameraTargetFollow: MonoBehaviour
{
    [SerializeField]
    public Transform player;

    public float minX;
    public float maxX;

    public float minY;
    public float maxY;

    void LateUpdate()
    {
        float clampedX = Mathf.Clamp(player.position.x, minX, maxX);
        float clampedY = Mathf.Clamp(player.position.y, minY, maxY);
        
        transform.position = new Vector3(clampedX, clampedY, transform.position.z);

    }
}
