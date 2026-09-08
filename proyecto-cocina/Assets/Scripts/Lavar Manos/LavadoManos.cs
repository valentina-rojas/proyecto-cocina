using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class LavadoManos : MonoBehaviour
{
    public static LavadoManos Instance { get; private set; }

    [Header("Progreso y Sensibilidad")]
    [Tooltip("Distancia total en píxeles que debe frotarse el jabón para completar el lavado (ej: 1500 a 3000)")]
    [SerializeField] private float distanciaTotalNecesaria = 2000f;
    [Tooltip("Tiempo en segundos sin mover el jabón antes de reiniciar la barra")]
    [SerializeField] private float tiempoParaReiniciar = 0.5f;

    [Header("UI")]
    [SerializeField] private Slider barra;
    [SerializeField] private GameObject objetoBarra;
    [SerializeField] private Button botonContinuar;

    [Header("Animaciones")]
    [SerializeField] private ImageFrameAnimation animacionManos;
    [SerializeField] private ImageFrameAnimation animacionEspuma;

    [Header("Resultado")]
    [SerializeField] private Image imagenManos;
    [SerializeField] private Sprite manosLimpias;

    private float distanciaAcumulada;
    private float tiempoInactivo;
    private bool completado;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start() => ReiniciarLavado();

    private void Update()
    {
        if (completado || distanciaAcumulada <= 0f) return;

        // Si el usuario deja quieto el cursor o dedo, contamos inactividad
        tiempoInactivo += Time.deltaTime;

        if (tiempoInactivo >= tiempoParaReiniciar)
        {
            ReiniciarLavado();
        }
    }

    // =========================================================
    // MECÁNICA: SOLO SE EJECUTA SI HAY MOVIMIENTO FÍSICO
    // =========================================================

    public void ProcesarFrotado(float deltaMovimiento)
    {
        if (completado || deltaMovimiento <= 0f) return;

        // Se resetea el contador de inactividad porque se está moviendo activamente
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