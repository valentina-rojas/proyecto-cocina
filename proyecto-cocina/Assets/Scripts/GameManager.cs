using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using System.Collections;

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
    // BOTÓN SIGUIENTE
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

    [Header("Configuración de la Escena")]
    public string nombreEscenaMenu = "MenuPrincipal";


    // =========================================================
    // ERRORES DE LA PARTIDA
    // =========================================================

    [HideInInspector]
    public bool ingredientesMalOrdenados;

    [HideInInspector]
    public bool carneCruda;

    [HideInInspector]
    public bool carneQuemada;


    // =========================================================
    // VARIABLES INTERNAS
    // =========================================================

    private InventorySlot[] todosLosSlots;

    private bool diaMostrado = false;


    // =========================================================
    // AWAKE
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


    // =========================================================
    // START
    // =========================================================

    private void Start()
    {
        todosLosSlots =
            FindObjectsByType<InventorySlot>(FindObjectsSortMode.None);


        // -----------------------------------------
        // PANEL DE RESUMEN
        // -----------------------------------------

        if (panelResumen != null)
            panelResumen.SetActive(false);


        // -----------------------------------------
        // BOTÓN SIGUIENTE
        // -----------------------------------------

        OcultarBotonContinuar();


        // -----------------------------------------
        // BOTÓN MENÚ PRINCIPAL
        // -----------------------------------------

        if (botonMenuPrincipal != null)
        {
            botonMenuPrincipal.onClick.RemoveAllListeners();
            botonMenuPrincipal.onClick.AddListener(VolverAlMenu);
        }


        // -----------------------------------------
        // COMPROBAR MESA
        // -----------------------------------------

        ChequearEstadoMesa();


        // -----------------------------------------
        // PRIMERAS INSTRUCCIONES
        // -----------------------------------------

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


        // Quitamos cualquier función anterior.

        botonContinuar.onClick.RemoveAllListeners();


        // Asignamos la nueva función.

        botonContinuar.onClick.AddListener(accion);


        // Mostramos el botón.

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
    // INGREDIENTES
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


    // =========================================================
    // DETECTAR CUANDO SE MUEVE UN INGREDIENTE
    // =========================================================

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
            StartCoroutine(ChequearMesaAlFinalDelFrame());
        }
    }


    private IEnumerator ChequearMesaAlFinalDelFrame()
    {
        yield return new WaitForEndOfFrame();

        ChequearEstadoMesa();
    }


    // =========================================================
    // COMPROBAR INGREDIENTES EN LA MESA
    // =========================================================

    public void ChequearEstadoMesa()
    {
        if (todosLosSlots == null)
            return;


        int itemsEnMesa = 0;


        foreach (InventorySlot slot in todosLosSlots)
        {
            if (slot != null &&
                slot.tipoDeEstanteAceptado == "mesa")
            {
                itemsEnMesa += slot.transform.childCount;
            }
        }


        if (diaMostrado)
            return;


        /*
         * Si no quedan ingredientes en la mesa,
         * se habilita el botón Siguiente.
         */

        if (itemsEnMesa == 0)
        {
            PrepararBotonContinuar(
                MostrarFeedbackIngredientes
            );
        }
        else
        {
            OcultarBotonContinuar();
        }
    }


    // =========================================================
    // FEEDBACK INGREDIENTES
    // =========================================================

    private void MostrarFeedbackIngredientes()
    {
        OcultarBotonContinuar();


        EvaluarIngredientes();


        diaMostrado = true;


        bool correcto =
            !ingredientesMalOrdenados;


        if (PopupContenido.Instance != null)
        {
            PopupContenido.Instance.MostrarFeedbackIngredientes(
                correcto,
                MostrarInstruccionesReceta
            );
        }
    }


    // =========================================================
    // INSTRUCCIONES DE RECETA
    // =========================================================

    private void MostrarInstruccionesReceta()
    {
        OcultarBotonContinuar();


        estadoActual =
            EstadoJuego.SeleccionandoReceta;


        if (PopupContenido.Instance != null)
        {
            PopupContenido.Instance.MostrarInstruccionesReceta(
                IniciarSeleccionReceta
            );
        }
    }


    // =========================================================
    // INICIAR SELECCIÓN DE RECETA
    // =========================================================

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


    // =========================================================
    // SELECCIÓN DE RECETA
    // =========================================================

    public void EmpezarSeleccionReceta()
    {
        Debug.Log(
            "Iniciando selección de receta."
        );


        estadoActual =
            EstadoJuego.SeleccionandoReceta;


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


    // =========================================================
    // LAVADO DE MANOS
    // =========================================================

    public void ActivarLavado()
    {
        OcultarBotonContinuar();


        estadoActual =
            EstadoJuego.Lavado;


        Debug.Log(
            "Estado: Lavado."
        );


        if (PopupContenido.Instance != null)
        {
            PopupContenido.Instance.MostrarInstruccionesLavado(
                () =>
                {
                    if (CameraManager.Instance != null)
                    {
                        CameraManager.Instance
                            .MostrarCamaraLavadoManos();
                    }
                    else
                    {
                        Debug.LogError(
                            "GameManager: No existe CameraManager."
                        );
                    }
                }
            );
        }
    }


    // =========================================================
    // LAVADO COMPLETO
    // =========================================================
    //
    // IMPORTANTE:
    // Acá NO aparece el feedback.
    //
    // Solo se habilita el mismo botón Siguiente.
    // =========================================================

    public void LavadoManosCompleto()
    {
        Debug.Log(
            "Lavado completado."
        );


        PrepararBotonContinuar(
            ContinuarDesdeLavado
        );
    }


    // =========================================================
    // CONTINUAR DESDE LAVADO
    // =========================================================

    private void ContinuarDesdeLavado()
    {
        OcultarBotonContinuar();


        Debug.Log(
            "Mostrando feedback del lavado."
        );


        if (PopupContenido.Instance != null)
        {
            PopupContenido.Instance.MostrarFeedbackLavado(
                MostrarInstruccionesCortado
            );
        }
    }


    // =========================================================
    // CORTADO
    // =========================================================

    private void MostrarInstruccionesCortado()
    {
        OcultarBotonContinuar();


        estadoActual =
            EstadoJuego.Cortado;


        if (PopupContenido.Instance != null)
        {
            PopupContenido.Instance.MostrarInstruccionesCortado(
                () =>
                {
                    if (CameraManager.Instance != null)
                    {
                        CameraManager.Instance
                            .MostrarCamaraCortadoIngredientes();
                    }
                    else
                    {
                        Debug.LogError(
                            "GameManager: No existe CameraManager."
                        );
                    }
                }
            );
        }
    }


    public void ActivarCortado()
    {
        MostrarInstruccionesCortado();
    }


    // =========================================================
    // CORTADO COMPLETO
    // =========================================================

    public void CortadoCompleto()
    {
        Debug.Log(
            "Cortado completado."
        );


        // No mostramos feedback todavía.

        PrepararBotonContinuar(
            ContinuarDesdeCortado
        );
    }


    // =========================================================
    // CONTINUAR DESDE CORTADO
    // =========================================================

    private void ContinuarDesdeCortado()
    {
        OcultarBotonContinuar();


        Debug.Log(
            "Mostrando feedback del cortado."
        );


        if (PopupContenido.Instance != null)
        {
            PopupContenido.Instance.MostrarFeedbackCortado(
                MostrarInstruccionesCoccion
            );
        }
    }


    // =========================================================
    // COCCIÓN
    // =========================================================

    private void MostrarInstruccionesCoccion()
    {
        OcultarBotonContinuar();


        estadoActual =
            EstadoJuego.Coccion;


        if (PopupContenido.Instance != null)
        {
            PopupContenido.Instance.MostrarInstruccionesCoccion(
                () =>
                {
                    if (CameraManager.Instance != null)
                    {
                        CameraManager.Instance
                            .MostrarCamaraCoccionIngredientes();
                    }
                    else
                    {
                        Debug.LogError(
                            "GameManager: No existe CameraManager."
                        );
                    }
                }
            );
        }
    }


    public void ActivarCoccion()
    {
        MostrarInstruccionesCoccion();
    }


    // =========================================================
    // COCCIÓN COMPLETA
    // =========================================================

    public void CoccionCompleta()
    {
        Debug.Log(
            "Cocción completada."
        );


        // No mostramos feedback automáticamente.

        PrepararBotonContinuar(
            ContinuarDesdeCoccion
        );
    }


    // =========================================================
    // CONTINUAR DESDE COCCIÓN
    // =========================================================

    private void ContinuarDesdeCoccion()
    {
        OcultarBotonContinuar();


        Debug.Log(
            "Mostrando feedback de la cocción."
        );


        if (PopupContenido.Instance != null)
        {
            PopupContenido.Instance.MostrarFeedbackCoccion(
                carneCruda,
                carneQuemada,
                MostrarInstruccionesEmplatado
            );
        }
    }


    // =========================================================
    // EMPLATADO
    // =========================================================

    private void MostrarInstruccionesEmplatado()
    {
        OcultarBotonContinuar();


        estadoActual =
            EstadoJuego.Emplatado;


        if (PopupContenido.Instance != null)
        {
            PopupContenido.Instance.MostrarInstruccionesEmplatado(
                () =>
                {
                    if (CameraManager.Instance != null)
                    {
                        CameraManager.Instance
                            .MostrarCamaraEmplatado();
                    }
                    else
                    {
                        Debug.LogError(
                            "GameManager: No existe CameraManager."
                        );
                    }
                }
            );
        }
    }


    public void ActivarEmplatado()
    {
        MostrarInstruccionesEmplatado();
    }


    // =========================================================
    // EMPLATADO COMPLETO
    // =========================================================

    public void EmplatadoCompleto()
    {
        estadoActual =
            EstadoJuego.Final;


        Debug.Log(
            "Emplatado completado."
        );


        // El feedback NO aparece todavía.

        PrepararBotonContinuar(
            ContinuarDesdeEmplatado
        );
    }


    // =========================================================
    // CONTINUAR DESDE EMPLATADO
    // =========================================================

    private void ContinuarDesdeEmplatado()
    {
        OcultarBotonContinuar();


        Debug.Log(
            "Mostrando feedback del emplatado."
        );


        if (PopupContenido.Instance != null)
        {
            PopupContenido.Instance.MostrarFeedbackEmplatado(
                ActivarBotonResumen
            );
        }
    }


    // =========================================================
    // BOTÓN DE RESUMEN FINAL
    // =========================================================

    private void ActivarBotonResumen()
    {
        /*
         * El botón Siguiente ya no se utiliza acá.
         * Mostramos el botón de resumen final.
         */

        if (botonContinuar != null)
            OcultarBotonContinuar();


        if (panelResumen != null)
            panelResumen.SetActive(false);


        /*
         * Si tenés un botón específico para mostrar
         * el resumen, se habilita desde acá.
         *
         * NOTA:
         * Si eliminaste botonMostrarResumen del Inspector,
         * podés mostrar directamente el resumen.
         */

        MostrarResumenFinal();
    }


    // =========================================================
    // REGISTRAR INGREDIENTES MAL ORDENADOS
    // =========================================================

    public void RegistrarIngredientesMalOrdenados()
    {
        ingredientesMalOrdenados = true;


        Debug.Log(
            "GameManager: Se registraron ingredientes mal ordenados."
        );
    }


    // =========================================================
    // REGISTRAR CARNE CRUDA
    // =========================================================

    public void RegistrarCarneCruda()
    {
        carneCruda = true;


        Debug.Log(
            "GameManager: Se registró carne cruda."
        );
    }


    // =========================================================
    // REGISTRAR CARNE QUEMADA
    // =========================================================

    public void RegistrarCarneQuemada()
    {
        carneQuemada = true;


        Debug.Log(
            "GameManager: Se registró carne quemada."
        );
    }


    // =========================================================
    // COMPROBAR SI GANÓ
    // =========================================================

    private bool GanoPartida()
    {
        return
            !ingredientesMalOrdenados &&
            !carneCruda &&
            !carneQuemada;
    }


    // =========================================================
    // RESUMEN FINAL
    // =========================================================

    private void MostrarResumenFinal()
    {
        if (imagenResultado != null)
        {
            if (GanoPartida())
            {
                imagenResultado.sprite =
                    spriteVictoria;
            }
            else
            {
                imagenResultado.sprite =
                    spriteDerrota;
            }
        }


        if (panelResumen != null)
            panelResumen.SetActive(true);
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


    // =========================================================
    // EVALUAR INGREDIENTES
    // =========================================================

    public void EvaluarIngredientes()
    {
        int incorrectos = 0;


        if (todosLosSlots == null)
            return;


        foreach (InventorySlot slot in todosLosSlots)
        {
            if (slot == null)
                continue;


            IngredienteData ingrediente =
                slot.GetComponentInChildren<IngredienteData>();


            if (ingrediente != null &&
                ingrediente.tipoIngrediente !=
                slot.tipoDeEstanteAceptado)
            {
                incorrectos++;
            }
        }


        if (incorrectos > 0)
        {
            RegistrarIngredientesMalOrdenados();


            Debug.Log(
                "❌ Ingredientes mal ordenados: " +
                incorrectos
            );
        }
        else
        {
            Debug.Log(
                "✅ Ingredientes ordenados correctamente."
            );
        }
    }
}