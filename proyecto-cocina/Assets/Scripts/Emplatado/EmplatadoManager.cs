using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class EmplatadoManager : MonoBehaviour
{
    public static EmplatadoManager Instance;

    [Header("Botón Siguiente")]
    [SerializeField] private Button botonContinuar;

    private bool emplatadoCompletado = false;


    // =========================================================
    // UNITY
    // =========================================================

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

    public void IniciarEmplatado()
    {
        emplatadoCompletado = false;

        OcultarBoton();

        if (GameManager.Instance != null)
        {
            GameManager.Instance.estadoActual =
                GameManager.EstadoJuego.Emplatado;
        }

        Debug.Log(
            "EmplatadoManager: Iniciando etapa de emplatado."
        );

        if (PopupContenido.Instance != null)
        {
            PopupContenido.Instance.MostrarInstruccionesEmplatado(
                ActivarCamaraEmplatado
            );
        }
        else
        {
            Debug.LogError(
                "EmplatadoManager: No existe PopupContenido."
            );

            ActivarCamaraEmplatado();
        }
    }


    // =========================================================
    // CÁMARA
    // =========================================================

    private void ActivarCamaraEmplatado()
    {
        if (CameraManager.Instance != null)
        {
            CameraManager.Instance
                .MostrarCamaraEmplatado();
        }
        else
        {
            Debug.LogError(
                "EmplatadoManager: No existe CameraManager."
            );
        }
    }


    // =========================================================
    // EMPLATADO COMPLETADO
    // =========================================================

    public void EmplatadoCompleto()
    {
        if (emplatadoCompletado)
            return;

        emplatadoCompletado = true;

        Debug.Log(
            "EmplatadoManager: Emplatado completado."
        );

        // Primero aparece el botón Siguiente.
        // El feedback NO aparece automáticamente.
        PrepararBoton(
            ContinuarDesdeEmplatado
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
                "EmplatadoManager: No está asignado el botón Siguiente."
            );

            return;
        }

        botonContinuar.onClick.RemoveAllListeners();

        botonContinuar.onClick.AddListener(accion);

        botonContinuar.gameObject.SetActive(true);

        botonContinuar.interactable = true;

        Debug.Log(
            "EmplatadoManager: Botón Siguiente habilitado."
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

    private void ContinuarDesdeEmplatado()
    {
        OcultarBoton();

        Debug.Log(
            "EmplatadoManager: Mostrando feedback del emplatado."
        );

        if (PopupContenido.Instance != null)
        {
            PopupContenido.Instance.MostrarFeedbackEmplatado(
                TerminarEtapaEmplatado
            );
        }
        else
        {
            Debug.LogError(
                "EmplatadoManager: No existe PopupContenido."
            );

            TerminarEtapaEmplatado();
        }
    }


    // =========================================================
    // TERMINAR ETAPA
    // =========================================================

    private void TerminarEtapaEmplatado()
    {
        OcultarBoton();

        Debug.Log(
            "EmplatadoManager: Etapa de emplatado finalizada."
        );

        if (GameManager.Instance != null)
        {
            GameManager.Instance.ContinuarDespuesDelEmplatado();
        }
        else
        {
            Debug.LogError(
                "EmplatadoManager: No existe GameManager."
            );
        }
    }
}