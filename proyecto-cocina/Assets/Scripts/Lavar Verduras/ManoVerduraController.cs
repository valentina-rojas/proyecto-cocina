using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Image))]
public class ManoVerduraController : MonoBehaviour, IDragHandler, IPointerDownHandler
{
    private Image imagen;
    private bool estaFrotando;
    private int frameActual;
    private float temporizadorFrame;
    private bool puedeInteractuar;

    // Datos de la verdura actualmente en la mano
    private DatosVerdura verduraActual;

    private void Awake()
    {
        imagen = GetComponent<Image>();
        Ocultar();
    }

    private void Update()
    {
        if (!estaFrotando || verduraActual == null || verduraActual.framesFrotado == null || verduraActual.framesFrotado.Length == 0) return;

        temporizadorFrame += Time.deltaTime;
        if (temporizadorFrame >= verduraActual.frameRate)
        {
            temporizadorFrame = 0f;
            frameActual = (frameActual + 1) % verduraActual.framesFrotado.Length;
            imagen.sprite = verduraActual.framesFrotado[frameActual];
        }
    }

    public void OnPointerDown(PointerEventData eventData) { }

    public void OnDrag(PointerEventData eventData)
    {
        if (!puedeInteractuar) return;
        LavadoVerduras.Instance?.ProcesarFrotado(eventData.delta.magnitude);
    }

    // =========================================================
    // CONFIGURACIÓN DINÁMICA POR VERDURA
    // =========================================================
    public void CargarVerdura(DatosVerdura datos)
    {
        verduraActual = datos;
        frameActual = 0;
        temporizadorFrame = 0f;
    }

    public void MostrarSosteniendo()
    {
        if (verduraActual == null) return;
        estaFrotando = false;
        puedeInteractuar = false;
        ActivarImagen(verduraActual.spriteManoSosteniendo);
    }

    public void MostrarBajoAgua()
    {
        if (verduraActual == null) return;
        estaFrotando = false;
        puedeInteractuar = true;
        ActivarImagen(verduraActual.spriteManoBajoAgua);
    }

    public void IniciarAnimacionFrotado()
    {
        if (estaFrotando || verduraActual == null) return;
        estaFrotando = true;
        temporizadorFrame = 0f;

        if (verduraActual.framesFrotado != null && verduraActual.framesFrotado.Length > 0)
        {
            imagen.sprite = verduraActual.framesFrotado[frameActual];
        }
    }

    public void PausarAnimacionFrotado()
    {
        if (!estaFrotando) return;
        estaFrotando = false;
        MostrarBajoAgua();
    }

    public void MostrarLimpio()
    {
        if (verduraActual == null) return;
        estaFrotando = false;
        puedeInteractuar = false;
        ActivarImagen(verduraActual.spriteManoLimpiaFinal);
    }

    public void Ocultar()
    {
        estaFrotando = false;
        puedeInteractuar = false;
        verduraActual = null;

        if (imagen == null) imagen = GetComponent<Image>();
        imagen.enabled = false;
        imagen.raycastTarget = false;
    }

    private void ActivarImagen(Sprite sprite)
    {
        if (imagen == null) imagen = GetComponent<Image>();
        imagen.enabled = true;
        imagen.raycastTarget = true;

        Color c = imagen.color;
        c.a = 1f;
        imagen.color = c;

        if (sprite != null)
            imagen.sprite = sprite;
    }
}