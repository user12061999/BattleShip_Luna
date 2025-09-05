using UnityEngine;

public class CameraZoom : MonoBehaviour
{
    public float minOrthographic = 20f;
    public float maxOrthographic = 80f;
    private Camera cam;

    void Start()
    {
        cam = Camera.main;
    }

    void Update()
    {
        
    }

    public void Zoom(float increment)
    {
        cam.orthographicSize = Mathf.Clamp(cam.orthographicSize , minOrthographic, maxOrthographic);
    }
}