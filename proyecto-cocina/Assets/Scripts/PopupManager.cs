using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

public class PopupManager : MonoBehaviour
{
    public static PopupManager Instance;

    [Header("UI")]
    public GameObject panelPopup;
    public TextMeshProUGUI titulo;
    public TextMeshProUGUI descripcion;
    public Button botonContinuar;

    private UnityAction accionAlCerrar;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        Time.timeScale = 1f;

        if (panelPopup != null)
            panelPopup.SetActive(false);

        if (botonContinuar != null)
        {
            botonContinuar.onClick.RemoveAllListeners();
            botonContinuar.onClick.AddListener(CerrarPopup);
        }
    }

    /// <summary>
    /// Muestra un popup y pausa el juego.
    /// </summary>
    public void MostrarPopup(string tituloTexto,
                             string descripcionTexto,
                             UnityAction accion = null)
    {
        accionAlCerrar = accion;

        if (titulo != null)
            titulo.text = tituloTexto;

        if (descripcion != null)
            descripcion.text = descripcionTexto;

        if (panelPopup != null)
            panelPopup.SetActive(true);

        Time.timeScale = 0f;
    }

    /// <summary>
    /// Cierra el popup, reanuda el juego y ejecuta la acción asignada.
    /// </summary>
    public void CerrarPopup()
    {
        if (panelPopup != null)
            panelPopup.SetActive(false);

        Time.timeScale = 1f;

        accionAlCerrar?.Invoke();
        accionAlCerrar = null;
    }

    /// <summary>
    /// Cierra el popup sin ejecutar ninguna acción.
    /// </summary>
    public void CerrarPopupSinAccion()
    {
        if (panelPopup != null)
            panelPopup.SetActive(false);

        Time.timeScale = 1f;
        accionAlCerrar = null;
    }

    private void OnDestroy()
    {
        // Evita que el juego quede pausado si este objeto se destruye.
        Time.timeScale = 1f;
    }
}