using UnityEngine;

public class CameraLayerRender : MonoBehaviour
{
    public string layerToRender = "Speedlines";  

    void Start()
    {
        // Get the camera component attached to this object
        Camera cam = GetComponent<Camera>();

        if (cam != null)
        {
            
            int layerMask = LayerMask.GetMask(layerToRender);

            cam.cullingMask = layerMask;
        }
        else
        {
            Debug.LogError("No Camera component found on this GameObject.");
        }
    }
}
