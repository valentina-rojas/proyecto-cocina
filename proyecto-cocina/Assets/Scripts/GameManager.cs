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

    // Mantiene 'estadoActual' en minúscula para no romper InventorySlot.cs
    public EstadoJuego estadoActual = EstadoJuego.OrdenandoIngredientes;

    [Header("Referencias de Sistemas")]
    [SerializeField] private ScoreData puntuacion;
    [SerializeField] private InicioDiaUI inicioDiaUI;
    [SerializeField] private string nombreEscenaMenu = "MenuPrincipal";

    // Puentes hacia ScoreData para resolver los errores de GuardadoAlimentosManager.cs
    public ScoreData Score => puntuacion;
    public bool ingredientesMalOrdenados => puntuacion != null && puntuacion.IngredientesMalOrdenados;
    public bool carneCruda => puntuacion != null && puntuacion.CarneCruda;
    public bool carneQuemada => puntuacion != null && puntuacion.CarneQuemada;

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
        MostrarInstruccionesIngredientes();
    }

    // =========================================================
    // REGISTRO DE ERRORES (Delegados a ScoreData)
    // =========================================================

    public void RegistrarIngredientesMalOrdenados()
    {
        puntuacion?.RegistrarIngredientesMalOrdenados();
        Debug.Log("GameManager: Se registraron ingredientes mal ordenados.");
    }

    public void RegistrarCarneCruda()
    {
        puntuacion?.RegistrarCarneCruda();
        Debug.Log("GameManager: Se registró carne cruda.");
    }

    public void RegistrarCarneQuemada()
    {
        puntuacion?.RegistrarCarneQuemada();
        Debug.Log("GameManager: Se registró carne quemada.");
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
        Debug.Log("GameManager: Guardado de alimentos finalizado.");
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
            PopupContenido.Instance.MostrarInstruccionesReceta(IniciarSeleccionReceta);
    }

    private void IniciarSeleccionReceta()
    {
        if (inicioDiaUI != null)
            inicioDiaUI.MostrarPanel();
    }

    public void EmpezarSeleccionReceta()
    {
        estadoActual = EstadoJuego.SeleccionandoReceta;
        SeleccionRecetaManager.Instance?.IniciarSeleccion();
    }

    public void SeleccionRecetaCompleta()
    {
        UIManager.Instance?.PrepararBotonContinuar(ContinuarDesdeSeleccionReceta);
    }

    private void ContinuarDesdeSeleccionReceta()
    {
        UIManager.Instance?.OcultarBotonContinuar();
        ActivarLavado();
    }

    // =========================================================
    // ETAPAS RESTANTES
    // =========================================================

    public void ActivarLavado() => LavadoManos.Instance?.IniciarLavado();
    public void ContinuarDespuesDelLavado() => ActivarCortado();

    public void ActivarCortado() => CortadoManager.Instance?.IniciarCortado();
    public void ContinuarDespuesDelCortado() => ActivarCoccion();

    public void ActivarCoccion() => CoccionManager.Instance?.IniciarCoccion();
    public void ContinuarDespuesDeCoccion() => ActivarEmplatado();

    public void ActivarEmplatado() => EmplatadoManager.Instance?.IniciarEmplatado();
    public void ContinuarDespuesDelEmplatado() => FinalizarPartida();

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