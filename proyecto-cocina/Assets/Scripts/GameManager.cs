using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Debug")]
    [Tooltip("Solo se usa en el Editor o Development Build. Con None arranca normal.")]
    [SerializeField] private EtapaInicialDebug etapaInicialDebug = EtapaInicialDebug.Ninguna;

    public enum EtapaInicialDebug
    {
        Ninguna,
        OrdenandoIngredientes,
        SeleccionandoReceta,
        LavadoVerduras,
        LavadoManos,
        Cortado,
        LavadoManosPostCortado,
        Mezclado,
        LavadoManosPostMezclado,
        Coccion,
        Emplatado
    }

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

    private class Etapa
    {
        public EstadoJuego estado;
        public UnityAction<UnityAction> intro;      // popup antes de empezar (recibe qué hacer al cerrarse)
        public UnityAction inicio;                  // lo que arranca la etapa
        public UnityAction<UnityAction> feedback;   // popup al terminar (recibe qué hacer al cerrarse)
        public UnityAction alTerminar;              // limpieza al terminar
    }

    [Header("Referencias de Sistemas")]
    [SerializeField] private ScoreData puntuacion;
    [SerializeField] private string nombreEscenaMenu = "MenuPrincipal";

    [Header("Condiciones de Derrota")]
    [SerializeField] private int maximoErroresPermitidos = 3;
    public int MaximoErroresPermitidos => maximoErroresPermitidos;

    public EstadoJuego estadoActual = EstadoJuego.OrdenandoIngredientes;

    public ScoreData Score => puntuacion;
    public bool ingredientesMalOrdenados => puntuacion != null && puntuacion.IngredientesMalOrdenados;
    public bool carneCruda => puntuacion != null && puntuacion.CarneCruda;
    public bool carneQuemada => puntuacion != null && puntuacion.CarneQuemada;
    public bool contaminacionCruzadaCortado => puntuacion != null && puntuacion.ContaminacionCruzadaCortado;

    private List<Etapa> etapas;
    private int indiceEtapa = -1;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        ConstruirEtapas();
    }

    private void Start()
    {
        int indiceInicial = ObtenerIndiceInicial();
        indiceEtapa = indiceInicial - 1;   // AvanzarEtapa() suma 1 al avanzar
        AvanzarEtapa();
    }

    private int ObtenerIndiceInicial()
    {
    #if UNITY_EDITOR || DEVELOPMENT_BUILD
        if (etapaInicialDebug != EtapaInicialDebug.Ninguna)
            return (int)etapaInicialDebug - 1;   // Ninguna = 0, la primera etapa real = 1
    #endif
        return 0;
    }

    // ---------------------------------------------------------
    // Definición de etapas (el único lugar que hay que tocar
    // para agregar, quitar o reordenar etapas)
    // ---------------------------------------------------------

    private void ConstruirEtapas()
    {
        etapas = new List<Etapa>
        {
            new Etapa
            {
                estado = EstadoJuego.OrdenandoIngredientes,
                inicio = () => PopupContenido.Instance?.MostrarInstruccionesIngredientes()
            },
            new Etapa
            {
                estado = EstadoJuego.SeleccionandoReceta,
                intro  = accion => PopupContenido.Instance.MostrarInstruccionesReceta(accion),
                inicio = () => SeleccionRecetaManager.Instance?.IniciarSeleccion(),
                alTerminar = () => SeleccionRecetaManager.Instance?.FinalizarSeleccion()
            },
            new Etapa
            {
                estado = EstadoJuego.LavadoVerduras,
                inicio = () =>
                {
                    CameraManager.Instance?.MostrarCamaraLavadoVerduras();
                    LavadoVerduras.Instance?.IniciarLavado();
                }
            },
            new Etapa
            {
                estado = EstadoJuego.LavadoManos,
                inicio = () => LavadoManos.Instance?.IniciarLavado()
            },
            new Etapa
            {
                estado = EstadoJuego.Cortado,
                inicio = () => CortadoManager.Instance?.IniciarCortado(),
                feedback = accion => PopupContenido.Instance.MostrarFeedbackCortado(contaminacionCruzadaCortado, accion)
            },
            new Etapa
            {
                estado = EstadoJuego.LavadoManos,
                intro  = alAceptar => PreguntarLavadoManos(alAceptar, RechazarLavadoManosPostCortado),
                inicio = () => LavadoManos.Instance?.IniciarLavado(mostrarInstrucciones: false)
            },
            new Etapa
            {
                estado = EstadoJuego.Mezclado,
                intro  = accion => PopupContenido.Instance.MostrarInstruccionesMezclado(accion),
                inicio = () => MezcladoManager.Instance?.IniciarMezclado(),
                feedback = accion => PopupContenido.Instance.MostrarFeedbackMezclado(accion)
            },
            new Etapa
            {
                estado = EstadoJuego.LavadoManos,
                intro  = alAceptar => PreguntarLavadoManos(alAceptar, RechazarLavadoManosPostMezclado),
                inicio = () => LavadoManos.Instance?.IniciarLavado(mostrarInstrucciones: false)
            },
            new Etapa
            {
                estado = EstadoJuego.Coccion,
                intro  = accion => PopupContenido.Instance.MostrarInstruccionesCoccion(accion),
                inicio = () => CoccionManager.Instance?.IniciarCoccion()
            },
            new Etapa
            {
                estado = EstadoJuego.Emplatado,
                inicio = () => EmplatadoManager.Instance?.IniciarEmplatado()
            }
        };
    }

    // ---------------------------------------------------------
    // Motor de etapas
    // ---------------------------------------------------------

    /// <summary>
    /// Llamar cuando el jugador termina la etapa actual (botón continuar, etc.).
    /// También se usa una vez al inicio para arrancar la primera etapa.
    /// </summary>
    public void AvanzarEtapa()
    {
        if (estadoActual == EstadoJuego.Final) return;

        Etapa actual = indiceEtapa >= 0 ? etapas[indiceEtapa] : null;
        actual?.alTerminar?.Invoke();

        UnityAction continuar = IrALaSiguienteEtapa;

        if (actual?.feedback != null && PopupContenido.Instance != null)
            actual.feedback(continuar);
        else
            continuar();
    }

    private void IrALaSiguienteEtapa()
    {
        bool esUltima = indiceEtapa == etapas.Count - 1;

        if (esUltima)
        {
            FinalizarPartida();
            return;
        }

        if (ContarErrores() >= maximoErroresPermitidos)
        {
            DerrotaInmediata();
            return;
        }

        indiceEtapa++;
        IniciarEtapa(etapas[indiceEtapa]);
    }

    private void IniciarEtapa(Etapa etapa)
    {
        estadoActual = etapa.estado;
        UIManager.Instance?.OcultarBotonContinuar();

        if (etapa.intro != null && PopupContenido.Instance != null)
            etapa.intro(etapa.inicio);
        else
            etapa.inicio?.Invoke();
    }

    // ---------------------------------------------------------
    // Selección de receta (único caso con botón condicionado)
    // ---------------------------------------------------------

    public void ActualizarEstadoSeleccionReceta(bool esValido)
    {
        if (estadoActual != EstadoJuego.SeleccionandoReceta) return;

        if (esValido)
            UIManager.Instance?.PrepararBotonContinuar(AvanzarEtapa);
        else
            UIManager.Instance?.OcultarBotonContinuar();
    }

    public void SeleccionRecetaCompleta() => ActualizarEstadoSeleccionReceta(true);

    // ---------------------------------------------------------
    // Opcion lavar manos luego de una instancia
    // ---------------------------------------------------------


    private void PreguntarLavadoManos(UnityAction alAceptar, UnityAction alRechazar)
    {
        if (PreguntaLavadoManosUI.Instance == null)
        {
            alAceptar();
            return;
        }

        PreguntaLavadoManosUI.Instance.Mostrar(alAceptar, alRechazar);
    }

    private void RechazarLavadoManosPostCortado()
    {
        puntuacion?.RegistrarManosSinLavarPostCortado();
        AvanzarEtapa();
    }

    private void RechazarLavadoManosPostMezclado()
    {
        puntuacion?.RegistrarManosSinLavarPostMezclado();
        AvanzarEtapa();
    }

    // ---------------------------------------------------------
    // Errores y derrota
    // ---------------------------------------------------------

    public void RegistrarIngredientesMalOrdenados() => puntuacion?.RegistrarIngredientesMalOrdenados();
    public void RegistrarCarneCruda() => puntuacion?.RegistrarCarneCruda();
    public void RegistrarCarneQuemada() => puntuacion?.RegistrarCarneQuemada();
    public void RegistrarContaminacionCruzadaCortado() => puntuacion?.RegistrarContaminacionCruzadaCortado();

    public int ContarErrores()
{
    int errores = 0;
    if (ingredientesMalOrdenados) errores++;
    if (carneCruda) errores++;
    if (carneQuemada) errores++;

    if (puntuacion != null)
    {
        errores += puntuacion.ErroresCortadoContaminado;
        if (puntuacion.ManosSinLavarPostCortado) errores++;
        if (puntuacion.ManosSinLavarPostMezclado) errores++;
    }

    return errores;
}

    public void DerrotaInmediata()
    {
        if (estadoActual == EstadoJuego.Final) return;

        estadoActual = EstadoJuego.Final;
        PopupManager.Instance?.CerrarPopup();
        UIManager.Instance?.MostrarResumenFinal(false, ContarErrores());
    }

    // ---------------------------------------------------------
    // Final y menú
    // ---------------------------------------------------------

    private void FinalizarPartida()
    {
        estadoActual = EstadoJuego.Final;
        bool gano = puntuacion != null && puntuacion.EsVictoria();
        UIManager.Instance?.MostrarResumenFinal(gano, ContarErrores());
    }

    public void VolverAlMenu() => SceneLoader.CargarEscena(nombreEscenaMenu);
}