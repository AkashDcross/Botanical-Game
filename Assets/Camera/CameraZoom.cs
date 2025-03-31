using UnityEngine;
using Cinemachine;

public class CameraZoom : MonoBehaviour
{
    public CinemachineVirtualCamera virtualCamera;
    public float zoomSpeed = 10f; // Speed of zooming in and out
    public float minZoom = 5f; // Minimum zoom level (higher zoom level is closer)
    public float maxZoom = 15f; // Maximum zoom level (lower zoom level is further away)
    public float smoothness = 5f; // Smoothness of the zoom transition

    private float targetZoom;

    void Start()
    {
        // Initialize targetZoom based on the current zoom level
        if (virtualCamera.m_Lens.Orthographic)
        {
            targetZoom = virtualCamera.m_Lens.OrthographicSize;
        }
        else
        {
            targetZoom = virtualCamera.m_Lens.FieldOfView;
        }
    }

    void Update()
    {
        // Get the scroll wheel input
        float scrollData = Input.GetAxis("Mouse ScrollWheel");

        // Adjust the target zoom based on scroll input
        targetZoom -= scrollData * zoomSpeed;
        targetZoom = Mathf.Clamp(targetZoom, minZoom, maxZoom);

        // Smoothly interpolate the zoom level
        if (virtualCamera.m_Lens.Orthographic)
        {
            virtualCamera.m_Lens.OrthographicSize = Mathf.Lerp(virtualCamera.m_Lens.OrthographicSize, targetZoom, Time.deltaTime * smoothness);
        }
        else
        {
            virtualCamera.m_Lens.FieldOfView = Mathf.Lerp(virtualCamera.m_Lens.FieldOfView, targetZoom, Time.deltaTime * smoothness);
        }
    }
}
