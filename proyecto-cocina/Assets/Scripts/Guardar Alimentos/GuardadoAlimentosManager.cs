using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GuardadoAlimentosManager : MonoBehaviour
{
    public static GuardadoAlimentosManager Instance;

    [Header("Botón Siguiente")]
    [SerializeField] private Button botonContinuar;

    private InventorySlot[] todosLosSlots;

    private bool guardadoCompletado = false;
    private bool feedbackMostrado = false;

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

    private void Start()
    {
        todosLosSlots =
            FindObjectsByType<InventorySlot>(FindObjectsSortMode.None);

        OcultarBoton();

        // Comprobamos el estado inicial
        ChequearEstadoGuardado();
    }

    private void OnEnable()
    {
        DraggableItem.OnAnyItemEndDrag += OnItemTerminoDeMoverse;
    }

    private void OnDisable()
    {
        DraggableItem.OnAnyItemEndDrag -= OnItemTerminoDeMoverse;
    }

    private void OnItemTerminoDeMoverse()
    {
        if (guardadoCompletado)
            return;

        StartCoroutine(ChequearAlFinalDelFrame());
    }

    private IEnumerator ChequearAlFinalDelFrame()
    {
        yield return new WaitForEndOfFrame();

        ChequearEstadoGuardado();
    }

    public void ChequearEstadoGuardado()
    {
        if (todosLosSlots == null)
            return;

        int alimentosEnMesa = 0;

        foreach (InventorySlot slot in todosLosSlots)
        {
            if (slot == null)
                continue;

            if (slot.tipoDeEstanteAceptado == "mesa")
            {
                alimentosEnMesa += slot.transform.childCount;
            }
        }

        Debug.Log("Alimentos restantes en mesa: " + alimentosEnMesa);

        if (alimentosEnMesa == 0)
        {
            CompletarGuardado();
        }
        else
        {
            OcultarBoton();
        }
    }

    private void CompletarGuardado()
    {
        if (guardadoCompletado)
            return;

        guardadoCompletado = true;

        Debug.Log("✅ Guardado de alimentos completado.");

        PrepararBoton(ContinuarDesdeGuardado);
    }

    private void ContinuarDesdeGuardado()
    {
        OcultarBoton();

        MostrarFeedback();
    }

    private void MostrarFeedback()
    {
        if (feedbackMostrado)
            return;

        feedbackMostrado = true;

        EvaluarIngredientes();

        bool correcto = !GameManager.Instance.ingredientesMalOrdenados;

        Debug.Log(
            correcto
                ? "✅ Todos los alimentos fueron guardados correctamente."
                : "❌ Hay alimentos mal guardados."
        );

        if (PopupContenido.Instance != null)
        {
            PopupContenido.Instance.MostrarFeedbackIngredientes(
                correcto,
                ContinuarDespuesDelFeedback
            );
        }
        else
        {
            Debug.LogError(
                "GuardadoAlimentosManager: No existe PopupContenido."
            );

            ContinuarDespuesDelFeedback();
        }
    }

    private void ContinuarDespuesDelFeedback()
    {
        OcultarBoton();

        if (GameManager.Instance != null)
        {
            GameManager.Instance.ContinuarDespuesDelGuardado();
        }
    }

    public void EvaluarIngredientes()
    {
        int incorrectos = 0;

        if (todosLosSlots == null)
            return;

        foreach (InventorySlot slot in todosLosSlots)
        {
            if (slot == null)
                continue;

            IngredienteData ingrediente =
                slot.GetComponentInChildren<IngredienteData>();

            if (ingrediente != null &&
                ingrediente.tipoIngrediente != slot.tipoDeEstanteAceptado)
            {
                incorrectos++;

                Debug.LogWarning(
                    $"❌ {ingrediente.nombreIngrediente} está mal guardado. " +
                    $"Tipo: {ingrediente.tipoIngrediente} | " +
                    $"Estante: {slot.tipoDeEstanteAceptado}"
                );
            }
        }

        if (incorrectos > 0)
        {
            GameManager.Instance.RegistrarIngredientesMalOrdenados();
        }
        else
        {
            Debug.Log("✅ No se encontraron alimentos mal guardados.");
        }
    }

    private void PrepararBoton(UnityEngine.Events.UnityAction accion)
    {
        if (botonContinuar == null)
        {
            Debug.LogError(
                "GuardadoAlimentosManager: No está asignado el botón Siguiente."
            );
            return;
        }

        botonContinuar.onClick.RemoveAllListeners();
        botonContinuar.onClick.AddListener(accion);

        botonContinuar.gameObject.SetActive(true);
        botonContinuar.interactable = true;
    }

    private void OcultarBoton()
    {
        if (botonContinuar == null)
            return;

        botonContinuar.onClick.RemoveAllListeners();
        botonContinuar.interactable = false;
        botonContinuar.gameObject.SetActive(false);
    }
}