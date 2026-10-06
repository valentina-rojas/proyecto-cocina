using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("Botón Continuar")]
    [SerializeField] private Button botonContinuar;

    [Header("Panel Resumen")]
    [SerializeField] private GameObject panelResumen;
    [SerializeField] private Image imagenResultado;
    [SerializeField] private Sprite spriteVictoria;
    [SerializeField] private Sprite spriteDerrota;
    [SerializeField] private Button botonMenuPrincipal;

    [Header("Calificación de Estrellas")]
    [Tooltip("Arrastra aquí las 3 imágenes de estrellas (Índice 0: 1ra, 1: 2da, 2: 3ra)")]
    [SerializeField] private Image[] imagenesEstrellas = new Image[3];
    [SerializeField] private Sprite spriteEstrellaLlena;
    [SerializeField] private Sprite spriteEstrellaVacia;

    [Header("Feedback de Desempeño")]
    [SerializeField] private TextMeshProUGUI textoDesempeno;
    [TextArea(2, 3)]
    [SerializeField] private string mensajeTresEstrellas = "¡Excelente trabajo!";
    [TextArea(2, 3)]
    [SerializeField] private string mensajeDosEstrellas = "¡Buen trabajo!";
    [TextArea(2, 3)]
    [SerializeField] private string mensajeUnaEstrella = "Puedes mejorar";
    [TextArea(2, 3)]
    [SerializeField] private string mensajeCeroEstrellas = "¡Cocina Contaminada!";

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void StartVisuals()
    {
        if (panelResumen != null) panelResumen.SetActive(false);
        OcultarBotonContinuar();
        
        if (botonMenuPrincipal != null)
            botonMenuPrincipal.onClick.AddListener(() => GameManager.Instance.VolverAlMenu());
    }

    private void Start()
    {
        StartVisuals();
    }

    public void PrepararBotonContinuar(UnityAction accion)
    {
        if (botonContinuar == null) return;
        botonContinuar.onClick.RemoveAllListeners();
        botonContinuar.onClick.AddListener(accion);
        botonContinuar.gameObject.SetActive(true);
        botonContinuar.interactable = true;
    }

    public void OcultarBotonContinuar()
    {
        if (botonContinuar == null) return;
        botonContinuar.onClick.RemoveAllListeners();
        botonContinuar.interactable = false;
        botonContinuar.gameObject.SetActive(false);
    }

    public void MostrarResumenFinal(bool esVictoria, int cantidadErrores)
    {
        int estrellasConseguidas;
        bool mostrarVictoria;
        string mensaje;

        // Si se llenó la barra (5 o más errores) -> DERROTA TOTAL CON 0 ESTRELLAS
        if (cantidadErrores >= 5)
        {
            estrellasConseguidas = 0;
            mostrarVictoria = false;
            mensaje = mensajeCeroEstrellas;
        }
        else if (cantidadErrores == 0 && esVictoria)
        {
            estrellasConseguidas = 3;
            mostrarVictoria = true;
            mensaje = mensajeTresEstrellas;
        }
        else if (cantidadErrores == 1)
        {
            estrellasConseguidas = 2;
            mostrarVictoria = true;
            mensaje = mensajeDosEstrellas;
        }
        else // De 2 a 4 errores
        {
            estrellasConseguidas = 1;
            mostrarVictoria = esVictoria;
            mensaje = mensajeUnaEstrella;
        }

        // 1. Asignar el sprite de Victoria o Derrota
        if (imagenResultado != null)
        {
            imagenResultado.sprite = mostrarVictoria ? spriteVictoria : spriteDerrota;
        }

        // 2. Asignar el mensaje de texto
        if (textoDesempeno != null)
        {
            textoDesempeno.text = mensaje;
        }

        // 3. Actualizar la cantidad visual de estrellas (con 0, todas quedarán vacías o apagadas)
        ActualizarEstrellasUI(estrellasConseguidas);

        // 4. Mostrar el panel
        if (panelResumen != null)
        {
            panelResumen.SetActive(true);
        }
    }

    private void ActualizarEstrellasUI(int estrellas)
    {
        if (imagenesEstrellas == null) return;

        for (int i = 0; i < imagenesEstrellas.Length; i++)
        {
            if (imagenesEstrellas[i] == null) continue;

            bool encendida = i < estrellas;

            // Si tienes sprites para llena y vacía, alterna los sprites (0 estrellas = las 3 vacías)
            if (spriteEstrellaLlena != null && spriteEstrellaVacia != null)
            {
                imagenesEstrellas[i].sprite = encendida ? spriteEstrellaLlena : spriteEstrellaVacia;
                imagenesEstrellas[i].gameObject.SetActive(true);
            }
            else
            {
                // Si solo quieres encender/apagar el GameObject de la estrella (0 estrellas = las 3 ocultas)
                imagenesEstrellas[i].gameObject.SetActive(encendida);
            }
        }
    }
}