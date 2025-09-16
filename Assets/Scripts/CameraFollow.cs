using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Attributes")]
    [SerializeField]
    private Transform target;
    [SerializeField] private float cameraSmoothing;
    public Vector3 cameraOffset;
    private Vector3 currentVelocity;


    void Update()
    {
        Vector3 targetPosition = target.position + cameraOffset;
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref currentVelocity, cameraSmoothing);
    }
}
