using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

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

    public EstadoJuego estadoActual = EstadoJuego.OrdenandoIngredientes;

    [Header("Referencias de Sistemas")]
    [SerializeField] private ScoreData puntuacion;
    [SerializeField] private InicioDiaUI inicioDiaUI;
    [SerializeField] private string nombreEscenaMenu = "MenuPrincipal";

    public ScoreData Score => puntuacion;
    public bool ingredientesMalOrdenados => puntuacion != null && puntuacion.IngredientesMalOrdenados;
    public bool carneCruda => puntuacion != null && puntuacion.CarneCruda;
    public bool carneQuemada => puntuacion != null && puntuacion.CarneQuemada;
    public bool contaminacionCruzadaCortado => puntuacion != null && puntuacion.ContaminacionCruzadaCortado;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        MostrarInstruccionesIngredientes();
    }

    // =========================================================
    // REGISTRO DE ERRORES
    // =========================================================

    public void RegistrarIngredientesMalOrdenados()
    {
        puntuacion?.RegistrarIngredientesMalOrdenados();
    }

    public void RegistrarCarneCruda()
    {
        puntuacion?.RegistrarCarneCruda();
    }

    public void RegistrarCarneQuemada()
    {
        puntuacion?.RegistrarCarneQuemada();
    }

    public void RegistrarContaminacionCruzadaCortado()
    {
        puntuacion?.RegistrarContaminacionCruzadaCortado();
    }

    // =========================================================
    // ETAPA 1 - GUARDADO DE ALIMENTOS
    // =========================================================

    public void MostrarInstruccionesIngredientes()
    {
        UIManager.Instance?.OcultarBotonContinuar();

        estadoActual = EstadoJuego.OrdenandoIngredientes;

        if (PopupContenido.Instance != null)
            PopupContenido.Instance.MostrarInstruccionesIngredientes();
    }

    public void ContinuarDespuesDelGuardado()
    {
        MostrarInstruccionesReceta();
    }

    // =========================================================
    // ETAPA 2 - SELECCIÓN DE RECETA
    // =========================================================

    private void MostrarInstruccionesReceta()
    {
        UIManager.Instance?.OcultarBotonContinuar();

        estadoActual = EstadoJuego.SeleccionandoReceta;

        if (PopupContenido.Instance != null)
        {
            PopupContenido.Instance.MostrarInstruccionesReceta(
                EmpezarSeleccionReceta
            );
        }
        else
        {
            EmpezarSeleccionReceta();
        }
    }

    public void EmpezarSeleccionReceta()
    {
        estadoActual = EstadoJuego.SeleccionandoReceta;

        UIManager.Instance?.OcultarBotonContinuar();

        SeleccionRecetaManager.Instance?.IniciarSeleccion();
    }

    public void ActualizarEstadoSeleccionReceta(bool esValido)
    {
        if (estadoActual != EstadoJuego.SeleccionandoReceta)
            return;

        if (esValido)
        {
            UIManager.Instance?.PrepararBotonContinuar(
                ContinuarDesdeSeleccionReceta
            );
        }
        else
        {
            UIManager.Instance?.OcultarBotonContinuar();
        }
    }

    public void SeleccionRecetaCompleta()
    {
        ActualizarEstadoSeleccionReceta(true);
    }

    private void ContinuarDesdeSeleccionReceta()
    {
        SeleccionRecetaManager.Instance?.FinalizarSeleccion();

        UIManager.Instance?.OcultarBotonContinuar();

        ActivarLavado();
    }

    // =========================================================
    // ETAPAS RESTANTES
    // =========================================================

    public void ActivarLavado()
    {
        LavadoManos.Instance?.IniciarLavado();
    }

    public void ContinuarDespuesDelLavado()
    {
        ActivarCortado();
    }

    public void ActivarCortado()
    {
        CortadoManager.Instance?.IniciarCortado();
    }

    public void ContinuarDespuesDelCortado()
    {
        if (PopupContenido.Instance != null)
        {
            PopupContenido.Instance.MostrarFeedbackCortado(
                contaminacionCruzadaCortado,
                ActivarCoccion
            );
        }
        else
        {
            ActivarCoccion();
        }
    }

    // =========================================================
    // ETAPA - COCCIÓN
    // =========================================================

    // En GameManager.cs
public void ActivarCoccion()
{
    estadoActual = EstadoJuego.Coccion;
    Debug.Log("[Coccion] ActivarCoccion ejecutado");

    if (PopupContenido.Instance != null)
    {
        Debug.Log("[Coccion] Mostrando popup de instrucciones...");
        PopupContenido.Instance.MostrarInstruccionesCoccion(IniciarCoccion);
    }
    else
    {
        Debug.LogWarning("[Coccion] PopupContenido es nulo, iniciando directo");
        IniciarCoccion();
    }
}

private void IniciarCoccion()
{
    Debug.Log("[Coccion] Callback IniciarCoccion recibido");
    CoccionManager.Instance?.IniciarCoccion();
}

    public void ContinuarDespuesDeCoccion()
    {
        ActivarEmplatado();
    }

    // =========================================================
    // ETAPA - EMPLATADO
    // =========================================================

    public void ActivarEmplatado()
    {
        EmplatadoManager.Instance?.IniciarEmplatado();
    }

    public void ContinuarDespuesDelEmplatado()
    {
        FinalizarPartida();
    }

    // =========================================================
    // FINALIZACIÓN Y MENÚ
    // =========================================================

    private void FinalizarPartida()
    {
        estadoActual = EstadoJuego.Final;

        bool gano = puntuacion != null && puntuacion.EsVictoria();

        UIManager.Instance?.MostrarResumenFinal(gano);
    }

    public void VolverAlMenu()
    {
        SceneLoader.CargarEscena(nombreEscenaMenu);
    }
}