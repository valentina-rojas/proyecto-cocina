using System;
using UnityEngine;
using UnityEngine.Events;

public class PopupContenido : MonoBehaviour
{
    public static PopupContenido Instance { get; private set; }

    public struct MensajePopup
    {
        public string titulo;
        public string[] lineasTexto;
        public Sprite imagen;

        public MensajePopup(string titulo, string texto, Sprite imagen = null)
        {
            this.titulo = titulo;
            this.lineasTexto = new string[] { texto };
            this.imagen = imagen;
        }

        public MensajePopup(string titulo, string[] lineas, Sprite imagen = null)
        {
            this.titulo = titulo;
            this.lineasTexto = lineas;
            this.imagen = imagen;
        }
    }

    [Header("Imágenes: Guardado de Alimentos")]
    [SerializeField] private Sprite imgInstIngredientes;
    [SerializeField] private Sprite imgFbIngredientesCorrectos;
    [SerializeField] private Sprite imgFbIngredientesIncorrectos;

    [Header("Imágenes: Lavado de Verduras")]
    [SerializeField] private Sprite imgInstLavadoVerduras;
    [SerializeField] private Sprite imgFbLavadoVerduras;

    [Header("Imágenes: Lavado de Manos")]
    [SerializeField] private Sprite imgInstLavadoManos;
    [SerializeField] private Sprite imgFbLavadoManos;

    [Header("Imágenes: Cortado")]
    [SerializeField] private Sprite imgInstCortado;
    [SerializeField] private Sprite imgFbCortado;
    [SerializeField] private Sprite imgFbCortadoContaminado;

    [Header("Imágenes: Mezclado")]
    [SerializeField] private Sprite imgInstMezclado;
    [SerializeField] private Sprite imgFbMezclado;

    [Header("Imágenes: Cocción")]
    [SerializeField] private Sprite imgInstCoccion;
    [SerializeField] private Sprite imgFbCarneCorrecta;
    [SerializeField] private Sprite imgFbCarneCruda;
    [SerializeField] private Sprite imgFbCarneQuemada;

    [Header("Imágenes: Emplatado")]
    [SerializeField] private Sprite imgInstEmplatado;
    [SerializeField] private Sprite imgFbEmplatado;

    [NonSerialized] public MensajePopup instIngredientes;
    [NonSerialized] public MensajePopup fbIngredientesCorrectos;
    [NonSerialized] public MensajePopup fbIngredientesIncorrectos;

    [NonSerialized] public MensajePopup instLavadoVerduras;
    [NonSerialized] public MensajePopup fbLavadoVerduras;

    [NonSerialized] public MensajePopup instLavado;
    [NonSerialized] public MensajePopup fbLavado;

    [NonSerialized] public MensajePopup instCortado;
    [NonSerialized] public MensajePopup fbCortado;
    [NonSerialized] public MensajePopup fbCortadoContaminado;

    [NonSerialized] public MensajePopup instMezclado;
    [NonSerialized] public MensajePopup fbMezclado;

    [NonSerialized] public MensajePopup instCoccion;
    [NonSerialized] public MensajePopup fbCarneCorrecta;
    [NonSerialized] public MensajePopup fbCarneCruda;
    [NonSerialized] public MensajePopup fbCarneQuemada;

    [NonSerialized] public MensajePopup instEmplatado;
    [NonSerialized] public MensajePopup fbEmplatado;

