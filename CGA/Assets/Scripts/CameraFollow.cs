using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;  // The target that the camera will follow
    public float smoothing = 5f;  // How smooth the camera movement will be

    private Vector3 offset;  // The initial offset from the target

    void Start()
    {
        if (target == null)
        {
            Debug.LogError("CameraFollow: Target not assigned.");
            return;
        }

        // Calculate the initial offset between the camera and the target
        offset = transform.position - target.position;
    }

    void LateUpdate()
    {
        if (target == null) return;

        // Calculate the new position for the camera
        Vector3 targetCamPos = target.position + offset;

        // Smoothly move the camera towards that position
        transform.position = Vector3.Lerp(transform.position, targetCamPos, smoothing * Time.deltaTime);
    }
}
