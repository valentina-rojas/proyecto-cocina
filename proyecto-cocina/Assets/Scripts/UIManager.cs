using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("Botón Continuar")]
    [SerializeField] private Button botonContinuar;

    [Header("Panel Resumen")]
    [SerializeField] private GameObject panelResumen;
    [SerializeField] private Image imagenResultado;
    [SerializeField] private Sprite spriteVictoria;
    [SerializeField] private Sprite spriteDerrota;
    [SerializeField] private Button botonMenuPrincipal;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        if (panelResumen != null) panelResumen.SetActive(false);
        OcultarBotonContinuar();
        
        if (botonMenuPrincipal != null)
            botonMenuPrincipal.onClick.AddListener(() => GameManager.Instance.VolverAlMenu());
    }

    public void PrepararBotonContinuar(UnityAction accion)
    {
        if (botonContinuar == null) return;
        botonContinuar.onClick.RemoveAllListeners();
        botonContinuar.onClick.AddListener(accion);
        botonContinuar.gameObject.SetActive(true);
        botonContinuar.interactable = true;
    }

    public void OcultarBotonContinuar()
    {
        if (botonContinuar == null) return;
        botonContinuar.onClick.RemoveAllListeners();
        botonContinuar.interactable = false;
        botonContinuar.gameObject.SetActive(false);
    }

    public void MostrarResumenFinal(bool esVictoria)
    {
        if (imagenResultado != null)
            imagenResultado.sprite = esVictoria ? spriteVictoria : spriteDerrota;

        if (panelResumen != null)
            panelResumen.SetActive(true);
    }
}