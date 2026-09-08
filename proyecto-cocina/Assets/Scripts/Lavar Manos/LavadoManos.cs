using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class LavadoManos : MonoBehaviour
{
    public static LavadoManos Instance { get; private set; }

    [Header("Referencias")]
    [SerializeField] private SoapController jabonDraggable;
    [SerializeField] private Slider barra;
    [SerializeField] private GameObject objetoBarra;

    [Header("Animaciones")]
    [SerializeField] private ImageFrameAnimation animacionManos;
    [SerializeField] private ImageFrameAnimation animacionEspuma;

    [Header("Configuración")]
    [SerializeField] private float tiempoNecesario = 3f;
    [SerializeField] private float velocidadMinima = 100f;

    [Header("Resultado")]
    [SerializeField] private Image imagenManos;
    [SerializeField] private Sprite manosLimpias;
    [SerializeField] private Button botonContinuar;

    private float progreso;
    private bool completado;
    private Vector3 ultimaPosicionJabon;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        Reiniciar();
    }

    private void Update()
    {
        if (completado || jabonDraggable == null) return;

        // Comprobación de arrastre y velocidad
        float distancia = Vector3.Distance(jabonDraggable.transform.position, ultimaPosicionJabon);
        bool estaLavando = AreaLavado.JugadorEstaEncima &&
                           jabonDraggable.EstaSiendoArrastrado &&
                           distancia >= velocidadMinima * Time.deltaTime;

        ultimaPosicionJabon = jabonDraggable.transform.position;

        if (estaLavando)
        {
            progreso += Time.deltaTime;
            SetVisualesLavando(true);

            if (progreso >= tiempoNecesario)
                CompletarLavado();
        }
        else
        {
            progreso = 0f;
            SetVisualesLavando(false);
        }

        if (barra != null)
            barra.value = Mathf.Clamp01(progreso / tiempoNecesario);
    }

    // --- Helpers de Estado Visual ---

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
            if (activo) animacionManos.Play(); else animacionManos.Stop();
        }
    }

    // --- Flujo de la Etapa ---

    private void CompletarLavado()
    {
        completado = true;
        SetVisualesLavando(false);

        if (imagenManos != null && manosLimpias != null)
            imagenManos.sprite = manosLimpias;

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
        SetBotonContinuar(false);
        GameManager.Instance?.ContinuarDespuesDelLavado();
    }

    public void IniciarLavado()
    {
        Reiniciar();

        if (PopupContenido.Instance != null)
            PopupContenido.Instance.MostrarInstruccionesLavado(ActivarCamaraLavado);
        else
            ActivarCamaraLavado();
    }

    private void ActivarCamaraLavado()
    {
        CameraManager.Instance?.MostrarCamaraLavadoManos();
    }

    public void Reiniciar()
    {
        progreso = 0f;
        completado = false;

        if (barra != null) barra.value = 0f;
        if (jabonDraggable != null) ultimaPosicionJabon = jabonDraggable.transform.position;

        SetVisualesLavando(false);
        SetBotonContinuar(false);
    }

    // --- Control del Botón ---

    private void SetBotonContinuar(bool visible, UnityAction accion = null)
    {
        if (botonContinuar == null) return;

        botonContinuar.onClick.RemoveAllListeners();
        if (visible && accion != null)
            botonContinuar.onClick.AddListener(accion);

        botonContinuar.interactable = visible;
        botonContinuar.gameObject.SetActive(visible);
    }
}