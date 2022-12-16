using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerCam : MonoBehaviour
{
    public Transform target;
    void Start()
    {
        Vector3 targetPos = target.position;
        targetPos.z = transform.position.z;
        transform.position = targetPos;
    }
    void FixedUpdate()
    {
        Vector3 targetPos = target.position;
        targetPos.z = transform.position.z;
        transform.position = targetPos;
    }
}
