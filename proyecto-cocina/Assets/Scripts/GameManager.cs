using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public enum EstadoJuego
    {
        OrdenandoIngredientes,
        SeleccionandoReceta,
        Lavado,
        Cortado,
        Final
    }

    [HideInInspector]
    public EstadoJuego estadoActual = EstadoJuego.OrdenandoIngredientes;

    [Header("Botón Continuar")]
    public Button botonContinuar;

    [Header("Botones de avance")]
    public Button botonIrALavado;
    public Button botonIrACortado;
    public Button botonMostrarResumen;

    [Header("Inicio del día")]
    public InicioDiaUI inicioDiaUI;

    [Header("UI: Panel de Resumen")]
    public GameObject panelResumen;
    public TextMeshProUGUI textoResumen;
    public Button botonMenuPrincipal;

    [Header("Configuración de la Escena")]
    public string nombreEscenaMenu = "MenuPrincipal";

    private InventorySlot[] todosLosSlots;
    private bool diaMostrado = false;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        todosLosSlots = FindObjectsByType<InventorySlot>(FindObjectsSortMode.None);

        if (panelResumen != null)
            panelResumen.SetActive(false);

        if (botonContinuar != null)
        {
            botonContinuar.interactable = false;
            botonContinuar.onClick.AddListener(MostrarInicioDia);
        }

        if (botonIrALavado != null)
        {
            botonIrALavado.interactable = false;
            botonIrALavado.onClick.AddListener(CameraManager.Instance.MostrarCamaraLavadoManos);
        }

        if (botonIrACortado != null)
        {
            botonIrACortado.interactable = false;
            botonIrACortado.onClick.AddListener(CameraManager.Instance.MostrarCamaraCortadoIngredientes);
        }

        if (botonMostrarResumen != null)
        {
            botonMostrarResumen.interactable = false;
            botonMostrarResumen.onClick.AddListener(MostrarResumenFinal);
        }

        if (botonMenuPrincipal != null)
        {
            botonMenuPrincipal.onClick.AddListener(VolverAlMenu);
        }

        ChequearEstadoMesa();
    }

    private void OnEnable()
    {
        DraggableItem.OnAnyItemEndDrag += OnItemDroppedNotification;
    }

    private void OnDisable()
    {
        DraggableItem.OnAnyItemEndDrag -= OnItemDroppedNotification;
    }

    private void OnItemDroppedNotification()
    {
        if (estadoActual == EstadoJuego.OrdenandoIngredientes)
        {
            ChequearEstadoMesa();
        }
    }

    //====================================================
    // ORDENAR INGREDIENTES
    //====================================================

    public void ChequearEstadoMesa()
    {
        int itemsEnMesa = 0;

        foreach (InventorySlot slot in todosLosSlots)
        {
            if (slot.tipoDeEstanteAceptado == "mesa")
                itemsEnMesa += slot.transform.childCount;
        }

        if (diaMostrado)
            return;

        if (botonContinuar != null)
        {
            botonContinuar.interactable = (itemsEnMesa == 0);
        }
    }

    private void MostrarInicioDia()
    {
        diaMostrado = true;

        if (botonContinuar != null)
            botonContinuar.interactable = false;

        if (inicioDiaUI != null)
            botonContinuar.gameObject.SetActive(false);
            botonIrALavado.gameObject.SetActive(true);
            inicioDiaUI.MostrarPanel();
    }

    //====================================================
    // SELECCIÓN DE RECETA
    //====================================================

    public void EmpezarSeleccionReceta()
    {
        estadoActual = EstadoJuego.SeleccionandoReceta;

        Debug.Log("Estado: Seleccionando receta");

        SeleccionRecetaManager.Instance.IniciarSeleccion();
    }

    public void ActivarLavado()
    {
        estadoActual = EstadoJuego.Lavado;

        Debug.Log("Estado: Lavado");

        if (botonIrALavado != null)
            botonIrALavado.interactable = true;
    }

    //====================================================
    // LAVADO
    //====================================================

    public void LavadoManosCompleto()
    {
        estadoActual = EstadoJuego.Cortado;

        Debug.Log("Lavado completado");

        if (botonIrACortado != null)
            botonIrACortado.interactable = true;
    }

    //====================================================
    // CORTADO
    //====================================================

    public void CortadoCompleto()
    {
        estadoActual = EstadoJuego.Final;

        Debug.Log("Cortado completado");

        if (botonMostrarResumen != null)
            botonMostrarResumen.interactable = true;
    }

    //====================================================
    // RESUMEN FINAL
    //====================================================

    private void MostrarResumenFinal()
    {
        int correctos = 0;
        int incorrectos = 0;

        foreach (InventorySlot slot in todosLosSlots)
        {
            IngredienteData ingrediente = slot.GetComponentInChildren<IngredienteData>();

            if (ingrediente != null)
            {
                if (ingrediente.tipoIngrediente == slot.tipoDeEstanteAceptado)
                    correctos++;
                else
                    incorrectos++;
            }
        }

        if (textoResumen != null)
        {
            textoResumen.text =
                $"Ingredientes Correctos: <color=green>{correctos}</color>\n" +
                $"Ingredientes Incorrectos: <color=red>{incorrectos}</color>";
        }

        if (panelResumen != null)
            panelResumen.SetActive(true);
    }

    private void VolverAlMenu()
    {
        SceneManager.LoadScene(nombreEscenaMenu);
    }
}