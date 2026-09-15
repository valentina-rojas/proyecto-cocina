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

    [Header("UI")]
    [SerializeField] private Button botonContinuar;

    private bool coccionCompletada;

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
            hornalla.botonCoccion.interactable = false;
    }

    // =========================================================
    // INICIO DE COCCIÓN
    // =========================================================

    public void IniciarCoccion()
    {
        coccionCompletada = false;
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
            hornalla.botonCoccion.interactable = true;
    }

    // =========================================================
    // COCCIÓN DE LA CARNE COMPLETADA
    // =========================================================

    public void CoccionCompleta()
    {
        if (coccionCompletada)
            return;

        coccionCompletada = true;

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
}