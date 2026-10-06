using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public enum EstadoJuego
    {
        OrdenandoIngredientes,
        SeleccionandoReceta,
        LavadoVerduras,
        LavadoManos,
        Cortado,
        Mezclado,
        Coccion,
        Emplatado,
        Final
    }

    public EstadoJuego estadoActual = EstadoJuego.OrdenandoIngredientes;

    [Header("Referencias de Sistemas")]
    [SerializeField] private ScoreData puntuacion;
    [SerializeField] private InicioDiaUI inicioDiaUI;
    [SerializeField] private string nombreEscenaMenu = "MenuPrincipal";

    [Header("Condiciones de Derrota")]
    [SerializeField] private int maximoErroresPermitidos = 5;

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
    // REGISTRO DE ERRORES (Solo acumulan en silencio durante la etapa)
    // =========================================================

    public void RegistrarIngredientesMalOrdenados() => puntuacion?.RegistrarIngredientesMalOrdenados();
    public void RegistrarCarneCruda() => puntuacion?.RegistrarCarneCruda();
    public void RegistrarCarneQuemada() => puntuacion?.RegistrarCarneQuemada();
    public void RegistrarContaminacionCruzadaCortado() => puntuacion?.RegistrarContaminacionCruzadaCortado();

    // =========================================================
    // CONTROL CENTRALIZADO DE DERROTA POR ERRORES
    // =========================================================

    /// <summary>
    /// Comprueba si se alcanzó el límite de errores. 
    /// Si se alcanzó, termina el juego en derrota (0 estrellas). 
    /// Si no, avanza a la siguiente etapa designada.
    /// </summary>
    public void EjecutarSiguientePasoOPerder(UnityAction accionSiguienteEtapa)
    {
        if (ContarErrores() >= maximoErroresPermitidos)
        {
            DerrotaInmediata();
        }
        else
        {
            accionSiguienteEtapa?.Invoke();
        }
    }

    public void DerrotaInmediata()
    {
        if (estadoActual == EstadoJuego.Final) return;

        estadoActual = EstadoJuego.Final;
        PopupManager.Instance?.CerrarPopup();
        UIManager.Instance?.MostrarResumenFinal(false, ContarErrores());
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
        // Al terminar de ordenar y presionar continuar
        EjecutarSiguientePasoOPerder(MostrarInstruccionesReceta);
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
            PopupContenido.Instance.MostrarInstruccionesReceta(EmpezarSeleccionReceta);
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
            UIManager.Instance?.PrepararBotonContinuar(ContinuarDesdeSeleccionReceta);
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
        
        EjecutarSiguientePasoOPerder(ActivarLavadoVerduras);
    }

    // =========================================================
    // ETAPA 3 - LAVADO DE VERDURAS
    // =========================================================

    public void ActivarLavadoVerduras()
    {
        estadoActual = EstadoJuego.LavadoVerduras;
        IniciarEtapaLavadoVerduras();
    }

    private void IniciarEtapaLavadoVerduras()
    {
        CameraManager.Instance?.MostrarCamaraLavadoVerduras(); 
        LavadoVerduras.Instance?.IniciarLavado(); 
    }

    public void ContinuarDespuesDelLavadoVerduras()
    {
        // Al presionar continuar después de lavar verduras
        EjecutarSiguientePasoOPerder(ActivarLavadoManos);
    }

    // =========================================================
    // ETAPA 4 - LAVADO DE MANOS
    // =========================================================

    public void ActivarLavadoManos()
    {
        estadoActual = EstadoJuego.LavadoManos;
        LavadoManos.Instance?.IniciarLavado();
    }

    public void ContinuarDespuesDelLavadoManos()
    {
        // Al presionar continuar después de lavarse las manos
        EjecutarSiguientePasoOPerder(ActivarCortado);
    }

    // =========================================================
    // ETAPA 5 - CORTADO
    // =========================================================

    public void ActivarCortado()
    {
        estadoActual = EstadoJuego.Cortado;
        CortadoManager.Instance?.IniciarCortado();
    }

    public void ContinuarDespuesDelCortado()
    {
        // Si hay popup de feedback, lo muestra primero con la barra actualizada
        if (PopupContenido.Instance != null)
        {
            PopupContenido.Instance.MostrarFeedbackCortado(
                contaminacionCruzadaCortado,
                () => EjecutarSiguientePasoOPerder(ActivarMezclado)
            );
        }
        else
        {
            EjecutarSiguientePasoOPerder(ActivarMezclado);
        }
    }

    // =========================================================
    // ETAPA 5.5 - MEZCLADO
    // =========================================================

    public void ActivarMezclado()
    {
        estadoActual = EstadoJuego.Mezclado;
        Debug.Log("[Mezclado] ActivarMezclado ejecutado");

        if (PopupContenido.Instance != null)
        {
            PopupContenido.Instance.MostrarInstruccionesMezclado(IniciarMezclado);
        }
        else
        {
            IniciarMezclado();
        }
    }

    private void IniciarMezclado()
    {
        MezcladoManager.Instance?.IniciarMezclado();
    }

    public void ContinuarDespuesDelMezclado()
    {
        if (PopupContenido.Instance != null)
        {
            PopupContenido.Instance.MostrarFeedbackMezclado(
                () => EjecutarSiguientePasoOPerder(ActivarCoccion)
            );
        }
        else
        {
            EjecutarSiguientePasoOPerder(ActivarCoccion);
        }
    }

    // =========================================================
    // ETAPA 6 - COCCIÓN
    // =========================================================

    public void ActivarCoccion()
    {
        estadoActual = EstadoJuego.Coccion;

        if (PopupContenido.Instance != null)
        {
            PopupContenido.Instance.MostrarInstruccionesCoccion(IniciarCoccion);
        }
        else
        {
            IniciarCoccion();
        }
    }

    private void IniciarCoccion()
    {
        CoccionManager.Instance?.IniciarCoccion();
    }

    public void ContinuarDespuesDeCoccion()
    {
        EjecutarSiguientePasoOPerder(ActivarEmplatado);
    }

    // =========================================================
    // ETAPA 7 - EMPLATADO
    // =========================================================

    public void ActivarEmplatado()
    {
        estadoActual = EstadoJuego.Emplatado;
        EmplatadoManager.Instance?.IniciarEmplatado();
    }

    public void ContinuarDespuesDelEmplatado()
    {
        FinalizarPartida();
    }

    // =========================================================
    // FINALIZACIÓN Y MENÚ
    // =========================================================

    public int ContarErrores()
    {
        int errores = 0;
        if (ingredientesMalOrdenados) errores++;
        if (carneCruda) errores++;
        if (carneQuemada) errores++;

        if (puntuacion != null)
            errores += puntuacion.ErroresCortadoContaminado;

        return errores;
    }

    private void FinalizarPartida()
    {
        estadoActual = EstadoJuego.Final;
        bool gano = puntuacion != null && puntuacion.EsVictoria();
        int totalErrores = ContarErrores();

        UIManager.Instance?.MostrarResumenFinal(gano, totalErrores);
    }

    public void VolverAlMenu()
    {
        SceneLoader.CargarEscena(nombreEscenaMenu);
    }
}