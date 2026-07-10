using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections; // Necesario para usar Corrutinas (IEnumerator)

public class GameManager : MonoBehaviour
{ 
    public static GameManager Instance;

    public enum EstadoJuego
    {
        OrdenandoIngredientes,
        SeleccionandoReceta,
        Lavado,
        Cortado,
        Coccion,
        Emplatado,
        Final
    }

    [HideInInspector]
    public EstadoJuego estadoActual = EstadoJuego.OrdenandoIngredientes;

    [Header("Botón Continuar")]
    public Button botonContinuar;

    [Header("Botones de avance")]
    public Button botonIrALavado;
    public Button botonIrACortado;
    public Button botonIrACoccion;
    public Button botonIrAEmplatado;
    public Button botonMostrarResumen;

    [Header("Inicio del día")]
    public InicioDiaUI inicioDiaUI;

    [Header("UI: Resultado Final")]
    public GameObject panelResumen;
    public Image imagenResultado;
    public Sprite spriteVictoria;
    public Sprite spriteDerrota;
    public Button botonMenuPrincipal;

    [Header("Configuración de la Escena")]
    public string nombreEscenaMenu = "MenuPrincipal";

    private InventorySlot[] todosLosSlots;
    private bool diaMostrado = false;

    [HideInInspector] public bool ingredientesMalOrdenados;
    [HideInInspector] public bool carneCruda;
    [HideInInspector] public bool carneQuemada;

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

       /* PopupManager.Instance.MostrarPopup(
    "Guardado de alimentos",
    "Almacena cada alimento en el lugar correspondiente.");*/

        if (panelResumen != null)
            panelResumen.SetActive(false);

        // Configuración segura del botón Continuar
        if (botonContinuar != null)
        {
            botonContinuar.onClick.RemoveAllListeners(); // Limpia cualquier residuo previo
            botonContinuar.onClick.AddListener(MostrarInicioDia);
            botonContinuar.interactable = false; 
            Debug.Log("GameManager: Botón Continuar configurado correctamente en código.");
        }
        else
        {
            Debug.LogError("GameManager: ¡No has arrastrado el Botón Continuar al Inspector!");
        }

        // Configuración de los botones de avance
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
 
        if (botonIrACoccion != null)
        {
            botonIrACoccion.interactable = false;
            botonIrACoccion.onClick.AddListener(CameraManager.Instance.MostrarCamaraCoccionIngredientes);
        }

        if (botonIrAEmplatado != null)
        {
            botonIrAEmplatado.interactable = false;
            botonIrAEmplatado.onClick.AddListener(CameraManager.Instance.MostrarCamaraEmplatado);
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
            // Esperamos un frame para que los objetos cambien de padre realmente en la jerarquía
            StartCoroutine(ChequearMesaAlFinalDelFrame());
        }
    }

    private IEnumerator ChequearMesaAlFinalDelFrame()
    {
        yield return new WaitForEndOfFrame();
        ChequearEstadoMesa();
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
            // Se activa SOLO si está vacía (0 items)
            botonContinuar.interactable = (itemsEnMesa == 0);
        }
    }

    private void MostrarInicioDia()
    {
        // Alerta en consola para verificar que Unity detecta el clic físico
        Debug.LogWarning("¡EL CLIC FUNCIONA! Ejecutando MostrarInicioDia().");

        EvaluarIngredientes();

        diaMostrado = true;

        if (botonContinuar != null)
        {
            botonContinuar.interactable = false;
            botonContinuar.gameObject.SetActive(false);
        }

        if (botonIrALavado != null)
        {
            botonIrALavado.gameObject.SetActive(true);
        }
        else
        {
            Debug.LogWarning("GameManager: 'botonIrALavado' no está asignado en el Inspector.");
        }

        if (inicioDiaUI != null)
        {
            inicioDiaUI.MostrarPanel();
        }
        else 
        {
            Debug.LogError("GameManager: ¡Falta asignar el componente 'InicioDiaUI' en el Inspector!");
        }
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
        estadoActual = EstadoJuego.Coccion;
        Debug.Log("Cortado completado");
        
        if (botonIrACoccion != null)
            botonIrACoccion.interactable = true;
    }

    //====================================================
    // COCCION
    //====================================================

    public void CoccionCompleta()
    {
        estadoActual = EstadoJuego.Emplatado;
        Debug.Log("GameManager: Cocción completada (Hornalla apagada). Estado cambiado a Emplatado.");

        if (botonIrAEmplatado != null)
        {
            botonIrAEmplatado.gameObject.SetActive(true); // Por si acaso estaba oculto
            botonIrAEmplatado.interactable = true;        // Habilita el clic
        }
        else
        {
            Debug.LogWarning("GameManager: El 'botonIrAEmplatado' no está asignado en el Inspector.");
        }
    }

    //====================================================
    // EMPLATADO
    //====================================================

    public void EmplatadoCompleto()
    {
        estadoActual = EstadoJuego.Final;
        Debug.Log("Emplatado completado");

        if (botonMostrarResumen != null)
            botonMostrarResumen.interactable = true;
    }

    //====================================================
    // RESUMEN FINAL
    //====================================================

    public void RegistrarIngredientesMalOrdenados()
    {
        ingredientesMalOrdenados = true;
    }

    public void RegistrarCarneCruda()
    {
        carneCruda = true;
    }

    public void RegistrarCarneQuemada()
    {
        carneQuemada = true;
    }

    private bool GanoPartida()
    {
        return !ingredientesMalOrdenados &&
            !carneCruda &&
            !carneQuemada;
    }

    private void MostrarResumenFinal()
    {
        if (imagenResultado != null)
        {
            imagenResultado.sprite = GanoPartida()
                ? spriteVictoria
                : spriteDerrota;
        }

        if (panelResumen != null)
        {
            panelResumen.SetActive(true);
        }
    }

    private void VolverAlMenu()
    {
        SceneManager.LoadScene(nombreEscenaMenu);
    }

    public void EvaluarIngredientes()
    {
        int incorrectos = 0;

        foreach (InventorySlot slot in todosLosSlots)
        {
            IngredienteData ingrediente = slot.GetComponentInChildren<IngredienteData>();

            if (ingrediente != null &&
                ingrediente.tipoIngrediente != slot.tipoDeEstanteAceptado)
            {
                incorrectos++;
            }
        }

        if (incorrectos > 0)
        {
            RegistrarIngredientesMalOrdenados();
            Debug.Log("❌ Ingredientes mal ordenados");
        }
        else
        {
            Debug.Log("✅ Ingredientes ordenados correctamente");
        }
    }
}