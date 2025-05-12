using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    // public float smoothSpeed = 0.125f; // Remove or comment out Lerp speed
    public float smoothTime = 0.2f; // Time it takes to roughly reach the target (smaller value = faster/snappier)
    public Vector3 offset;

    private Vector3 initialCameraPosition;
    private Vector3 currentVelocity = Vector3.zero; // Need this variable for SmoothDamp

    void Start()
    {
        initialCameraPosition = transform.position;
        if (target == null) Debug.LogWarning("CameraFollow: Target not set!");
    }

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 targetPosition = new Vector3(target.position.x + offset.x, target.position.y + offset.y, initialCameraPosition.z);

        // Use SmoothDamp instead of Lerp
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref currentVelocity, smoothTime);
    }
}