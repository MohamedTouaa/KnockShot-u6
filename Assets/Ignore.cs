using UnityEngine;

public class IgnoreLayer : MonoBehaviour
{
    // Reference to the camera
    public Camera mainCamera;

    // Layer to ignore (change "IgnoreLayer" to the name of your layer)
    public string layerToIgnore = "IgnoreLayer";

    void Start()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main; // If no camera is assigned, get the main camera
        }

        // Get the layer mask for the layer to ignore
        int layerMask = 1 << LayerMask.NameToLayer(layerToIgnore);

        // Invert the layer mask and set it to the camera's culling mask to exclude the layer
        mainCamera.cullingMask &= ~layerMask;
    }
}
