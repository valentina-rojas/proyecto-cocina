using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro; // Si usas TextMeshPro (si usas UI tradicional, cambia TextMeshProUGUI por Text)

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
    [SerializeField] private TextMeshProUGUI textoDesempeno; // O 'Text' de UnityEngine.UI
    [TextArea(2, 3)]
    [SerializeField] private string mensajeTresEstrellas = "¡Excelente trabajo!";
    [TextArea(2, 3)]
    [SerializeField] private string mensajeDosEstrellas = "¡Buen trabajo!";
    [TextArea(2, 3)]
    [SerializeField] private string mensajeUnaEstrella = "Juego Perdido";

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        if (panelResumen != null) panelResumen.SetActive(false);
        OcultarBotonContinuar();
        
        if (botonMenuPrincipal != null)
            botonMenuPrincipal.onClick.AddListener(() => GameManager.Instance.VolverAlMenu());
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

        // Reglas basadas en la cantidad de equivocaciones
        if (cantidadErrores == 0)
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
        else // 2 o más errores
        {
            estrellasConseguidas = 1;
            mostrarVictoria = false;
            mensaje = mensajeUnaEstrella;
        }

        // 1. Asignar el sprite de Victoria o Derrota
        if (imagenResultado != null)
        {
            imagenResultado.sprite = mostrarVictoria ? spriteVictoria : spriteDerrota;
        }

        // 2. Asignar el mensaje de texto limpio
        if (textoDesempeno != null)
        {
            textoDesempeno.text = mensaje;
        }

        // 3. Actualizar la cantidad visual de estrellas
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

            // Si tienes sprites para llena y vacía, alterna los sprites
            if (spriteEstrellaLlena != null && spriteEstrellaVacia != null)
            {
                imagenesEstrellas[i].sprite = encendida ? spriteEstrellaLlena : spriteEstrellaVacia;
                imagenesEstrellas[i].gameObject.SetActive(true);
            }
            else
            {
                // Si solo quieres encender/apagar el GameObject de la estrella
                imagenesEstrellas[i].gameObject.SetActive(encendida);
            }
        }
    }
}