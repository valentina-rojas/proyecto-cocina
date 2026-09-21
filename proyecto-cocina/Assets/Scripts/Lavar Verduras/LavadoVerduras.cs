using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class LavadoVerduras : MonoBehaviour
{
    public static LavadoVerduras Instance { get; private set; }

    [Header("Canilla")]
    [SerializeField] private Button botonCanilla;
    [SerializeField] private Image imagenCanilla;
    [SerializeField] private Sprite canillaCerrada;
    [SerializeField] private Sprite canillaAbierta;
    [SerializeField] private ImageFrameAnimation animacionAgua;

    [Header("Prefabs de Verduras a Lavar")]
    [Tooltip("Arrastra aquí los prefabs de las verduras (ej. Prefab_Tomate, Prefab_Zanahoria)")]
    [SerializeField] private List<DatosVerdura> prefabsVerduras = new List<DatosVerdura>();

    [Header("Slots en Mesada")]
    [Tooltip("Transforms vacíos en la mesada izquierda que marcan dónde aparecen las verduras sucias")]
    [SerializeField] private List<Transform> slotsSuciosMesa = new List<Transform>();

    [Tooltip("Imágenes en la mesada derecha que mostrarán las verduras limpiadas")]
    [SerializeField] private List<Image> slotsLimpiosMesa = new List<Image>();

    [Header("Mano y Verdura")]
    [SerializeField] private ManoVerduraController manoVerdura;

    [Header("UI y Sensibilidad")]
    [SerializeField] private Slider barraProgreso;
    [SerializeField] private GameObject objetoBarra;
    [SerializeField] private Button botonContinuar;
    [SerializeField] private float tiempoParaReiniciar = 0.5f;
    [SerializeField] private float delayVerduraLimpia = 0.6f;

    // Lista de instancias vivas en la escena
    private List<DatosVerdura> verdurasInstanciadas = new List<DatosVerdura>();

    // Estado actual de la ronda
    private DatosVerdura verduraSeleccionada;
    private float distanciaObjetivo;
    private float distanciaAcumulada;
    private float tiempoInactivo;
    private int verdurasLavadasTotal;
    private bool estaMoviendo;
    private bool verduraAgarrada;
    private bool canillaEstaAbierta;
    private bool completadoTodo;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        ApagarSlotsLimpios();
    }

    private void Start()
    {
        ConfigurarBotonCanilla();
        ReiniciarEscena();
    }

    private void ConfigurarBotonCanilla()
    {
        if (botonCanilla != null)
        {
            botonCanilla.onClick.RemoveAllListeners();
            botonCanilla.onClick.AddListener(AlternarCanilla);
            botonCanilla.transition = Selectable.Transition.None;
        }
    }

    private void ApagarSlotsLimpios()
    {
        for (int i = 0; i < slotsLimpiosMesa.Count; i++)
        {
            if (slotsLimpiosMesa[i] != null)
            {
                slotsLimpiosMesa[i].sprite = null;
                slotsLimpiosMesa[i].enabled = false;
                slotsLimpiosMesa[i].gameObject.SetActive(false);
            }
        }
    }

    // =========================================================
    // CONTROL DEL FLUJO GLOBAL
    // =========================================================

    public void IniciarLavado()
    {
        gameObject.SetActive(true);
        completadoTodo = false;
        ReiniciarEscena();

        // Muestra las instrucciones de lavado de verduras antes de encender la interacción/cámara
        if (PopupContenido.Instance != null)
            PopupContenido.Instance.MostrarInstruccionesLavadoVerduras(ActivarCamaraLavadoVerduras);
        else
            ActivarCamaraLavadoVerduras();
    }

    private void ActivarCamaraLavadoVerduras()
    {
        CameraManager.Instance?.MostrarCamaraLavadoVerduras();
    }

    // =========================================================
    // INSTANCIACIÓN EN LOS SLOTS SUCIOS
    // =========================================================

    private void SpawnearVerdurasEnSlots()
    {
        foreach (var v in verdurasInstanciadas)
        {
            if (v != null) Destroy(v.gameObject);
        }
        verdurasInstanciadas.Clear();

        int cantidad = Mathf.Min(prefabsVerduras.Count, slotsSuciosMesa.Count);

        for (int i = 0; i < cantidad; i++)
        {
            if (prefabsVerduras[i] == null || slotsSuciosMesa[i] == null) continue;

            DatosVerdura nuevaVerdura = Instantiate(prefabsVerduras[i], slotsSuciosMesa[i]);

            RectTransform rt = nuevaVerdura.GetComponent<RectTransform>();
            if (rt != null)
            {
                rt.anchoredPosition = Vector2.zero;
                rt.localScale = Vector3.one;
                rt.localRotation = Quaternion.identity;
            }

            Button btn = nuevaVerdura.Boton;
            if (btn != null)
            {
                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(() => AgarrarVerdura(nuevaVerdura));
            }

            verdurasInstanciadas.Add(nuevaVerdura);
        }
    }

    private void Update()
    {
        if (completadoTodo || distanciaAcumulada <= 0f) return;

        tiempoInactivo += Time.deltaTime;
        if (tiempoInactivo >= tiempoParaReiniciar)
        {
            ReiniciarLavadoActual();
        }
    }

    // =========================================================
    // 1. AGARRAR CUALQUIER VERDURA DEL SLOT
    // =========================================================

    public void AgarrarVerdura(DatosVerdura item)
    {
        if (verduraAgarrada || completadoTodo || item == null) return;

        verduraAgarrada = true;
        verduraSeleccionada = item;
        verduraSeleccionada.gameObject.SetActive(false);

        distanciaObjetivo = item.distanciaFrotadoNecesaria > 0 ? item.distanciaFrotadoNecesaria : 1800f;

        if (botonCanilla != null)
            botonCanilla.interactable = true;

        if (manoVerdura != null)
        {
            manoVerdura.CargarVerdura(item);
            manoVerdura.MostrarSosteniendo();
        }
    }

    // =========================================================
    // 2. CANILLA
    // =========================================================

    public void AlternarCanilla()
    {
        if (completadoTodo || !verduraAgarrada) return;

        if (canillaEstaAbierta) CerrarCanilla();
        else AbrirCanilla();
    }

    private void AbrirCanilla()
    {
        if (!verduraAgarrada || completadoTodo) return;

        canillaEstaAbierta = true;

        if (imagenCanilla != null && canillaAbierta != null)
            imagenCanilla.sprite = canillaAbierta;

        if (animacionAgua != null)
        {
            animacionAgua.gameObject.SetActive(true);
            animacionAgua.Play();
        }

        manoVerdura?.MostrarBajoAgua();
        SetVisibilidadBarra(true);
    }

    private void CerrarCanilla()
    {
        canillaEstaAbierta = false;

        if (imagenCanilla != null && canillaCerrada != null)
            imagenCanilla.sprite = canillaCerrada;

        if (animacionAgua != null)
        {
            animacionAgua.Stop();
            animacionAgua.gameObject.SetActive(false);
        }

        ReiniciarLavadoActual();

        if (verduraAgarrada && !completadoTodo)
        {
            manoVerdura?.MostrarSosteniendo();
            SetVisibilidadBarra(false);
        }
    }

    // =========================================================
    // 3. FROTADO
    // =========================================================

    public void ProcesarFrotado(float deltaMovimiento)
    {
        if (!canillaEstaAbierta || !verduraAgarrada || completadoTodo || deltaMovimiento <= 0f) return;

        tiempoInactivo = 0f;
        distanciaAcumulada += deltaMovimiento;

        float porcentaje = Mathf.Clamp01(distanciaAcumulada / distanciaObjetivo);
        if (barraProgreso != null) barraProgreso.value = porcentaje;

        if (!estaMoviendo)
        {
            estaMoviendo = true;
            manoVerdura?.IniciarAnimacionFrotado();
        }

        if (distanciaAcumulada >= distanciaObjetivo)
        {
            CompletarVerduraActual();
        }
    }

    public void ReiniciarLavadoActual()
    {
        distanciaAcumulada = 0f;
        tiempoInactivo = 0f;
        estaMoviendo = false;

        if (barraProgreso != null) barraProgreso.value = 0f;

        manoVerdura?.PausarAnimacionFrotado();

        if (canillaEstaAbierta && verduraAgarrada)
        {
            manoVerdura?.MostrarBajoAgua();
        }
    }

    // =========================================================
    // 4. COMPLETAR Y COLOCAR EN SLOT DERECHO
    // =========================================================

    private void CompletarVerduraActual()
    {
        distanciaAcumulada = 0f;
        estaMoviendo = false;

        CerrarCanilla();

        if (botonCanilla != null)
            botonCanilla.interactable = false;

        manoVerdura?.MostrarLimpio();

        StartCoroutine(RutinaPasarALaMesada());
    }

    private IEnumerator RutinaPasarALaMesada()
    {
        if (barraProgreso != null) barraProgreso.value = 1f;

        yield return new WaitForSeconds(delayVerduraLimpia);

        SetVisibilidadBarra(false);
        if (barraProgreso != null) barraProgreso.value = 0f;

        manoVerdura?.Ocultar();
        verduraAgarrada = false;

        if (verdurasLavadasTotal < slotsLimpiosMesa.Count && slotsLimpiosMesa[verdurasLavadasTotal] != null)
        {
            Image slotLimpio = slotsLimpiosMesa[verdurasLavadasTotal];
            slotLimpio.sprite = verduraSeleccionada.spriteMesaLimpia;

            Color c = slotLimpio.color;
            c.a = 1f;
            slotLimpio.color = c;

            slotLimpio.enabled = true;
            slotLimpio.gameObject.SetActive(true);
        }

        verdurasLavadasTotal++;

        if (verdurasLavadasTotal >= verdurasInstanciadas.Count)
        {
            completadoTodo = true;
            SetBotonContinuar(true, ContinuarDesdeLavadoVerduras);
        }
    }

    private void ContinuarDesdeLavadoVerduras()
    {
        SetBotonContinuar(false);

        if (PopupContenido.Instance != null)
            PopupContenido.Instance.MostrarFeedbackLavado(TerminarEtapaLavado);
        else
            TerminarEtapaLavado();
    }

    private void TerminarEtapaLavado()
    {
        SetBotonContinuar(false);
        gameObject.SetActive(false); // Apagamos este canvas/panel
        GameManager.Instance?.ContinuarDespuesDelLavadoVerduras();
    }

    private void SetVisibilidadBarra(bool visible)
    {
        if (objetoBarra != null) objetoBarra.SetActive(visible);
        else if (barraProgreso != null) barraProgreso.gameObject.SetActive(visible);
    }

    public void ReiniciarEscena()
    {
        StopAllCoroutines();

        completadoTodo = false;
        verduraAgarrada = false;
        canillaEstaAbierta = false;
        estaMoviendo = false;
        distanciaAcumulada = 0f;
        tiempoInactivo = 0f;
        verdurasLavadasTotal = 0;

        CerrarCanilla();

        if (botonCanilla != null)
            botonCanilla.interactable = false;

        ApagarSlotsLimpios();

        SetVisibilidadBarra(false);
        if (barraProgreso != null) barraProgreso.value = 0f;

        manoVerdura?.Ocultar();
        SetBotonContinuar(false);

        SpawnearVerdurasEnSlots();
    }

    private void SetBotonContinuar(bool visible, UnityAction accion = null)
    {
        if (botonContinuar == null) return;

        botonContinuar.onClick.RemoveAllListeners();
        if (visible && accion != null)
            botonContinuar.onClick.AddListener(accion);

        botonContinuar.interactable = visible;
        botonContinuar.gameObject.SetActive(visible);

        if (botonContinuar.TryGetComponent<CanvasGroup>(out var cg))
        {
            cg.alpha = visible ? 1f : 0f;
            cg.interactable = visible;
            cg.blocksRaycasts = visible;
        }
    }
}