using UnityEngine;

public class CameraAspectRatio : MonoBehaviour
{
    private float targetAspect = 16f / 9f;

    void Start()
    {
        float windowAspect = (float)Screen.width / Screen.height;

        float scaleHeight = windowAspect / targetAspect;

        Camera cam = GetComponent<Camera>();

        if (scaleHeight < 1.0f)
        {
            Rect rect = cam.rect;

            rect.width = 1;
            rect.height = scaleHeight;
            rect.x = 0;
            rect.y = (1 - scaleHeight) / 2;

            cam.rect = rect;
        }
        else
        {
            float scaleWidth = 1.0f / scaleHeight;

            Rect rect = cam.rect;

            rect.width = scaleWidth;
            rect.height = 1;
            rect.x = (1 - scaleWidth) / 2;
            rect.y = 0;

            cam.rect = rect;
        }
    }
}