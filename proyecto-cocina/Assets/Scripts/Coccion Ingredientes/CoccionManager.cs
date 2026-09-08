using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class CoccionManager : MonoBehaviour
{
    public static CoccionManager Instance;

    [Header("Referencias")]
    [SerializeField] private Carne carne;

    [Header("Botón Siguiente")]
    [SerializeField] private Button botonContinuar;

    private bool coccionCompletada = false;

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
        OcultarBoton();
    }


    // =========================================================
    // INICIAR ETAPA
    // =========================================================

    public void IniciarCoccion()
    {
        coccionCompletada = false;

        OcultarBoton();

        if (GameManager.Instance != null)
        {
            GameManager.Instance.estadoActual =
                GameManager.EstadoJuego.Coccion;
        }

        Debug.Log(
            "CoccionManager: Iniciando etapa de cocción."
        );

        if (PopupContenido.Instance != null)
        {
            PopupContenido.Instance.MostrarInstruccionesCoccion(
                ActivarCamaraCoccion
            );
        }
        else
        {
            Debug.LogError(
                "CoccionManager: No existe PopupContenido."
            );

            ActivarCamaraCoccion();
        }
    }


    // =========================================================
    // CÁMARA
    // =========================================================

    private void ActivarCamaraCoccion()
    {
        if (CameraManager.Instance != null)
        {
            CameraManager.Instance
                .MostrarCamaraCoccionIngredientes();
        }
        else
        {
            Debug.LogError(
                "CoccionManager: No existe CameraManager."
            );
        }
    }


    // =========================================================
    // COCCIÓN COMPLETADA
    // =========================================================

    public void CoccionCompleta()
    {
        if (coccionCompletada)
            return;

        coccionCompletada = true;

        Debug.Log(
            "CoccionManager: Cocción completada."
        );

        // Primero aparece el botón Siguiente.
        // El feedback NO aparece automáticamente.
        PrepararBoton(
            ContinuarDesdeCoccion
        );
    }


    // =========================================================
    // BOTÓN SIGUIENTE
    // =========================================================

    private void PrepararBoton(UnityAction accion)
    {
        if (botonContinuar == null)
        {
            Debug.LogError(
                "CoccionManager: No está asignado el botón Siguiente."
            );

            return;
        }

        botonContinuar.onClick.RemoveAllListeners();

        botonContinuar.onClick.AddListener(accion);

        botonContinuar.gameObject.SetActive(true);

        botonContinuar.interactable = true;

        Debug.Log(
            "CoccionManager: Botón Siguiente habilitado."
        );
    }


    private void OcultarBoton()
    {
        if (botonContinuar == null)
            return;

        botonContinuar.onClick.RemoveAllListeners();

        botonContinuar.interactable = false;

        botonContinuar.gameObject.SetActive(false);
    }


    // =========================================================
    // FEEDBACK
    // =========================================================

    private void ContinuarDesdeCoccion()
    {
        OcultarBoton();

        Debug.Log(
            "CoccionManager: Mostrando feedback de la cocción."
        );

        bool carneEstaCruda = false;
        bool carneEstaQuemada = false;

        if (carne != null)
        {
            carneEstaCruda =
                carne.estado == Carne.Estado.Cruda;

            carneEstaQuemada =
                carne.estado == Carne.Estado.Quemada;
        }

        if (PopupContenido.Instance != null)
        {
            PopupContenido.Instance.MostrarFeedbackCoccion(
                carneEstaCruda,
                carneEstaQuemada,
                TerminarEtapaCoccion
            );
        }
        else
        {
            Debug.LogError(
                "CoccionManager: No existe PopupContenido."
            );

            TerminarEtapaCoccion();
        }
    }


    // =========================================================
    // TERMINAR ETAPA
    // =========================================================

    private void TerminarEtapaCoccion()
    {
        OcultarBoton();

        Debug.Log(
            "CoccionManager: Etapa de cocción finalizada."
        );

        if (GameManager.Instance != null)
        {
            GameManager.Instance.ContinuarDespuesDeCoccion();
        }
        else
        {
            Debug.LogError(
                "CoccionManager: No existe GameManager."
            );
        }
    }
}