using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    public Transform cameraTransform; // Assign your Main Camera's Transform here
    public float parallaxEffectMultiplierX = 0.95f; // How much the background moves relative to the camera on X. 1 = moves with camera, 0 = static. <1 = moves slower.
    public float parallaxEffectMultiplierY = 1f;    // How much the background moves relative to the camera on Y. Set to 1f if you want Y to be fixed or match camera.

    private Vector3 initialBackgroundPosition; // The starting position of this background object
    private Vector3 initialCameraPosition;   // The starting position of the camera

    void Start()
    {
        if (cameraTransform == null)
        {
            // Try to find the main camera if not assigned
            Camera mainCamera = Camera.main;
            if (mainCamera != null)
            {
                cameraTransform = mainCamera.transform;
            }
            else
            {
                Debug.LogError("ParallaxBackground: Camera Transform is not assigned and Main Camera could not be found!");
                enabled = false; // Disable the script if no camera
                return;
            }
        }

        initialBackgroundPosition = transform.position;
        initialCameraPosition = cameraTransform.position;
    }

    // LateUpdate is called after all Update functions have been called
    // Ideal for camera and parallax effects to ensure player/camera has moved first
    void LateUpdate()
    {
        if (cameraTransform == null) return;

        // Calculate how much the camera has moved from its starting position
        float deltaX = cameraTransform.position.x - initialCameraPosition.x;
        float deltaY = cameraTransform.position.y - initialCameraPosition.y;

        // Calculate the new position for the background
        // It moves a fraction of the distance the camera moved, based on the multiplier
        transform.position = new Vector3(
            initialBackgroundPosition.x + (deltaX * parallaxEffectMultiplierX),
            initialBackgroundPosition.y + (deltaY * parallaxEffectMultiplierY), // If you don't want Y parallax, use 'initialBackgroundPosition.y' here
            initialBackgroundPosition.z // Keep the original Z depth
        );
    }
}