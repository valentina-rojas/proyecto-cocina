using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ImageFrameAnimation : MonoBehaviour
{
    [Header("Referencias")]
    public Image targetImage;

    [Header("Animación")]
    public Sprite[] frames;
    public float frameRate = 0.08f;
    public bool loop = true;
    public bool playOnStart = false;

    Coroutine rutina;

    private void Start()
    {
        if (playOnStart)
            Play();
    }

    public void Play()
    {
        if (frames.Length == 0 || targetImage == null)
            return;

        // Si ya está reproduciéndose, no volver a iniciarla.
        if (rutina != null)
            return;

        rutina = StartCoroutine(Animacion());
    }

    public void Stop()
    {
        if (rutina == null)
            return;

        StopCoroutine(rutina);
        rutina = null;
    }
    IEnumerator Animacion()
    {
        do
        {
            for (int i = 0; i < frames.Length; i++)
            {
                targetImage.sprite = frames[i];
                yield return new WaitForSeconds(frameRate);
            }

        } while (loop);

        rutina = null;
    }
}