    // Control de secuencia activa
    private int secuenciaIdActual = 0;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        InicializarMensajes();
    }

    private void InicializarMensajes()
    {
        instIngredientes = new MensajePopup("Organización de la heladera", new string[] {
            "El manual indica que nunca debemos juntar alimentos crudos con los que ya están listos para consumir.",
            "Ubicá cada producto según su tipo y temperatura recomendada para evitar goteos y contaminación.",
            "Arrastrá cada ingrediente a su estante o sector correspondiente."
        }, imgInstIngredientes);

        fbIngredientesCorrectos = new MensajePopup("¡Excelente organización!", "Guardaste cada alimento en el lugar indicado, garantizando su frescura y seguridad.", imgFbIngredientesCorrectos);
        fbIngredientesIncorrectos = new MensajePopup("Revisá la distribución", "Algunos alimentos quedaron en el sector equivocado.", imgFbIngredientesIncorrectos);

        instLavadoVerduras = new MensajePopup("Lavado de verduras", "Asegurate de lavarlas correctamente para quitarles toda la suciedad.", imgInstLavadoVerduras);
        fbLavadoVerduras = new MensajePopup("¡Verduras limpias!", "¡Excelente! Todas las verduras quedaron limpias y listas para cortar.", imgFbLavadoVerduras);

        instLavado = new MensajePopup("Higiene personal", new string[] { 
            "Antes de tocar cualquier comida o utensilio, es fundamental sanitizarse.",
            "Frotá bien con jabón por toda la superficie de las manos hasta completar el tiempo requerido."
        }, imgInstLavadoManos);

        fbLavado = new MensajePopup("¡Manos limpias!", "Cumpliste con el protocolo de desinfección, ahora podés manipular los alimentos con seguridad.", imgFbLavadoManos);

        instCortado = new MensajePopup("Cortar los ingredientes", new string[] {
            "Seleccioná la tabla y el cuchillo asignados a cada tipo de alimento para prevenir la contaminación cruzada.",
            "Deslizá con cuidado siguiendo el trazo de la guía para lograr un corte parejo."
        }, imgInstCortado);

        fbCortado = new MensajePopup("¡Corte preciso!", "Usaste los utensilios correctos y completaste el corte a la perfección.", imgFbCortado);
        fbCortadoContaminado = new MensajePopup("¡Atención!", "La tabla o el cuchillo elegidos no correspondían a esos alimentos, se produjo contaminación cruzada.", imgFbCortadoContaminado);

        instMezclado = new MensajePopup("Mezclar los ingredientes", "Arrastrá los ingredientes al bowl y realizá movimientos circulares para mezclarlos.", imgInstMezclado);
        fbMezclado = new MensajePopup("¡Mezcla lista!", "¡Excelente! Los ingredientes se integraron de forma uniforme.", imgFbMezclado);

        instCoccion = new MensajePopup("Punto de cocción", new string[] {
            "El manual dice que para una cocción adecuada se deben superar los 70 °C...",
            "Prestá atención al tiempo sobre la hornalla y retirá la pieza en el momento justo para no secarla ni quemarla."
        }, imgInstCoccion);

        fbCarneCorrecta = new MensajePopup("¡Punto justo!", "La carne alcanzó una temperatura segura y una cocción uniforme.", imgFbCarneCorrecta);
        fbCarneCruda = new MensajePopup("Falta cocción", "El alimento quedó crudo en el centro, lo que representa un riesgo para la salud.", imgFbCarneCruda);
        fbCarneQuemada = new MensajePopup("Exceso de calor", "La comida superó el tiempo máximo en el fuego y se quemó.", imgFbCarneQuemada);

        instEmplatado = new MensajePopup("Emplatado", "Colocá los ingredientes en el plato siguiendo el orden indicado.", imgInstEmplatado);
        fbEmplatado = new MensajePopup("¡Plato terminado!", "Completaste correctamente la preparación.", imgFbEmplatado);
    }

    private void Mostrar(MensajePopup msg, UnityAction accionFinal = null)
    {
        if (msg.lineasTexto == null || msg.lineasTexto.Length == 0)
        {
            accionFinal?.Invoke();
            return;
        }

        // Cada llamada genera un identificador único
        secuenciaIdActual++;
        int idEstaSecuencia = secuenciaIdActual;

        MostrarSecuencia(msg.titulo, msg.lineasTexto, 0, msg.imagen, accionFinal, idEstaSecuencia);
    }

    private void MostrarSecuencia(string titulo, string[] lineas, int index, Sprite imagen, UnityAction accionFinal, int idSecuencia)
    {
        // Si la secuencia fue cancelada/omitida o se inició otra, aborta de inmediato
        if (idSecuencia != secuenciaIdActual)
        {
            return;
        }

        if (PopupManager.Instance == null)
        {
            accionFinal?.Invoke();
            return;
        }

        bool esUltimaLinea = index >= lineas.Length - 1;

        // Callback cuando el usuario da a CONTINUAR (línea por línea)
        UnityAction callbackContinuar;
        if (esUltimaLinea)
        {
            callbackContinuar = () =>
            {
                if (idSecuencia == secuenciaIdActual)
                {
                    accionFinal?.Invoke();
                }
            };
        }
        else
        {
            callbackContinuar = () =>
            {
                if (idSecuencia == secuenciaIdActual)
                {
                    MostrarSecuencia(titulo, lineas, index + 1, imagen, accionFinal, idSecuencia);
                }
            };
        }

        // Callback cuando el usuario da a OMITIR (cancela la secuencia entera)
        UnityAction callbackOmitir = () =>
        {
            if (idSecuencia == secuenciaIdActual)
            {
                // Invalida la secuencia para que ninguna otra línea pueda volver a abrirse
                secuenciaIdActual++;
                accionFinal?.Invoke();
            }
        };

        PopupManager.Instance.MostrarPopup(titulo, lineas[index], imagen, callbackContinuar, callbackOmitir);
    }

    // --- Métodos de Instrucciones ---
    public void MostrarInstruccionesIngredientes(UnityAction accion = null) => Mostrar(instIngredientes, accion);

    public void MostrarInstruccionesReceta(UnityAction accion = null)
    {
        if (DayManager.Instance != null)
        {
            string titulo = "Receta del día";
            Sprite imagen = DayManager.Instance.imagenRecetaActual;

            string[] lineasReceta = new string[]
            {
                $"El plato del día es una {DayManager.Instance.recetaActual}.",
                "Acá está la lista de ingredientes, asegurate de que no falte ninguno."
            };

            secuenciaIdActual++;
            MostrarSecuencia(titulo, lineasReceta, 0, imagen, accion, secuenciaIdActual);
        }
        else
        {
            Debug.LogWarning("PopupContenido: DayManager.Instance es null.");
            accion?.Invoke();
        }
    }

    public void MostrarInstruccionesLavadoVerduras(UnityAction accion = null) => Mostrar(instLavadoVerduras, accion);
    public void MostrarFeedbackLavadoVerduras(UnityAction accion = null)      => Mostrar(fbLavadoVerduras, accion);

    public void MostrarInstruccionesLavado(UnityAction accion = null)         => Mostrar(instLavado, accion);
    public void MostrarInstruccionesCortado(UnityAction accion = null)        => Mostrar(instCortado, accion);
    public void MostrarInstruccionesMezclado(UnityAction accion = null)       => Mostrar(instMezclado, accion);
    public void MostrarInstruccionesCoccion(UnityAction accion = null)        => Mostrar(instCoccion, accion);
    public void MostrarInstruccionesEmplatado(UnityAction accion = null)      => Mostrar(instEmplatado, accion);

    // --- Métodos de Feedbacks ---
    public void MostrarFeedbackIngredientes(bool correcto, UnityAction accion = null) => 
        Mostrar(correcto ? fbIngredientesCorrectos : fbIngredientesIncorrectos, accion);

    public void MostrarFeedbackLavado(UnityAction accion = null)              => Mostrar(fbLavado, accion);

    public void MostrarFeedbackCortado(bool huboContaminacionCruzada, UnityAction accion = null)
    {
        Mostrar(huboContaminacionCruzada ? fbCortadoContaminado : fbCortado, accion);
    }

    public void MostrarFeedbackMezclado(UnityAction accion = null)            => Mostrar(fbMezclado, accion);

    public void MostrarFeedbackCoccion(bool cruda, bool quemada, UnityAction accion = null)
    {
        if (quemada) Mostrar(fbCarneQuemada, accion);
        else if (cruda) Mostrar(fbCarneCruda, accion);
        else Mostrar(fbCarneCorrecta, accion);
    }

    public void MostrarFeedbackEmplatado(UnityAction accion = null)           => Mostrar(fbEmplatado, accion);
}