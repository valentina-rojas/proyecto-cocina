using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class LavadoManos : MonoBehaviour
{
    public static LavadoManos Instance { get; private set; }

    [Header("Canilla")]
    [SerializeField] private Button botonCanilla;
    [SerializeField] private Image imagenCanilla;
    [SerializeField] private Sprite canillaCerrada;
    [SerializeField] private Sprite canillaAbierta;

    [Header("Animaciones UI (Capas del Canvas)")]
    [SerializeField] private ImageFrameAnimation animacionAgua;  
    [SerializeField] private ImageFrameAnimation animacionManos;
    [SerializeField] private ImageFrameAnimation animacionEspuma;

    [Header("Progreso y Sensibilidad")]
    [Tooltip("Distancia en píxeles que debe frotarse el jabón para completar el lavado")]
    [SerializeField] private float distanciaTotalNecesaria = 2000f;
    [Tooltip("Tiempo en segundos sin mover el jabón antes de reiniciar la barra")]
    [SerializeField] private float tiempoParaReiniciar = 0.5f;

    [Header("UI")]
    [SerializeField] private Slider barra;
    [SerializeField] private GameObject objetoBarra;
    [SerializeField] private Button botonContinuar;

    [Header("Resultado")]
    [SerializeField] private Image imagenManos;
    [SerializeField] private Sprite manosSucias;
    [SerializeField] private Sprite manosLimpias;

    private float distanciaAcumulada;
    private float tiempoInactivo;
    private bool canillaEstaAbierta;
    private bool completado;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        CerrarCanilla();
    }

    private void Start()
    {
        ConfigurarBotonCanilla();
        ReiniciarLavado();
    }

    private void ConfigurarBotonCanilla()
    {
        if (botonCanilla != null)
        {
            botonCanilla.onClick.RemoveAllListeners();
            botonCanilla.onClick.AddListener(AlternarCanilla);
            botonCanilla.transition = Selectable.Transition.None;
        }
    }

    private void Update()
    {
        if (completado || distanciaAcumulada <= 0f) return;

        tiempoInactivo += Time.deltaTime;

        if (tiempoInactivo >= tiempoParaReiniciar)
        {
            ReiniciarLavado();
        }
    }

    // =========================================================
    // MECÁNICA DE CANILLA (TOGGLE)
    // =========================================================

    public void AlternarCanilla()
    {
        if (completado) return;

        if (canillaEstaAbierta)
            CerrarCanilla();
        else
            AbrirCanilla();
    }

    public void AbrirCanilla()
    {
        canillaEstaAbierta = true;

        if (imagenCanilla != null && canillaAbierta != null)
            imagenCanilla.sprite = canillaAbierta;

        // Inicia animación del agua
        if (animacionAgua != null)
        {
            animacionAgua.gameObject.SetActive(true);
            animacionAgua.Play();
        }

        Debug.Log("🚰 Canilla ABIERTA.");
    }

    public void CerrarCanilla()
    {
        canillaEstaAbierta = false;

        if (imagenCanilla != null && canillaCerrada != null)
            imagenCanilla.sprite = canillaCerrada;

        // Detiene y oculta animación del agua
        if (animacionAgua != null)
        {
            animacionAgua.Stop();
            animacionAgua.gameObject.SetActive(false);
        }

        SetVisualesLavando(false);

        Debug.Log("🚰 Canilla CERRADA.");
    }

    // =========================================================
    // MECÁNICA: FROTADO DE MANOS
    // =========================================================

    public void ProcesarFrotado(float deltaMovimiento)
    {
        if (!canillaEstaAbierta || completado || deltaMovimiento <= 0f) return;

        tiempoInactivo = 0f;
        distanciaAcumulada += deltaMovimiento;

        SetVisualesLavando(true);

        if (barra != null)
            barra.value = Mathf.Clamp01(distanciaAcumulada / distanciaTotalNecesaria);

        if (distanciaAcumulada >= distanciaTotalNecesaria)
        {
            CompletarLavado();
        }
    }

    public void ReiniciarLavado()
    {
        if (completado) return;

        distanciaAcumulada = 0f;
        tiempoInactivo = 0f;

        if (barra != null) barra.value = 0f;

        // Vuelve al sprite de manos sucias
        if (imagenManos != null && manosSucias != null)
            imagenManos.sprite = manosSucias;

        SetVisualesLavando(false);
        SetBotonContinuar(false);
    }

    private void SetVisualesLavando(bool activo)
    {
        if (objetoBarra != null && objetoBarra.activeSelf != activo)
            objetoBarra.SetActive(activo);

        if (animacionEspuma != null)
        {
            if (animacionEspuma.gameObject.activeSelf != activo)
                animacionEspuma.gameObject.SetActive(activo);

            if (activo) animacionEspuma.Play(); else animacionEspuma.Stop();
        }

        if (animacionManos != null)
        {
            if (activo) animacionManos.Play(); else animacionManos.Stop();
        }
    }

    private void CompletarLavado()
    {
        completado = true;
        SetVisualesLavando(false);

        // Al terminar el lavado también cerramos el agua
        CerrarCanilla();

        if (barra != null) barra.value = 1f;

        if (imagenManos != null && manosLimpias != null)
            imagenManos.sprite = manosLimpias;

        SetBotonContinuar(true, ContinuarDesdeLavado);
    }

    private void ContinuarDesdeLavado()
    {
        SetBotonContinuar(false);

        if (PopupContenido.Instance != null)
            PopupContenido.Instance.MostrarFeedbackLavado(TerminarEtapaLavado);
        else
            TerminarEtapaLavado();
    }

    private void TerminarEtapaLavado()
    {
        SetBotonContinuar(false);
        GameManager.Instance?.ContinuarDespuesDelLavado();
    }

    public void IniciarLavado()
    {
        completado = false;
        CerrarCanilla();
        ReiniciarLavado();

        if (PopupContenido.Instance != null)
            PopupContenido.Instance.MostrarInstruccionesLavado(ActivarCamaraLavado);
        else
            ActivarCamaraLavado();
    }

    private void ActivarCamaraLavado()
    {
        CameraManager.Instance?.MostrarCamaraLavadoManos();
    }

    private void SetBotonContinuar(bool visible, UnityAction accion = null)
    {
        if (botonContinuar == null) return;

        botonContinuar.onClick.RemoveAllListeners();
        if (visible && accion != null)
            botonContinuar.onClick.AddListener(accion);

        botonContinuar.interactable = visible;
        botonContinuar.gameObject.SetActive(visible);
    }
}