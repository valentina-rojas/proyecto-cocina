using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class CoccionManager : MonoBehaviour
{
    public static CoccionManager Instance { get; private set; }

    [Header("Referencias")]
    [SerializeField] private Carne carne;

    [Header("UI")]
    [SerializeField] private Button botonContinuar;

    private bool coccionCompletada;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start() => SetBotonContinuar(false);

    // =========================================================
    // FLUJO DE ETAPA
    // =========================================================

    public void IniciarCoccion()
    {
        coccionCompletada = false;
        SetBotonContinuar(false);

        if (PopupContenido.Instance != null)
            PopupContenido.Instance.MostrarInstruccionesCoccion(ActivarCamaraCoccion);
        else
            ActivarCamaraCoccion();
    }

    private void ActivarCamaraCoccion()
    {
        CameraManager.Instance?.MostrarCamaraCoccionIngredientes();
    }

    public void CoccionCompleta()
    {
        if (coccionCompletada) return;
        coccionCompletada = true;

        SetBotonContinuar(true, ContinuarDesdeCoccion);
    }

    private void ContinuarDesdeCoccion()
    {
        SetBotonContinuar(false);

        bool carneEstaCruda = carne != null && carne.estado == Carne.Estado.Cruda;
        bool carneEstaQuemada = carne != null && carne.estado == Carne.Estado.Quemada;

        if (PopupContenido.Instance != null)
            PopupContenido.Instance.MostrarFeedbackCoccion(carneEstaCruda, carneEstaQuemada, TerminarEtapaCoccion);
        else
            TerminarEtapaCoccion();
    }

    private void TerminarEtapaCoccion()
    {
        SetBotonContinuar(false);
        GameManager.Instance?.ContinuarDespuesDeCoccion();
    }

    // =========================================================
    // HELPER UI
    // =========================================================

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