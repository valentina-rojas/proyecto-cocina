using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Image))]
public class ManoVerduraController : MonoBehaviour, IDragHandler, IPointerDownHandler
{
    [Header("Sprites de Estados")]
    [SerializeField] private Sprite spriteSosteniendo;       // 1. Reposo con tomate
    [SerializeField] private Sprite spriteBajoAgua;          // 2. Quieto bajo el chorro
    [SerializeField] private Sprite spriteTomateLimpio;      // 3. Tomate limpio terminado

    [Header("Frames de Animación (Frotado)")]
    [SerializeField] private Sprite[] framesFrotado;
    [SerializeField] private float frameRate = 0.08f;

    private Image imagen;
    private bool estaFrotando;
    private int frameActual;
    private float temporizadorFrame;
    private bool puedeInteractuar;

    private void Awake()
    {
        imagen = GetComponent<Image>();
        // OCULTAR INMEDIATAMENTE AL DESPERTAR
        Ocultar();
    }

    private void Update()
    {
        if (!estaFrotando || framesFrotado == null || framesFrotado.Length == 0) return;

        temporizadorFrame += Time.deltaTime;
        if (temporizadorFrame >= frameRate)
        {
            temporizadorFrame = 0f;
            frameActual = (frameActual + 1) % framesFrotado.Length;
            imagen.sprite = framesFrotado[frameActual];
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // Requerido para inicializar el drag
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!puedeInteractuar) return;
        LavadoVerduras.Instance?.ProcesarFrotado(eventData.delta.magnitude);
    }

    // =========================================================
    // CONTROL VISUAL Y ESTADOS
    // =========================================================

    public void MostrarSosteniendo()
    {
        estaFrotando = false;
        puedeInteractuar = false;

        ActivarImagen(spriteSosteniendo);
    }

    public void MostrarBajoAgua()
    {
        estaFrotando = false;
        puedeInteractuar = true;

        ActivarImagen(spriteBajoAgua);
    }

    public void IniciarAnimacionFrotado()
    {
        if (estaFrotando) return;
        estaFrotando = true;
        temporizadorFrame = 0f;

        if (framesFrotado != null && framesFrotado.Length > 0)
        {
            imagen.sprite = framesFrotado[frameActual];
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
        estaFrotando = false;
        puedeInteractuar = false;

        ActivarImagen(spriteTomateLimpio);
    }

    public void Ocultar()
    {
        estaFrotando = false;
        puedeInteractuar = false;

        if (imagen == null) imagen = GetComponent<Image>();
        
        // Apagamos el renderer visual para que no se vea nada al arrancar
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
        {
            imagen.sprite = sprite;
        }
    }
}