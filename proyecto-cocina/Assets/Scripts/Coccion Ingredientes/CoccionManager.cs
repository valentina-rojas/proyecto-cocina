using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class CoccionManager : MonoBehaviour
{
    public static CoccionManager Instance { get; private set; }

    [Header("Referencias")]
    [SerializeField] private Carne carne;
    [SerializeField] private Hornalla hornalla;
    [SerializeField] private TermometroTemperatura termometro;

    [Header("Animación de Palpitar (Perilla)")]
    [SerializeField] private float velocidadPalpitar = 4f;
    [SerializeField] private float escalaMinima = 0.98f;
    [SerializeField] private float escalaMaxima = 1.02f;

    [Header("UI")]
    [SerializeField] private Button botonContinuar;

    private bool coccionCompletada;

    // Control de palpitación interna
    private Transform perillaPalpitar;
    private Vector3 escalaOriginalPerilla = Vector3.one;
    private float tiempoAnimacion;
    private bool estaPalpitandoPerilla;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        SetBotonContinuar(false);

        if (hornalla != null && hornalla.botonCoccion != null)
        {
            hornalla.botonCoccion.interactable = false;
            // Al hacer clic en la perilla para encender el fuego, se detiene la animación
            hornalla.botonCoccion.onClick.AddListener(DetenerPalpitarPerilla);
        }
    }

    private void OnDisable()
    {
        ResetearEscalaPerilla();
    }

    private void LateUpdate()
    {
        if (!estaPalpitandoPerilla || perillaPalpitar == null || !perillaPalpitar.gameObject.activeInHierarchy)
            return;

        tiempoAnimacion += Time.unscaledDeltaTime * velocidadPalpitar;
        float factor = (Mathf.Sin(tiempoAnimacion) + 1f) * 0.5f;
        float multiplicadorEscala = Mathf.Lerp(escalaMinima, escalaMaxima, factor);

        perillaPalpitar.localScale = escalaOriginalPerilla * multiplicadorEscala;
    }

    // =========================================================
    // INICIO DE COCCIÓN
    // =========================================================

    public void IniciarCoccion()
    {
        coccionCompletada = false;
        ResetearEscalaPerilla();
        SetBotonContinuar(false);

        IniciarTemperatura();
    }

    public void IniciarTemperatura()
    {
        CameraManager.Instance?.MostrarCamaraCoccionIngredientes();

        if (termometro != null)
        {
            // Evita suscripciones duplicadas
            termometro.OnFinalizado -= HabilitarCoccionCarne;
            termometro.OnFinalizado += HabilitarCoccionCarne;

            termometro.gameObject.SetActive(true);
            termometro.Iniciar();
        }
        else
        {
            HabilitarCoccionCarne();
        }
    }

    // =========================================================
    // TEMPERATURA FINALIZADA
    // =========================================================

    private void HabilitarCoccionCarne()
    {
        if (termometro != null)
            termometro.OnFinalizado -= HabilitarCoccionCarne;

        // Si la temperatura seleccionada fue incorrecta,
        // la carne se cocina más rápido.
        if (carne != null &&
            termometro != null &&
            !termometro.TemperaturaCorrecta)
        {
            carne.tiempoAmarillo *= 0.7f;
            carne.tiempoVerde *= 0.7f;
        }

        if (hornalla != null && hornalla.botonCoccion != null)
        {
            hornalla.botonCoccion.interactable = true;
            // Inicia la palpitación de la perilla para indicar que hay que prender el fuego
            IniciarPalpitarPerilla(hornalla.botonCoccion.transform);
        }
    }

    // =========================================================
    // COCCIÓN DE LA CARNE COMPLETADA
    // =========================================================

    public void CoccionCompleta()
    {
        if (coccionCompletada)
            return;

        coccionCompletada = true;
        DetenerPalpitarPerilla();

        SetBotonContinuar(true, ContinuarDesdeCoccion);
    }

    // =========================================================
    // CONTINUAR DESDE COCCIÓN
    // =========================================================

    private void ContinuarDesdeCoccion()
    {
        SetBotonContinuar(false);

        bool carneEstaCruda =
            carne != null &&
            carne.estado == Carne.Estado.Cruda;

        bool carneEstaQuemada =
            carne != null &&
            carne.estado == Carne.Estado.Quemada;

        if (PopupContenido.Instance != null)
        {
            PopupContenido.Instance.MostrarFeedbackCoccion(
                carneEstaCruda,
                carneEstaQuemada,
                TerminarEtapaCoccion
            );
        }
        else
        {
            TerminarEtapaCoccion();
        }
    }

    // =========================================================
    // FINALIZAR ETAPA
    // =========================================================

    private void TerminarEtapaCoccion()
    {
        ResetearEscalaPerilla();
        SetBotonContinuar(false);

        GameManager.Instance?.ContinuarDespuesDeCoccion();
    }

    // =========================================================
    // BOTÓN CONTINUAR
    // =========================================================

    private void SetBotonContinuar(
        bool visible,
        UnityAction accion = null)
    {
        if (botonContinuar == null)
            return;

        botonContinuar.onClick.RemoveAllListeners();

        if (visible && accion != null)
            botonContinuar.onClick.AddListener(accion);

        botonContinuar.interactable = visible;
        botonContinuar.gameObject.SetActive(visible);
    }

    // =========================================================
    // CONTROL DE PALPITACIÓN DE LA PERILLA
    // =========================================================

    private void IniciarPalpitarPerilla(Transform objetivo)
    {
        if (objetivo == null) return;

        perillaPalpitar = objetivo;
        // Guarda la escala que tiene configurada en el Canvas para no romper su tamaño base
        escalaOriginalPerilla = objetivo.localScale.sqrMagnitude > 0.001f ? objetivo.localScale : Vector3.one;
        tiempoAnimacion = 0f;
        estaPalpitandoPerilla = true;
    }

    public void DetenerPalpitarPerilla()
    {
        estaPalpitandoPerilla = false;
        if (perillaPalpitar != null)
        {
            perillaPalpitar.localScale = escalaOriginalPerilla;
        }
    }

    private void ResetearEscalaPerilla()
    {
        estaPalpitandoPerilla = false;
        if (perillaPalpitar != null)
        {
            perillaPalpitar.localScale = escalaOriginalPerilla;
            perillaPalpitar = null;
        }
        tiempoAnimacion = 0f;
    }
}