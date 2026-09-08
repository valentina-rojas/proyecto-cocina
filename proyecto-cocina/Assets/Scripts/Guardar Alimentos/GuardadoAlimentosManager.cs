using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class GuardadoAlimentosManager : MonoBehaviour
{
    public static GuardadoAlimentosManager Instance { get; private set; }

    [Header("UI")]
    [SerializeField] private Button botonContinuar;

    private InventorySlot[] todosLosSlots;
    private bool guardadoCompletado;
    private bool feedbackMostrado;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        todosLosSlots = FindObjectsByType<InventorySlot>(FindObjectsSortMode.None);
        SetBotonContinuar(false);
        ChequearEstadoGuardado();
    }

    private void OnEnable() => DraggableItem.OnAnyItemEndDrag += OnItemTerminoDeMoverse;
    private void OnDisable() => DraggableItem.OnAnyItemEndDrag -= OnItemTerminoDeMoverse;

    private void OnItemTerminoDeMoverse()
    {
        if (!guardadoCompletado)
            ChequearEstadoGuardado();
    }

    // =========================================================
    // MECÁNICA Y VALIDACIÓN
    // =========================================================

    public void ChequearEstadoGuardado()
    {
        if (todosLosSlots == null) return;

        int alimentosEnMesa = todosLosSlots
            .Where(slot => slot != null && slot.tipoDeEstanteAceptado == "mesa")
            .Sum(slot => slot.transform.childCount);

        if (alimentosEnMesa == 0)
            CompletarGuardado();
        else
            SetBotonContinuar(false);
    }

    private void CompletarGuardado()
    {
        if (guardadoCompletado) return;
        guardadoCompletado = true;

        SetBotonContinuar(true, ContinuarDesdeGuardado);
    }

    private void ContinuarDesdeGuardado()
    {
        SetBotonContinuar(false);
        MostrarFeedback();
    }

    private void MostrarFeedback()
    {
        if (feedbackMostrado) return;
        feedbackMostrado = true;

        EvaluarIngredientes();

        bool ingredientesCorrectos = GameManager.Instance.Score == null || 
                                     !GameManager.Instance.Score.IngredientesMalOrdenados;

        if (PopupContenido.Instance != null)
            PopupContenido.Instance.MostrarFeedbackIngredientes(ingredientesCorrectos, TerminarEtapa);
        else
            TerminarEtapa();
    }

    private void TerminarEtapa()
    {
        SetBotonContinuar(false);
        GameManager.Instance?.ContinuarDespuesDelGuardado();
    }

    public void EvaluarIngredientes()
    {
        if (todosLosSlots == null) return;

        bool hayIncorrectos = false;

        foreach (InventorySlot slot in todosLosSlots)
        {
            if (slot == null) continue;

            IngredienteData ingrediente = slot.GetComponentInChildren<IngredienteData>();
            if (ingrediente != null && ingrediente.tipoIngrediente != slot.tipoDeEstanteAceptado)
            {
                hayIncorrectos = true;
                break;
            }
        }

        if (hayIncorrectos)
            GameManager.Instance?.RegistrarIngredientesMalOrdenados();
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