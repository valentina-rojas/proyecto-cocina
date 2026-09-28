using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class GuardadoAlimentosManager : MonoBehaviour
{
    public static GuardadoAlimentosManager Instance { get; private set; }

    [Header("UI")]
    [SerializeField] private Button botonContinuar;

    private InventorySlot[] todosLosSlots;
    private int cantidadInicialIngredientes;
    private bool feedbackMostrado;
    private bool faseCompletada;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        DraggableItem.OnAnyItemEndDrag += OnItemTerminoDeMoverse;
    }

    private void OnDisable()
    {
        DraggableItem.OnAnyItemEndDrag -= OnItemTerminoDeMoverse;
    }

    private IEnumerator Start()
    {
        SetBotonContinuar(false);

        yield return new WaitUntil(() =>
            FindObjectsByType<InventorySlot>(FindObjectsSortMode.None).Length > 0
        );

        todosLosSlots = FindObjectsByType<InventorySlot>(FindObjectsSortMode.None);

        yield return null;

        cantidadInicialIngredientes = ContarIngredientesEnMesas();

        Debug.Log($"Guardado de alimentos iniciado. Ingredientes iniciales: {cantidadInicialIngredientes}");
    }

    // =========================================================
    // EVENTO AL TERMINAR DE ARRASTRAR
    // =========================================================

    private void OnItemTerminoDeMoverse()
    {
        // Si ya terminamos esta etapa o no estamos en la fase de guardado/ordenado, NO interferir
        if (faseCompletada)
            return;

        if (GameManager.Instance != null && GameManager.Instance.estadoActual != GameManager.EstadoJuego.OrdenandoIngredientes)
            return;

        StartCoroutine(ChequearDespuesDeUnFrame());
    }

    private IEnumerator ChequearDespuesDeUnFrame()
    {
        yield return null;
        ChequearEstadoGuardado();
    }

    // =========================================================
    // CONTAR INGREDIENTES EN LAS MESAS
    // =========================================================

    private int ContarIngredientesEnMesas()
    {
        if (todosLosSlots == null)
            return 0;

        int total = 0;

        foreach (InventorySlot slot in todosLosSlots)
        {
            if (slot == null || !slot.EsMesa)
                continue;

            DraggableItem[] ingredientes = slot.GetComponentsInChildren<DraggableItem>(true);
            total += ingredientes.Length;
        }

        return total;
    }

    // =========================================================
    // MECÁNICA Y VALIDACIÓN
    // =========================================================

    public void ChequearEstadoGuardado()
    {
        if (faseCompletada)
            return;

        if (GameManager.Instance != null && GameManager.Instance.estadoActual != GameManager.EstadoJuego.OrdenandoIngredientes)
            return;

        if (todosLosSlots == null || todosLosSlots.Length == 0)
        {
            Debug.LogWarning("⚠️ No hay InventorySlot disponibles todavía.");
            return;
        }

        int alimentosEnMesa = ContarIngredientesEnMesas();
        Debug.Log($"Ingredientes restantes en mesa: {alimentosEnMesa}");

        if (cantidadInicialIngredientes <= 0)
        {
            Debug.LogWarning("⚠️ No se detectaron ingredientes iniciales.");
            SetBotonContinuar(false);
            return;
        }

        // En esta fase: SOLO avanza si la mesa queda vacía
        if (alimentosEnMesa == 0)
        {
            SetBotonContinuar(true, ContinuarDesdeGuardado);
        }
        else
        {
            SetBotonContinuar(false);
        }
    }

    // =========================================================
    // CONTINUAR
    // =========================================================

    private void ContinuarDesdeGuardado()
    {
        if (ContarIngredientesEnMesas() > 0)
        {
            Debug.LogWarning("⚠️ No puedes continuar: todavía hay alimentos en la mesa.");
            SetBotonContinuar(false);
            return;
        }

        SetBotonContinuar(false);
        MostrarFeedback();
    }

    // =========================================================
    // FEEDBACK
    // =========================================================

    private void MostrarFeedback()
    {
        if (feedbackMostrado)
            return;

        feedbackMostrado = true;
        EvaluarIngredientes();

        bool ingredientesCorrectos =
            GameManager.Instance == null ||
            GameManager.Instance.Score == null ||
            !GameManager.Instance.Score.IngredientesMalOrdenados;

        if (PopupContenido.Instance != null)
        {
            PopupContenido.Instance.MostrarFeedbackIngredientes(
                ingredientesCorrectos,
                TerminarEtapa
            );
        }
        else
        {
            TerminarEtapa();
        }
    }

    public void EvaluarIngredientes()
    {
        if (todosLosSlots == null)
            return;

        bool hayIncorrectos = false;

        foreach (InventorySlot slot in todosLosSlots)
        {
            if (slot == null || slot.EsMesa)
                continue;

            IngredienteData ingrediente = slot.GetComponentInChildren<IngredienteData>();

            if (ingrediente != null && ingrediente.tipo != slot.TipoAceptado)
            {
                hayIncorrectos = true;
                Debug.LogWarning($"❌ Ingrediente incorrecto: {ingrediente.nombreIngrediente} en el estante de {slot.TipoAceptado}");
                break;
            }
        }

        if (hayIncorrectos)
        {
            GameManager.Instance?.RegistrarIngredientesMalOrdenados();
        }
    }

    // =========================================================
    // TERMINAR ETAPA
    // =========================================================

    private void TerminarEtapa()
    {
        // Marcar terminada y desuscribir para no colisionar con SeleccionRecetaManager
        faseCompletada = true;
        DraggableItem.OnAnyItemEndDrag -= OnItemTerminoDeMoverse;

        SetBotonContinuar(false);
        GameManager.Instance?.ContinuarDespuesDelGuardado();
    }

    // =========================================================
    // UI
    // =========================================================

    private void SetBotonContinuar(bool visible, UnityAction accion = null)
    {
        if (botonContinuar == null)
        {
            Debug.LogWarning("⚠️ GuardadoAlimentosManager: botonContinuar no está asignado.");
            return;
        }

        botonContinuar.onClick.RemoveAllListeners();

        if (visible && accion != null)
        {
            botonContinuar.onClick.AddListener(accion);
        }

        botonContinuar.interactable = visible;
        botonContinuar.gameObject.SetActive(visible);
    }
}