using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class TermometroTemperatura : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private GameObject contenedorTermometro;
    [SerializeField] private RectTransform flecha;
    [SerializeField] private RectTransform limiteSuperior;
    [SerializeField] private RectTransform limiteInferior;
    [SerializeField] private Collider2D zonaCorrectaCollider;

    [Header("Botón de Parada")]
    [SerializeField] private Button botonDetener;

    [Header("Resultado UI")]
    [SerializeField] private GameObject popupResultado;
    [SerializeField] private TextMeshProUGUI textoResultado;
    [SerializeField] private Button botonReintentar;
    [SerializeField] private Button botonContinuar;

    [Header("Configuración")]
    [SerializeField] private float velocidad = 250f;

    private bool moviendo;
    private int direccion = 1;
    private DetectorColisionFlecha detectorFlecha;

    public bool TemperaturaCorrecta { get; private set; }
    public event Action OnFinalizado;

    private void Awake()
    {
        // Forzamos el popup apagado desde el primer instante
        if (popupResultado != null) popupResultado.SetActive(false);

        if (botonDetener != null) botonDetener.onClick.AddListener(Detener);
        if (botonReintentar != null) botonReintentar.onClick.AddListener(Reintentar);
        if (botonContinuar != null) botonContinuar.onClick.AddListener(Continuar);
    }

    private void OnDestroy()
    {
        if (botonDetener != null) botonDetener.onClick.RemoveListener(Detener);
        if (botonReintentar != null) botonReintentar.onClick.RemoveListener(Reintentar);
        if (botonContinuar != null) botonContinuar.onClick.RemoveListener(Continuar);
    }

    private void ConfigurarDetectorFisico()
    {
        if (flecha == null) return;

        detectorFlecha = flecha.GetComponent<DetectorColisionFlecha>();
        if (detectorFlecha == null)
            detectorFlecha = flecha.gameObject.AddComponent<DetectorColisionFlecha>();

        Rigidbody2D rb = flecha.GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            rb = flecha.gameObject.AddComponent<Rigidbody2D>();
            rb.bodyType = RigidbodyType2D.Kinematic;
            rb.useFullKinematicContacts = true;
        }

        BoxCollider2D col = flecha.GetComponent<BoxCollider2D>();
        if (col == null)
        {
            col = flecha.gameObject.AddComponent<BoxCollider2D>();
            col.isTrigger = true;
        }
    }

    private void Update()
    {
        if (!moviendo) return;
        MoverFlecha();
    }

    public void Iniciar()
{
    Debug.Log("TERMOMETRO INICIAR - activo: " + gameObject.activeInHierarchy);


    if (popupResultado != null)
        popupResultado.SetActive(false);

    if (contenedorTermometro != null)
        contenedorTermometro.SetActive(true);

    ConfigurarDetectorFisico();

    if (limiteInferior != null && flecha != null)
        flecha.position = limiteInferior.position;

    TemperaturaCorrecta = false;
    direccion = 1;

    if (botonDetener != null)
    {
        botonDetener.gameObject.SetActive(true);
        botonDetener.interactable = true;
    }

    moviendo = true;
        Debug.Log("TERMOMETRO INICIAR - moviendo: " + moviendo);
}

    private void MoverFlecha()
    {
        Vector3 posicion = flecha.position;
        posicion.y += velocidad * direccion * Time.deltaTime;

        if (posicion.y >= limiteSuperior.position.y)
        {
            posicion.y = limiteSuperior.position.y;
            direccion = -1;
        }
        else if (posicion.y <= limiteInferior.position.y)
        {
            posicion.y = limiteInferior.position.y;
            direccion = 1;
        }

        flecha.position = posicion;
    }

    public void Detener()
    {
        if (!moviendo) return;

        moviendo = false;

        if (botonDetener != null)
            botonDetener.gameObject.SetActive(false);

        TemperaturaCorrecta = detectorFlecha != null && detectorFlecha.EstaTocando;
        MostrarResultado();
    }

    private void MostrarResultado()
    {
        if (popupResultado != null) 
            popupResultado.SetActive(true);

        if (TemperaturaCorrecta)
        {
            textoResultado.text = "¡Temperatura correcta!\nLa hornalla está en temperatura óptima.";
            if (botonReintentar != null) botonReintentar.gameObject.SetActive(false);
        }
        else
        {
            textoResultado.text = "Temperatura incorrecta.\nPodés intentarlo nuevamente o continuar con esta potencia.";
            if (botonReintentar != null) botonReintentar.gameObject.SetActive(true);
        }
    }

    private void Reintentar() => Iniciar();

    private void Continuar()
    {
        if (popupResultado != null) 
            popupResultado.SetActive(false);

        gameObject.SetActive(false);
        OnFinalizado?.Invoke();
    }
}

public class DetectorColisionFlecha : MonoBehaviour
{
    public bool EstaTocando { get; private set; }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.name.Contains("Zona") || collision.CompareTag("ZonaCorrecta") || collision.name.Contains("Correcta"))
            EstaTocando = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.name.Contains("Zona") || collision.CompareTag("ZonaCorrecta") || collision.name.Contains("Correcta"))
            EstaTocando = false;
    }
}