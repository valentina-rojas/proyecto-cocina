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
    [SerializeField] private List<DatosVerdura> prefabsVerduras = new List<DatosVerdura>();

    [Header("Slots en Mesada")]
    [SerializeField] private List<Transform> slotsSuciosMesa = new List<Transform>();
    [SerializeField] private List<Image> slotsLimpiosMesa = new List<Image>();

    [Header("Mano y Verdura")]
    [SerializeField] private ManoVerduraController manoVerdura;

    [Header("Animación de Palpitar")]
    [SerializeField] private float velocidadPalpitar = 4f;
    [SerializeField] private float escalaMinima = 0.98f;
    [SerializeField] private float escalaMaxima = 1.02f;

    [Header("UI y Sensibilidad")]
    [SerializeField] private Slider barraProgreso;
    [SerializeField] private GameObject objetoBarra;
    [SerializeField] private Button botonContinuar;
    [SerializeField] private float tiempoParaReiniciar = 0.5f;
    [SerializeField] private float delayVerduraLimpia = 0.6f;

    private List<DatosVerdura> verdurasInstanciadas = new List<DatosVerdura>();

    private DatosVerdura verduraSeleccionada;
    private float distanciaObjetivo;
    private float distanciaAcumulada;
    private float tiempoInactivo;
    private int verdurasLavadasTotal;
    private bool estaMoviendo;
    private bool verduraAgarrada;
    private bool canillaEstaAbierta;
    private bool completadoTodo;

    // Control de palpitación con respaldo de escalas originales del Canvas
    private readonly List<Transform> objetosPalpitando = new List<Transform>();
    private readonly Dictionary<Transform, Vector3> escalasOriginales = new Dictionary<Transform, Vector3>();
    private float tiempoAnimacion;

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
        ActivarCamaraLavadoVerduras();
    }

    private void OnDisable()
    {
        ResetearEscalaObjetos();
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

    public void IniciarLavado()
    {
        gameObject.SetActive(true);
        completadoTodo = false;
        ReiniciarEscena();
        ActivarCamaraLavadoVerduras();

        if (PopupContenido.Instance != null)
            PopupContenido.Instance.MostrarInstruccionesLavadoVerduras(ActivarCamaraLavadoVerduras);
        else
            ActivarCamaraLavadoVerduras();
    }

    private void ActivarCamaraLavadoVerduras()
    {
        CameraManager.Instance?.MostrarCamaraLavadoVerduras();
    }

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
                btn.transition = Selectable.Transition.None;
            }

            verdurasInstanciadas.Add(nuevaVerdura);
        }

        ActualizarObjetosPalpitando();
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

    private void LateUpdate()
    {
        if (objetosPalpitando.Count == 0) return;

        tiempoAnimacion += Time.unscaledDeltaTime * velocidadPalpitar;
        float factor = (Mathf.Sin(tiempoAnimacion) + 1f) * 0.5f;
        float multiplicadorEscala = Mathf.Lerp(escalaMinima, escalaMaxima, factor);

        for (int i = 0; i < objetosPalpitando.Count; i++)
        {
            Transform t = objetosPalpitando[i];
            if (t != null && t.gameObject.activeInHierarchy)
            {
                if (escalasOriginales.TryGetValue(t, out Vector3 escalaBase))
                {
                    t.localScale = escalaBase * multiplicadorEscala;
                }
            }
        }
    }

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

        ActualizarObjetosPalpitando();
    }

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

        if (imagenCanilla != null)
        {
            imagenCanilla.gameObject.SetActive(true);
            imagenCanilla.enabled = true;
            Color c = imagenCanilla.color;
            c.a = 1f;
            imagenCanilla.color = c;

            if (canillaAbierta != null)
                imagenCanilla.sprite = canillaAbierta;
        }

        if (animacionAgua != null)
        {
            animacionAgua.gameObject.SetActive(true);
            animacionAgua.Play();
        }

        manoVerdura?.MostrarBajoAgua();
        SetVisibilidadBarra(true);

        ActualizarObjetosPalpitando();
    }

    private void CerrarCanilla()
    {
        canillaEstaAbierta = false;

        if (imagenCanilla != null)
        {
            imagenCanilla.gameObject.SetActive(true);
            imagenCanilla.enabled = true;
            Color c = imagenCanilla.color;
            c.a = 1f;
            imagenCanilla.color = c;

            if (canillaCerrada != null)
                imagenCanilla.sprite = canillaCerrada;
        }

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

        ActualizarObjetosPalpitando();
    }

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

        ActualizarObjetosPalpitando();
    }

    private void CompletarVerduraActual()
    {
        distanciaAcumulada = 0f;
        estaMoviendo = false;
        verduraAgarrada = false; // Se marca como no agarrada antes de cerrar para que no palpite la canilla

        ResetearEscalaObjetos();
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

        // Al asentarse en la mesada limpia, comienzan a palpitar las verduras restantes
        ActualizarObjetosPalpitando();
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
        ResetearEscalaObjetos();
        SetBotonContinuar(false);
        gameObject.SetActive(false);
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
        ResetearEscalaObjetos();

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
    }

    // =========================================================
    // CONTROL DIRECTO DE PALPITACIÓN
    // =========================================================

    private void ResetearEscalaObjetos()
    {
        for (int i = 0; i < objetosPalpitando.Count; i++)
        {
            Transform t = objetosPalpitando[i];
            if (t != null && escalasOriginales.TryGetValue(t, out Vector3 original))
            {
                t.localScale = original;
            }
        }

        objetosPalpitando.Clear();
        tiempoAnimacion = 0f;
    }

    private void RegistrarObjetoPalpitar(Transform t)
    {
        if (t == null) return;

        if (!escalasOriginales.ContainsKey(t))
        {
            Vector3 escalaActual = t.localScale;
            escalasOriginales[t] = escalaActual.sqrMagnitude > 0.001f ? escalaActual : Vector3.one;
        }

        if (!objetosPalpitando.Contains(t))
        {
            objetosPalpitando.Add(t);
        }
    }

    private void ActualizarObjetosPalpitando()
    {
        ResetearEscalaObjetos();

        if (completadoTodo) return;

        // 1. Sin verdura agarrada: palpitan las verduras sucias en la mesa izquierda
        if (!verduraAgarrada)
        {
            for (int i = 0; i < verdurasInstanciadas.Count; i++)
            {
                if (verdurasInstanciadas[i] != null && verdurasInstanciadas[i].gameObject.activeSelf)
                {
                    Transform target = verdurasInstanciadas[i].Boton != null ? 
                                       verdurasInstanciadas[i].Boton.transform : 
                                       verdurasInstanciadas[i].transform;

                    RegistrarObjetoPalpitar(target);
                }
            }
            return;
        }

        // 2. Con verdura agarrada pero canilla cerrada: palpita solo el botón de la canilla
        if (!canillaEstaAbierta)
        {
            if (botonCanilla != null)
            {
                RegistrarObjetoPalpitar(botonCanilla.transform);
            }
        }
        // 3. Con la canilla abierta: nada palpita mientras el usuario frota bajo el agua
    }
}