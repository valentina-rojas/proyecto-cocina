using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InicioDiaUI : MonoBehaviour
{
    public GameObject panelInicioDia;
    public TextMeshProUGUI textoReceta;
    public Image imagenReceta;

    public Button botonContinuar;

    private void Start()
    {
        panelInicioDia.SetActive(false);

        botonContinuar.onClick.AddListener(CerrarPanel);
    }


    public void MostrarPanel()
    {
        textoReceta.text =
            "Hoy tenés que preparar:\n\n" +
            DayManager.Instance.recetaActual;


        if (imagenReceta != null)
            imagenReceta.sprite = DayManager.Instance.imagenRecetaActual;


        panelInicioDia.SetActive(true);
    }


    private void CerrarPanel()
    {
        panelInicioDia.SetActive(false);

        // Habilita el botón para ir al lavado
        if (GameManager.Instance != null)
        {
            GameManager.Instance.EmpezarSeleccionReceta();
        }
    }
}