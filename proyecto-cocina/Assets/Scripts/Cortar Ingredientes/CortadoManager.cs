using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class CortadoManager : MonoBehaviour
{
    public static CortadoManager Instance { get; private set; }

    [Header("UI")]
    [SerializeField] private Button botonContinuar;

    private bool cortadoCompletado;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start() => SetBotonContinuar(false);

    // =========================================================
    // FLUJO DE ETAPA
    // =========================================================

    public void IniciarCortado()
    {
        cortadoCompletado = false;
        SetBotonContinuar(false);

        if (PopupContenido.Instance != null)
            PopupContenido.Instance.MostrarInstruccionesCortado(ActivarCamaraCortado);
        else
            ActivarCamaraCortado();
    }

    private void ActivarCamaraCortado()
    {
        CameraManager.Instance?.MostrarCamaraCortadoIngredientes();
    }

    public void IngredienteCortado()
    {
        if (cortadoCompletado) return;
        cortadoCompletado = true;

        SetBotonContinuar(true, ContinuarDesdeCortado);
    }

    private void ContinuarDesdeCortado()
    {
        SetBotonContinuar(false);

        if (PopupContenido.Instance != null)
            PopupContenido.Instance.MostrarFeedbackCortado(TerminarEtapaCortado);
        else
            TerminarEtapaCortado();
    }

    private void TerminarEtapaCortado()
    {
        SetBotonContinuar(false);
        GameManager.Instance?.ContinuarDespuesDelCortado();
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