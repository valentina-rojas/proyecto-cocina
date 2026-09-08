using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class LavadoManos : MonoBehaviour
{
    public static LavadoManos Instance;

    [Header("Referencias")]
    public SoapController jabonDraggable;
    public Slider barra;

    [Header("UI Lavado")]
    public GameObject objetoBarra;

    [Header("Animaciones")]
    public ImageFrameAnimation animacionManos;
    public ImageFrameAnimation animacionEspuma;

    [Header("Configuración")]
    public float tiempoNecesario = 3f;
    public float velocidadMinima = 100f;

    [Header("Resultado Lavado")]
    public Image imagenManos;
    public Sprite manosLimpias;

    [Header("Botón Siguiente")]
    [SerializeField] private Button botonContinuar;

    private float progreso;
    private bool completado;

    private Vector3 ultimaPosicionJabon;


    // =========================================================
    // UNITY
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

    private void Start()
    {
        Reiniciar();
        OcultarBoton();
    }


    // =========================================================
    // MECÁNICA DEL LAVADO
    // =========================================================

    private void Update()
    {
        if (completado || jabonDraggable == null)
            return;

        bool estaLavando =
            AreaLavado.JugadorEstaEncima &&
            jabonDraggable.EstaSiendoArrastrado;

        if (estaLavando)
        {
            float distancia = Vector3.Distance(
                jabonDraggable.transform.position,
                ultimaPosicionJabon
            );

            float velocidad = 0f;

            if (Time.deltaTime > 0f)
            {
                velocidad = distancia / Time.deltaTime;
            }

            if (velocidad >= velocidadMinima)
            {
                progreso += Time.deltaTime;

                MostrarElementosLavado();

                if (animacionManos != null)
                    animacionManos.Play();

                if (animacionEspuma != null)
                    animacionEspuma.Play();
            }
            else
            {
                progreso = 0f;

                DetenerAnimaciones();
                OcultarElementosLavado();
            }
        }
        else
        {
            progreso = 0f;

            DetenerAnimaciones();
            OcultarElementosLavado();
        }

        progreso = Mathf.Clamp(
            progreso,
            0f,
            tiempoNecesario
        );

        if (barra != null)
        {
            barra.value = progreso / tiempoNecesario;
        }

        ultimaPosicionJabon =
            jabonDraggable.transform.position;

        if (progreso >= tiempoNecesario)
        {
            CompletarLavado();
        }
    }


    // =========================================================
    // ELEMENTOS VISUALES
    // =========================================================

    private void MostrarElementosLavado()
    {
        if (objetoBarra != null)
            objetoBarra.SetActive(true);

        if (animacionEspuma != null)
            animacionEspuma.gameObject.SetActive(true);
    }


    private void OcultarElementosLavado()
    {
        if (objetoBarra != null)
            objetoBarra.SetActive(false);

        if (animacionEspuma != null)
        {
            animacionEspuma.Stop();
            animacionEspuma.gameObject.SetActive(false);
        }
    }


    private void DetenerAnimaciones()
    {
        if (animacionManos != null)
            animacionManos.Stop();

        if (animacionEspuma != null)
            animacionEspuma.Stop();
    }


    // =========================================================
    // LAVADO COMPLETADO
    // =========================================================

    private void CompletarLavado()
    {
        if (completado)
            return;

        completado = true;

        DetenerAnimaciones();
        OcultarElementosLavado();

        Debug.Log("¡Lavado completado con éxito!");

        // Mostrar las manos limpias
        if (imagenManos != null && manosLimpias != null)
        {
            imagenManos.sprite = manosLimpias;
        }

        // IMPORTANTE:
        // No mostramos el feedback automáticamente.
        // Primero aparece el botón Siguiente.
        PrepararBoton(
            ContinuarDesdeLavado
        );
    }


    // =========================================================
    // BOTÓN SIGUIENTE
    // =========================================================

    private void PrepararBoton(UnityAction accion)
    {
        if (botonContinuar == null)
        {
            Debug.LogError(
                "LavadoManos: No está asignado el botón Siguiente."
            );

            return;
        }

        botonContinuar.onClick.RemoveAllListeners();

        botonContinuar.onClick.AddListener(accion);

        botonContinuar.gameObject.SetActive(true);
        botonContinuar.interactable = true;
    }


    private void OcultarBoton()
    {
        if (botonContinuar == null)
            return;

        botonContinuar.onClick.RemoveAllListeners();

        botonContinuar.interactable = false;
        botonContinuar.gameObject.SetActive(false);
    }


    // =========================================================
    // CONTINUAR
    // =========================================================

    private void ContinuarDesdeLavado()
    {
        OcultarBoton();

        Debug.Log(
            "LavadoManos: Mostrando feedback del lavado."
        );

        if (PopupContenido.Instance != null)
        {
            PopupContenido.Instance.MostrarFeedbackLavado(
                TerminarEtapaLavado
            );
        }
        else
        {
            Debug.LogError(
                "LavadoManos: No existe PopupContenido."
            );

            TerminarEtapaLavado();
        }
    }


    private void TerminarEtapaLavado()
    {
        OcultarBoton();

        Debug.Log(
            "LavadoManos: Etapa finalizada."
        );

        if (GameManager.Instance != null)
        {
            GameManager.Instance.ContinuarDespuesDelLavado();
        }
        else
        {
            Debug.LogError(
                "LavadoManos: No existe GameManager."
            );
        }
    }


    // =========================================================
    // INICIAR / REINICIAR
    // =========================================================

    public void IniciarLavado()
    {
        Reiniciar();

        if (GameManager.Instance != null)
        {
            GameManager.Instance.estadoActual =
                GameManager.EstadoJuego.Lavado;
        }

        OcultarBoton();

        Debug.Log(
            "LavadoManos: Iniciando etapa."
        );

        if (PopupContenido.Instance != null)
        {
            PopupContenido.Instance.MostrarInstruccionesLavado(
                ActivarCamaraLavado
            );
        }
        else
        {
            Debug.LogError(
                "LavadoManos: No existe PopupContenido."
            );

            ActivarCamaraLavado();
        }
    }


    private void ActivarCamaraLavado()
    {
        if (CameraManager.Instance != null)
        {
            CameraManager.Instance
                .MostrarCamaraLavadoManos();
        }
        else
        {
            Debug.LogError(
                "LavadoManos: No existe CameraManager."
            );
        }
    }


    public void Reiniciar()
    {
        progreso = 0f;
        completado = false;

        if (barra != null)
            barra.value = 0f;

        if (jabonDraggable != null)
        {
            ultimaPosicionJabon =
                jabonDraggable.transform.position;
        }

        DetenerAnimaciones();
        OcultarElementosLavado();
        OcultarBoton();

        Debug.Log(
            "Lavado reiniciado."
        );
    }
}