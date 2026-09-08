using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

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


    // =========================================================
    // BOTÓN GENERAL
    // =========================================================

    [Header("Botón Siguiente")]
    [Tooltip("Único botón Siguiente del Canvas general.")]
    public Button botonContinuar;


    // =========================================================
    // INICIO DEL DÍA
    // =========================================================

    [Header("Inicio del día")]
    public InicioDiaUI inicioDiaUI;


    // =========================================================
    // RESULTADO FINAL
    // =========================================================

    [Header("UI: Resultado Final")]
    public GameObject panelResumen;

    public Image imagenResultado;

    public Sprite spriteVictoria;
    public Sprite spriteDerrota;

    public Button botonMenuPrincipal;


    // =========================================================
    // CONFIGURACIÓN
    // =========================================================

    [Header("Configuración de la Escena")]
    public string nombreEscenaMenu = "MenuPrincipal";


    // =========================================================
    // RESULTADOS DE LAS ETAPAS
    // =========================================================

    [HideInInspector]
    public bool ingredientesMalOrdenados;

    [HideInInspector]
    public bool carneCruda;

    [HideInInspector]
    public bool carneQuemada;


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
        if (panelResumen != null)
            panelResumen.SetActive(false);

        OcultarBotonContinuar();

        if (botonMenuPrincipal != null)
        {
            botonMenuPrincipal.onClick.RemoveAllListeners();
            botonMenuPrincipal.onClick.AddListener(VolverAlMenu);
        }

        // Comienza mostrando las instrucciones
        // para guardar los alimentos.
        MostrarInstruccionesIngredientes();
    }


    // =========================================================
    // BOTÓN SIGUIENTE
    // =========================================================

    private void PrepararBotonContinuar(UnityAction accion)
    {
        if (botonContinuar == null)
        {
            Debug.LogError(
                "GameManager: No está asignado el botonContinuar en el Inspector."
            );

            return;
        }

        botonContinuar.onClick.RemoveAllListeners();

        botonContinuar.onClick.AddListener(accion);

        botonContinuar.gameObject.SetActive(true);

        botonContinuar.interactable = true;

        Debug.Log(
            "GameManager: Botón Siguiente habilitado."
        );
    }


    private void OcultarBotonContinuar()
    {
        if (botonContinuar == null)
            return;

        botonContinuar.onClick.RemoveAllListeners();

        botonContinuar.interactable = false;

        botonContinuar.gameObject.SetActive(false);
    }


    // =========================================================
    // ETAPA 1 - GUARDADO DE ALIMENTOS
    // =========================================================

    private void MostrarInstruccionesIngredientes()
    {
        OcultarBotonContinuar();

        estadoActual = EstadoJuego.OrdenandoIngredientes;

        if (PopupContenido.Instance != null)
        {
            PopupContenido.Instance.MostrarInstruccionesIngredientes();
        }
        else
        {
            Debug.LogError(
                "GameManager: No existe PopupContenido en la escena."
            );
        }
    }


    public void ContinuarDespuesDelGuardado()
    {
        Debug.Log(
            "GameManager: Guardado de alimentos finalizado."
        );

        MostrarInstruccionesReceta();
    }


    // =========================================================
    // ETAPA 2 - SELECCIÓN DE RECETA
    // =========================================================

    private void MostrarInstruccionesReceta()
    {
        OcultarBotonContinuar();

        estadoActual = EstadoJuego.SeleccionandoReceta;

        if (PopupContenido.Instance != null)
        {
            PopupContenido.Instance.MostrarInstruccionesReceta(
                IniciarSeleccionReceta
            );
        }
        else
        {
            Debug.LogError(
                "GameManager: No existe PopupContenido en la escena."
            );
        }
    }


    private void IniciarSeleccionReceta()
    {
        Debug.Log(
            "Estado: Seleccionando receta."
        );

        if (inicioDiaUI != null)
        {
            inicioDiaUI.MostrarPanel();
        }
        else
        {
            Debug.LogError(
                "GameManager: Falta asignar InicioDiaUI."
            );
        }
    }


    public void EmpezarSeleccionReceta()
    {
        Debug.Log(
            "Iniciando selección de receta."
        );

        estadoActual = EstadoJuego.SeleccionandoReceta;

        if (SeleccionRecetaManager.Instance != null)
        {
            SeleccionRecetaManager.Instance.IniciarSeleccion();
        }
        else
        {
            Debug.LogError(
                "GameManager: No existe SeleccionRecetaManager."
            );
        }
    }


    public void SeleccionRecetaCompleta()
    {
        Debug.Log(
            "GameManager: Selección de receta completada."
        );

        // El feedback no aparece automáticamente.
        // Primero se muestra el botón Siguiente.
        PrepararBotonContinuar(
            ContinuarDesdeSeleccionReceta
        );
    }


    private void ContinuarDesdeSeleccionReceta()
    {
        OcultarBotonContinuar();

        Debug.Log(
            "GameManager: Continuando después de seleccionar receta."
        );

        ActivarLavado();
    }


    // =========================================================
    // ETAPA 3 - LAVADO DE MANOS
    // =========================================================

    public void ActivarLavado()
    {
        if (LavadoManos.Instance != null)
        {
            LavadoManos.Instance.IniciarLavado();
        }
        else
        {
            Debug.LogError(
                "GameManager: No existe LavadoManos en la escena."
            );
        }
    }


    public void ContinuarDespuesDelLavado()
    {
        Debug.Log(
            "GameManager: Lavado de manos finalizado."
        );

        ActivarCortado();
    }

    // =========================================================
    // ETAPA 4 - CORTADO
    // =========================================================

    public void ActivarCortado()
    {
        if (CortadoManager.Instance != null)
        {
            CortadoManager.Instance.IniciarCortado();
        }
        else
        {
            Debug.LogError(
                "GameManager: No existe CortadoManager en la escena."
            );
        }
    }


    public void ContinuarDespuesDelCortado()
    {
        Debug.Log(
            "GameManager: Cortado finalizado."
        );

        ActivarCoccion();
    }


    // =========================================================
    // ETAPA 5 - COCCIÓN
    // =========================================================

    public void ActivarCoccion()
    {
        if (CoccionManager.Instance != null)
        {
            CoccionManager.Instance.IniciarCoccion();
        }
        else
        {
            Debug.LogError(
                "GameManager: No existe CoccionManager en la escena."
            );
        }
    }


    public void ContinuarDespuesDeCoccion()
    {
        Debug.Log(
            "GameManager: Cocción finalizada."
        );

        ActivarEmplatado();
    }


    // =========================================================
    // ETAPA 6 - EMPLATADO
    // =========================================================

    public void ActivarEmplatado()
    {
        if (EmplatadoManager.Instance != null)
        {
            EmplatadoManager.Instance.IniciarEmplatado();
        }
        else
        {
            Debug.LogError(
                "GameManager: No existe EmplatadoManager en la escena."
            );
        }
    }


    public void ContinuarDespuesDelEmplatado()
    {
        Debug.Log(
            "GameManager: Emplatado finalizado."
        );

        MostrarResumenFinal();
    }

    // =========================================================
    // RESULTADO FINAL
    // =========================================================

    private void ActivarBotonResumen()
    {
        OcultarBotonContinuar();

        if (panelResumen != null)
            panelResumen.SetActive(false);

        MostrarResumenFinal();
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
            imagenResultado.sprite =
                GanoPartida()
                    ? spriteVictoria
                    : spriteDerrota;
        }

        if (panelResumen != null)
            panelResumen.SetActive(true);
    }


    // =========================================================
    // REGISTRO DE ERRORES
    // =========================================================

    public void RegistrarIngredientesMalOrdenados()
    {
        ingredientesMalOrdenados = true;

        Debug.Log(
            "GameManager: Se registraron ingredientes mal ordenados."
        );
    }


    public void RegistrarCarneCruda()
    {
        carneCruda = true;

        Debug.Log(
            "GameManager: Se registró carne cruda."
        );
    }


    public void RegistrarCarneQuemada()
    {
        carneQuemada = true;

        Debug.Log(
            "GameManager: Se registró carne quemada."
        );
    }


    // =========================================================
    // VOLVER AL MENÚ
    // =========================================================

    private void VolverAlMenu()
    {
        Time.timeScale = 1f;

        SceneManager.LoadScene(
            nombreEscenaMenu
        );
    }
}