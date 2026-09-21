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

    [Header("Tomates en Mesa (Lado Izquierdo - Sucios)")]
    [Tooltip("Arrastra aquí los botones de tomates sucios que están en la mesa")]
    [SerializeField] private List<Button> botonesTomatesMesa = new List<Button>();

    [Header("Tomates en Mesa (Lado Derecho - Limpios)")]
    [Tooltip("Arrastra aquí las imágenes de los tomates limpios que irán apareciendo al otro lado")]
    [SerializeField] private List<GameObject> tomatesLimpiosMesa = new List<GameObject>();

    [Header("Mano y Verdura")]
    [SerializeField] private ManoVerduraController manoVerdura;

    [Header("Progreso y Sensibilidad")]
    [Tooltip("Distancia en píxeles que debe frotarse para lavar CADA tomate")]
    [SerializeField] private float distanciaTotalNecesaria = 1800f;
    [Tooltip("Tiempo en segundos sin mover el dedo/mouse antes de reiniciar la barra a 0")]
    [SerializeField] private float tiempoParaReiniciar = 0.5f;

    [Header("UI")]
    [SerializeField] private Slider barraProgreso;
    [Tooltip("El GameObject padre del Slider que contiene fondo, relleno y borde")]
    [SerializeField] private GameObject objetoBarra;
    [SerializeField] private Button botonContinuar;

    [Header("Tiempos de Transición")]
    [Tooltip("Tiempo que se muestra el tomate limpio en la mano antes de dejarlo en la mesada")]
    [SerializeField] private float delayTomateLimpio = 0.6f;

    private int tomatesLavados = 0;
    private float distanciaAcumulada;
    private float tiempoInactivo;
    private bool estaMoviendo;
    private bool tomateAgarrado;
    private bool canillaEstaAbierta;
    private bool completadoTodo;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        ConfigurarBotones();
        ReiniciarEscena();
    }

    private void ConfigurarBotones()
    {
        if (botonCanilla != null)
        {
            botonCanilla.onClick.RemoveAllListeners();
            botonCanilla.onClick.AddListener(AlternarCanilla);
            botonCanilla.transition = Selectable.Transition.None;
        }

        // Asignar evento de click a cada tomate sucio
        for (int i = 0; i < botonesTomatesMesa.Count; i++)
        {
            Button btnTomate = botonesTomatesMesa[i];
            if (btnTomate != null)
            {
                btnTomate.onClick.RemoveAllListeners();
                btnTomate.onClick.AddListener(() => AgarrarTomate(btnTomate));
            }
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
    // 1. CLIC EN UN TOMATE DE LA IZQUIERDA
    // =========================================================
    public void AgarrarTomate(Button tomateClickeado)
    {
        if (tomateAgarrado || completadoTodo) return;

        tomateAgarrado = true;

        if (tomateClickeado != null)
            tomateClickeado.gameObject.SetActive(false);

        if (botonCanilla != null)
            botonCanilla.interactable = true;

        if (manoVerdura != null)
        {
            manoVerdura.MostrarSosteniendo();
        }
    }

    // =========================================================
    // 2. ABRIR / CERRAR CANILLA
    // =========================================================
    public void AlternarCanilla()
    {
        if (completadoTodo || !tomateAgarrado) return;

        if (canillaEstaAbierta) CerrarCanilla();
        else AbrirCanilla();
    }

    private void AbrirCanilla()
    {
        if (!tomateAgarrado || completadoTodo) return;

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

        if (tomateAgarrado && !completadoTodo)
        {
            manoVerdura?.MostrarSosteniendo();
            SetVisibilidadBarra(false);
        }
    }

    // =========================================================
    // 3. FROTADO Y PROGRESO
    // =========================================================
    public void ProcesarFrotado(float deltaMovimiento)
    {
        if (!canillaEstaAbierta || !tomateAgarrado || completadoTodo || deltaMovimiento <= 0f) return;

        tiempoInactivo = 0f;
        distanciaAcumulada += deltaMovimiento;

        float porcentaje = Mathf.Clamp01(distanciaAcumulada / distanciaTotalNecesaria);
        if (barraProgreso != null) barraProgreso.value = porcentaje;

        if (!estaMoviendo)
        {
            estaMoviendo = true;
            manoVerdura?.IniciarAnimacionFrotado();
        }

        if (distanciaAcumulada >= distanciaTotalNecesaria)
        {
            CompletarTomateActual();
        }
    }

    public void ReiniciarLavadoActual()
    {
        distanciaAcumulada = 0f;
        tiempoInactivo = 0f;
        estaMoviendo = false;

        if (barraProgreso != null) barraProgreso.value = 0f;

        manoVerdura?.PausarAnimacionFrotado();

        if (canillaEstaAbierta && tomateAgarrado)
        {
            manoVerdura?.MostrarBajoAgua();
        }
    }

    // =========================================================
    // 4. DEPOSITAR EL TOMATE AL OTRO LADO DE LA MESADA
    // =========================================================
    private void CompletarTomateActual()
    {
        distanciaAcumulada = 0f;
        estaMoviendo = false;

        CerrarCanilla();

        if (botonCanilla != null)
            botonCanilla.interactable = false;

        // Mostrar el tomate limpio en la mano
        manoVerdura?.MostrarLimpio();

        StartCoroutine(RutinaPasarTomateALaMesada());
    }

    private IEnumerator RutinaPasarTomateALaMesada()
    {
        if (barraProgreso != null) barraProgreso.value = 1f;

        // Breve pausa para ver el resultado limpio
        yield return new WaitForSeconds(delayTomateLimpio);

        SetVisibilidadBarra(false);
        if (barraProgreso != null) barraProgreso.value = 0f;

        // 1. Ocultar la mano que lo sostenía
        manoVerdura?.Ocultar();
        tomateAgarrado = false;

        // 2. Colocar el tomate limpio en el otro lado de la mesada
        if (tomatesLavados < tomatesLimpiosMesa.Count && tomatesLimpiosMesa[tomatesLavados] != null)
        {
            tomatesLimpiosMesa[tomatesLavados].SetActive(true);
        }

        tomatesLavados++;

        // 3. Evaluar si terminamos todos
        if (tomatesLavados >= botonesTomatesMesa.Count)
        {
            completadoTodo = true;
            SetBotonContinuar(true, Finalizar);
        }
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
        tomateAgarrado = false;
        canillaEstaAbierta = false;
        estaMoviendo = false;
        distanciaAcumulada = 0f;
        tiempoInactivo = 0f;
        tomatesLavados = 0;

        CerrarCanilla();

        if (botonCanilla != null)
            botonCanilla.interactable = false;

        // Reactivar todos los tomates sucios de la izquierda
        for (int i = 0; i < botonesTomatesMesa.Count; i++)
        {
            if (botonesTomatesMesa[i] != null)
                botonesTomatesMesa[i].gameObject.SetActive(true);
        }

        // Ocultar todos los tomates limpios de la derecha
        for (int i = 0; i < tomatesLimpiosMesa.Count; i++)
        {
            if (tomatesLimpiosMesa[i] != null)
                tomatesLimpiosMesa[i].SetActive(false);
        }

        SetVisibilidadBarra(false);
        if (barraProgreso != null) barraProgreso.value = 0f;

        manoVerdura?.Ocultar();
        SetBotonContinuar(false);
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

    private void Finalizar()
    {
        SetBotonContinuar(false);
        GameManager.Instance?.ContinuarDespuesDelLavado();
    }
}