using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class LavadoManos : MonoBehaviour
{
    public static LavadoManos Instance { get; private set; }

    [Header("Canilla")]
    [SerializeField] private Button botonCanilla;
    [SerializeField] private Image imagenCanilla;
    [SerializeField] private Sprite canillaCerrada;
    [SerializeField] private Sprite canillaAbierta;

    [Header("Jabón / Interactuable")]
    [Tooltip("Transform o Botón del jabón que debe palpitar tras abrir la canilla")]
    [SerializeField] private RectTransform objetoJabon;

    [Header("Animación de Palpitar")]
    [SerializeField] private float velocidadPalpitar = 4f;
    [SerializeField] private float escalaMinima = 0.98f;
    [SerializeField] private float escalaMaxima = 1.02f;

    [Header("Animaciones UI (Capas del Canvas)")]
    [SerializeField] private ImageFrameAnimation animacionAgua;  
    [SerializeField] private ImageFrameAnimation animacionManos;
    [SerializeField] private ImageFrameAnimation animacionEspuma;

    [Header("Progreso y Sensibilidad")]
    [SerializeField] private float distanciaTotalNecesaria = 2000f;
    [SerializeField] private float tiempoParaReiniciar = 0.5f;

    [Header("UI")]
    [SerializeField] private Slider barra;
    [SerializeField] private GameObject objetoBarra;
    [SerializeField] private Button botonContinuar;

    [Header("Resultado")]
    [SerializeField] private Image imagenManos;
    [SerializeField] private Sprite manosSucias;
    [SerializeField] private Sprite manosLimpias;

    private float distanciaAcumulada;
    private float tiempoInactivo;
    private bool canillaEstaAbierta;
    private bool completado;
    private bool estaFrotando;

    // Control de palpitación con respaldo de escalas originales del Canvas
    private readonly List<Transform> objetosPalpitando = new List<Transform>();
    private readonly Dictionary<Transform, Vector3> escalasOriginales = new Dictionary<Transform, Vector3>();
    private float tiempoAnimacion;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        CerrarCanilla();
    }

    private void OnEnable()
    {
        ActivarCamaraLavado();
        ActualizarObjetosPalpitando();
    }

    private void Start()
    {
        ConfigurarBotonCanilla();
        ReiniciarLavado();
        ActivarCamaraLavado();
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

    private void Update()
    {
        if (completado || distanciaAcumulada <= 0f) return;

        tiempoInactivo += Time.deltaTime;
        if (tiempoInactivo >= tiempoParaReiniciar)
        {
            ReiniciarLavado();
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

    // =========================================================
    // MECÁNICA DE CANILLA (TOGGLE)
    // =========================================================

    public void AlternarCanilla()
    {
        if (completado) return;

        if (canillaEstaAbierta) CerrarCanilla();
        else AbrirCanilla();
    }

    public void AbrirCanilla()
    {
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

        ActualizarObjetosPalpitando();
    }

    public void CerrarCanilla()
    {
        canillaEstaAbierta = false;
        estaFrotando = false;

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

        SetVisualesLavando(false);
        ActualizarObjetosPalpitando();
    }

    // =========================================================
    // MECÁNICA: FROTADO DE MANOS
    // =========================================================

    public void ProcesarFrotado(float deltaMovimiento)
    {
        if (!canillaEstaAbierta || completado || deltaMovimiento <= 0f) return;

        if (!estaFrotando)
        {
            estaFrotando = true;
            ActualizarObjetosPalpitando(); // Detiene el palpitar del jabón al interactuar
        }

        tiempoInactivo = 0f;
        distanciaAcumulada += deltaMovimiento;

        SetVisualesLavando(true);

        if (barra != null)
            barra.value = Mathf.Clamp01(distanciaAcumulada / distanciaTotalNecesaria);

        if (distanciaAcumulada >= distanciaTotalNecesaria)
        {
            CompletarLavado();
        }
    }

    public void ReiniciarLavado()
    {
        if (completado) return;

        distanciaAcumulada = 0f;
        tiempoInactivo = 0f;
        estaFrotando = false;

        if (barra != null) barra.value = 0f;

        if (imagenManos != null)
        {
            imagenManos.gameObject.SetActive(true);
            imagenManos.enabled = true;
            Color c = imagenManos.color;
            c.a = 1f;
            imagenManos.color = c;

            if (manosSucias != null)
                imagenManos.sprite = manosSucias;
        }

        SetVisualesLavando(false);
        SetBotonContinuar(false);
        ActualizarObjetosPalpitando();
    }

    private void SetVisualesLavando(bool activo)
    {
        if (objetoBarra != null && objetoBarra.activeSelf != activo)
            objetoBarra.SetActive(activo);

        if (animacionEspuma != null)
        {
            if (animacionEspuma.gameObject.activeSelf != activo)
                animacionEspuma.gameObject.SetActive(activo);

            if (activo) animacionEspuma.Play(); else animacionEspuma.Stop();
        }

        if (animacionManos != null)
        {
            if (animacionManos.gameObject != (imagenManos != null ? imagenManos.gameObject : null))
            {
                if (animacionManos.gameObject.activeSelf != activo)
                    animacionManos.gameObject.SetActive(activo);

                if (imagenManos != null)
                    imagenManos.enabled = !activo;
            }

            if (activo) animacionManos.Play(); else animacionManos.Stop();
        }
    }

    private void CompletarLavado()
    {
        completado = true;
        ResetearEscalaObjetos();

        SetVisualesLavando(false);
        CerrarCanilla();

        if (barra != null) barra.value = 1f;

        if (imagenManos != null)
        {
            imagenManos.enabled = true;
            if (manosLimpias != null)
                imagenManos.sprite = manosLimpias;
        }

        SetBotonContinuar(true, ContinuarDesdeLavado);
    }

    private void ContinuarDesdeLavado()
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
        GameManager.Instance?.ContinuarDespuesDelLavadoManos();
    }

    public void IniciarLavado()
    {
        gameObject.SetActive(true);
        completado = false;

        CerrarCanilla();
        ReiniciarLavado();
        ActivarCamaraLavado();

        if (PopupContenido.Instance != null)
            PopupContenido.Instance.MostrarInstruccionesLavado(null);
    }

    private void ActivarCamaraLavado()
    {
        CameraManager.Instance?.MostrarCamaraLavadoManos();
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

        if (completado) return;

        // 1. Canilla cerrada: debe palpitar el botón de la canilla
        if (!canillaEstaAbierta)
        {
            if (botonCanilla != null)
            {
                RegistrarObjetoPalpitar(botonCanilla.transform);
            }
            return;
        }

        // 2. Canilla abierta y aún no está frotando activamente: palpita el jabón
        if (!estaFrotando)
        {
            if (objetoJabon != null)
            {
                RegistrarObjetoPalpitar(objetoJabon.transform);
            }
        }
    }
}