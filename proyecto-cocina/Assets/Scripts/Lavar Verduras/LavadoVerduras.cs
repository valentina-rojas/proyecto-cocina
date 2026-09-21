using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class LavadoVerduras : MonoBehaviour
{
    public static LavadoVerduras Instance { get; private set; }

    [Header("Canilla")]
    [SerializeField] private Button botonCanilla;
    [SerializeField] private Image imagenCanilla;
    [SerializeField] private Sprite canillaCerrada;
    [SerializeField] private Sprite canillaAbierta;
    [SerializeField] private ImageFrameAnimation animacionAgua;

    [Header("Tomate en Mesa")]
    [SerializeField] private Button botonTomateMesa;

    [Header("Mano y Verdura")]
    [SerializeField] private ManoVerduraController manoVerdura;

    [Header("Progreso y Sensibilidad")]
    [Tooltip("Distancia en píxeles que debe frotarse para completar el lavado")]
    [SerializeField] private float distanciaTotalNecesaria = 1800f;
    [Tooltip("Tiempo en segundos sin mover el dedo/mouse antes de reiniciar la barra a 0")]
    [SerializeField] private float tiempoParaReiniciar = 0.5f;

    [Header("UI")]
    [SerializeField] private Slider barraProgreso;
    [Tooltip("El GameObject padre del Slider que contiene fondo, relleno y borde")]
    [SerializeField] private GameObject objetoBarra;
    [SerializeField] private Button botonContinuar;

    [Header("Tiempos de Feedback")]
    [Tooltip("Pausa en segundos para contemplar el tomate limpio antes de mostrar el botón")]
    [SerializeField] private float delayAntesDeMostrarBoton = 0.35f;

    private float distanciaAcumulada;
    private float tiempoInactivo;
    private bool estaMoviendo;
    private bool tomateAgarrado;
    private bool canillaEstaAbierta;
    private bool completado;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        ConfigurarBotones();
        ReiniciarEscena();
    }

    private void ConfigurarBotones()
    {
        if (botonCanilla != null)
        {
            botonCanilla.onClick.RemoveAllListeners();
            botonCanilla.onClick.AddListener(AlternarCanilla);
            botonCanilla.transition = Selectable.Transition.None;
        }

        if (botonTomateMesa != null)
        {
            botonTomateMesa.onClick.RemoveAllListeners();
            botonTomateMesa.onClick.AddListener(AgarrarTomate);
        }
    }

    private void Update()
    {
        // Si ya completó o no ha empezado a frotar todavía, no hacemos nada
        if (completado || distanciaAcumulada <= 0f) return;

        tiempoInactivo += Time.deltaTime;

        // Si se detiene y supera el tiempo límite, se reinicia la barra como en LavadoManos
        if (tiempoInactivo >= tiempoParaReiniciar)
        {
            ReiniciarLavado();
        }
    }

    // =========================================================
    // 1. CLIC EN EL TOMATE DE LA MESA
    // =========================================================
    public void AgarrarTomate()
    {
        if (tomateAgarrado) return;

        tomateAgarrado = true;

        if (botonTomateMesa != null)
            botonTomateMesa.gameObject.SetActive(false);

        // Habilita la canilla tras agarrar el tomate
        if (botonCanilla != null)
            botonCanilla.interactable = true;

        if (manoVerdura != null)
        {
            manoVerdura.MostrarSosteniendo();
        }
    }

    // =========================================================
    // 2. ABRIR / CERRAR CANILLA
    // =========================================================
    public void AlternarCanilla()
    {
        if (completado || !tomateAgarrado) return;

        if (canillaEstaAbierta) CerrarCanilla();
        else AbrirCanilla();
    }

    private void AbrirCanilla()
    {
        if (!tomateAgarrado || completado) return;

        canillaEstaAbierta = true;

        if (imagenCanilla != null && canillaAbierta != null)
            imagenCanilla.sprite = canillaAbierta;

        if (animacionAgua != null)
        {
            animacionAgua.gameObject.SetActive(true);
            animacionAgua.Play();
        }

        manoVerdura?.MostrarBajoAgua();
        SetVisibilidadBarra(true);
    }

    private void CerrarCanilla()
    {
        canillaEstaAbierta = false;

        if (imagenCanilla != null && canillaCerrada != null)
            imagenCanilla.sprite = canillaCerrada;

        if (animacionAgua != null)
        {
            animacionAgua.Stop();
            animacionAgua.gameObject.SetActive(false);
        }

        // Si se cierra el agua a mitad del lavado, se reinicia el progreso
        ReiniciarLavado();

        if (tomateAgarrado && !completado)
        {
            manoVerdura?.MostrarSosteniendo();
            SetVisibilidadBarra(false);
        }
    }

    // =========================================================
    // 3. FROTADO Y PROGRESO (Idéntico a LavadoManos)
    // =========================================================
    public void ProcesarFrotado(float deltaMovimiento)
    {
        if (!canillaEstaAbierta || !tomateAgarrado || completado || deltaMovimiento <= 0f) return;

        // Resetea el reloj de inactividad cada vez que hay movimiento
        tiempoInactivo = 0f;
        distanciaAcumulada += deltaMovimiento;

        float porcentaje = Mathf.Clamp01(distanciaAcumulada / distanciaTotalNecesaria);
        if (barraProgreso != null) barraProgreso.value = porcentaje;

        // Activa la animación continua
        if (!estaMoviendo)
        {
            estaMoviendo = true;
            manoVerdura?.IniciarAnimacionFrotado();
        }

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
        estaMoviendo = false;

        if (barraProgreso != null) barraProgreso.value = 0f;

        // Detener animación y volver al frame quieto bajo el agua si sigue abierta
        manoVerdura?.PausarAnimacionFrotado();

        if (canillaEstaAbierta && tomateAgarrado)
        {
            manoVerdura?.MostrarBajoAgua();
        }
    }

    // =========================================================
    // 4. FINALIZAR ETAPA
    // =========================================================
    private void CompletarLavado()
    {
        completado = true;
        estaMoviendo = false;

        CerrarCanilla();

        if (botonCanilla != null)
            botonCanilla.interactable = false;

        // Mostrar de inmediato el tomate limpio
        manoVerdura?.MostrarLimpio();

        StartCoroutine(RutinaFinalizacion());
    }

    private IEnumerator RutinaFinalizacion()
    {
        if (barraProgreso != null) barraProgreso.value = 1f;

        yield return new WaitForSeconds(delayAntesDeMostrarBoton);

        SetVisibilidadBarra(false);
        SetBotonContinuar(true, Finalizar);
    }

    private void SetVisibilidadBarra(bool visible)
    {
        if (objetoBarra != null)
        {
            objetoBarra.SetActive(visible);
        }
        else if (barraProgreso != null)
        {
            barraProgreso.gameObject.SetActive(visible);
        }
    }

    public void ReiniciarEscena()
    {
        StopAllCoroutines();

        completado = false;
        tomateAgarrado = false;
        canillaEstaAbierta = false;
        estaMoviendo = false;
        distanciaAcumulada = 0f;
        tiempoInactivo = 0f;

        CerrarCanilla();

        if (botonCanilla != null)
            botonCanilla.interactable = false;

        if (botonTomateMesa != null) botonTomateMesa.gameObject.SetActive(true);
        SetVisibilidadBarra(false);
        if (barraProgreso != null) barraProgreso.value = 0f;

        manoVerdura?.Ocultar();
        SetBotonContinuar(false);
    }

    private void SetBotonContinuar(bool visible, UnityAction accion = null)
    {
        if (botonContinuar == null) return;

        botonContinuar.onClick.RemoveAllListeners();
        if (visible && accion != null)
            botonContinuar.onClick.AddListener(accion);

        botonContinuar.interactable = visible;
        botonContinuar.gameObject.SetActive(visible);

        if (botonContinuar.TryGetComponent<CanvasGroup>(out var cg))
        {
            cg.alpha = visible ? 1f : 0f;
            cg.interactable = visible;
            cg.blocksRaycasts = visible;
        }
    }

    private void Finalizar()
    {
        SetBotonContinuar(false);
        GameManager.Instance?.ContinuarDespuesDelLavado();
    }
}