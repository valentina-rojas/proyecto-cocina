using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Botones de avance")]
    public Button botonIrALavado;
    public Button botonIrACortado;
    public Button botonMostrarResumen;

    [Header("UI: Panel de Resumen")]
    public GameObject panelResumen;
    public TextMeshProUGUI textoResumen;
    public Button botonMenuPrincipal;

    [Header("Configuración de la Escena")]
    public string nombreEscenaMenu = "MenuPrincipal";

    private InventorySlot[] todosLosSlots;

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
        ChequearEstadoMesa();
    }

    /// <summary>
    /// Comprueba si ya no quedan ingredientes sobre la mesa.
    /// Si es así, habilita el botón para pasar al lavado.
    /// </summary>
    public void ChequearEstadoMesa()
    {
        int itemsEnMesa = 0;

        foreach (InventorySlot slot in todosLosSlots)
        {
            if (slot.tipoDeEstanteAceptado == "mesa")
            {
                itemsEnMesa += slot.transform.childCount;
            }
        }

        if (botonIrALavado != null)
            botonIrALavado.interactable = (itemsEnMesa == 0);
    }

    /// <summary>
    /// Llamar desde el minijuego de lavado cuando el jugador termine.
    /// </summary>
    public void LavadoManosCompleto()
    {
        if (botonIrACortado != null)
            botonIrACortado.interactable = true;
    }

    /// <summary>
    /// Llamar desde el minijuego de cortado cuando el jugador termine.
    /// </summary>
    public void CortadoCompleto()
    {
        if (botonMostrarResumen != null)
            botonMostrarResumen.interactable = true;
    }

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