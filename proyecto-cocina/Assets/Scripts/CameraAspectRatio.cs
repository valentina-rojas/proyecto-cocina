using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraAspectRatio : MonoBehaviour
{
    private float targetAspect = 16f / 9f;

    void Start()
    {
        CrearCamaraFondoNegro();
        AjustarViewport();
    }

    private void AjustarViewport()
    {
        float windowAspect = (float)Screen.width / Screen.height;
        float scaleHeight = windowAspect / targetAspect;
        Camera cam = GetComponent<Camera>();

        Rect rect = cam.rect;

        if (scaleHeight < 1.0f)
        {
            rect.width = 1f;
            rect.height = scaleHeight;
            rect.x = 0;
            rect.y = (1f - scaleHeight) / 2f;
        }
        else
        {
            float scaleWidth = 1.0f / scaleHeight;
            rect.width = scaleWidth;
            rect.height = 1f;
            rect.x = (1f - scaleWidth) / 2f;
            rect.y = 0;
        }

        cam.rect = rect;
    }

    private void CrearCamaraFondoNegro()
    {
        // Verifica si ya existe una cámara de fondo para no duplicarla
        if (GameObject.Find("Letterbox_BackgroundCamera") != null) return;

        GameObject bgCamObj = new GameObject("Letterbox_BackgroundCamera");
        Camera bgCam = bgCamObj.AddComponent<Camera>();
        bgCam.clearFlags = CameraClearFlags.SolidColor;
        bgCam.backgroundColor = Color.black;
        bgCam.cullingMask = 0; // Nothing
        bgCam.depth = -100;    // Se dibuja detrás de todas
        bgCam.rect = new Rect(0, 0, 1, 1);
    }
}