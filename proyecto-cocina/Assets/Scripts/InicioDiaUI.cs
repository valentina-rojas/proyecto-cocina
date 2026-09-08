using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InicioDiaUI : MonoBehaviour
{
    [Header("Panel")]
    public GameObject panelInicioDia;

    [Header("Información de la receta")]
    public TextMeshProUGUI textoReceta;
    public Image imagenReceta;

    [Header("Botón")]
    public Button botonContinuar;

    //====================================================
    // START
    //====================================================

    private void Start()
    {
        if (panelInicioDia != null)
        {
            panelInicioDia.SetActive(false);
        }

        if (botonContinuar != null)
        {
            botonContinuar.onClick.RemoveAllListeners();
            botonContinuar.onClick.AddListener(CerrarPanel);
        }
    }

    //====================================================
    // MOSTRAR PANEL
    //====================================================

    public void MostrarPanel()
    {
        if (DayManager.Instance == null)
        {
            Debug.LogError(
                "InicioDiaUI: No existe DayManager."
            );

            return;
        }

        if (textoReceta != null)
        {
            textoReceta.text =
                "Hoy tenés que preparar:\n\n" +
                DayManager.Instance.recetaActual;
        }

        if (imagenReceta != null)
        {
            imagenReceta.sprite =
                DayManager.Instance.imagenRecetaActual;
        }

        if (panelInicioDia != null)
        {
            panelInicioDia.SetActive(true);
        }
    }

    //====================================================
    // CERRAR PANEL
    //====================================================

    private void CerrarPanel()
    {
        if (panelInicioDia != null)
        {
            panelInicioDia.SetActive(false);
        }

        // Una vez que el jugador vio la receta,
        // comienza la selección de receta.
        if (GameManager.Instance != null)
        {
            GameManager.Instance.EmpezarSeleccionReceta();
        }
        else
        {
            Debug.LogError(
                "InicioDiaUI: No existe GameManager."
            );
        }
    }
}