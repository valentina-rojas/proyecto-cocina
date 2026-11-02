using System;
using UnityEngine;
using UnityEngine.Events;

public class PopupContenido : MonoBehaviour
{
    public static PopupContenido Instance { get; private set; }

    public readonly struct MensajePopup
    {
        public readonly string titulo;
        public readonly Sprite imagen;
        public readonly string[] lineas;

        public MensajePopup(string titulo, Sprite imagen, params string[] lineas)
        {
            this.titulo = titulo;
            this.imagen = imagen;
            this.lineas = lineas;
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

    private MensajePopup instIngredientes, fbIngredientesCorrectos, fbIngredientesIncorrectos;
    private MensajePopup instLavadoVerduras, fbLavadoVerduras;
    private MensajePopup instLavado, fbLavado;
    private MensajePopup instCortado, fbCortado, fbCortadoContaminado;
    private MensajePopup instMezclado, fbMezclado;
    private MensajePopup instCoccion, fbCarneCorrecta, fbCarneCruda, fbCarneQuemada;
    private MensajePopup instEmplatado, fbEmplatado;

    // Se incrementa con cada secuencia nueva u omitida; invalida los callbacks viejos
    private int secuenciaId;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        InicializarMensajes();
    }

    private void InicializarMensajes()
    {
        instIngredientes = new MensajePopup("Organización de la heladera", imgInstIngredientes,
            "El manual indica que nunca debemos juntar alimentos crudos con los que ya están listos para consumir.",
            "Ubicá cada producto según su tipo y temperatura recomendada para evitar goteos y contaminación.",
            "Arrastrá cada ingrediente a su estante o sector correspondiente.");
        fbIngredientesCorrectos = new MensajePopup("¡Excelente organización!", imgFbIngredientesCorrectos,
            "Guardaste cada alimento en el lugar indicado, garantizando su frescura y seguridad.");
        fbIngredientesIncorrectos = new MensajePopup("Revisá la distribución", imgFbIngredientesIncorrectos,
            "Algunos alimentos quedaron en el sector equivocado.");

        instLavadoVerduras = new MensajePopup("Lavado de verduras", imgInstLavadoVerduras,
            "Asegurate de lavarlas correctamente para quitarles toda la suciedad.");
        fbLavadoVerduras = new MensajePopup("¡Verduras limpias!", imgFbLavadoVerduras,
            "¡Excelente! Todas las verduras quedaron limpias y listas para cortar.");

        instLavado = new MensajePopup("Higiene personal", imgInstLavadoManos,
            "Antes de tocar cualquier comida o utensilio, es fundamental sanitizarse.",
            "Frotá bien con jabón por toda la superficie de las manos hasta completar el tiempo requerido.");
        fbLavado = new MensajePopup("¡Manos limpias!", imgFbLavadoManos,
            "Cumpliste con el protocolo de desinfección, ahora podés manipular los alimentos con seguridad.");

        instCortado = new MensajePopup("Cortar los ingredientes", imgInstCortado,
            "Seleccioná la tabla y el cuchillo asignados a cada tipo de alimento para prevenir la contaminación cruzada.",
            "Deslizá con cuidado siguiendo el trazo de la guía para lograr un corte parejo.");
        fbCortado = new MensajePopup("¡Corte preciso!", imgFbCortado,
            "Usaste los utensilios correctos y completaste el corte a la perfección.");
        fbCortadoContaminado = new MensajePopup("¡Atención!", imgFbCortadoContaminado,
            "La tabla o el cuchillo elegidos no correspondían a esos alimentos, se produjo contaminación cruzada.");

        instMezclado = new MensajePopup("Mezclar los ingredientes", imgInstMezclado,
            "Arrastrá los ingredientes al bowl y realizá movimientos circulares para mezclarlos.");
        fbMezclado = new MensajePopup("¡Mezcla lista!", imgFbMezclado,
            "¡Excelente! Los ingredientes se integraron de forma uniforme.");

        instCoccion = new MensajePopup("Punto de cocción", imgInstCoccion,
            "El manual dice que para una cocción adecuada se deben superar los 70 °C...",
            "Prestá atención al tiempo sobre la hornalla y retirá la pieza en el momento justo para no secarla ni quemarla.");
        fbCarneCorrecta = new MensajePopup("¡Punto justo!", imgFbCarneCorrecta,
            "La carne alcanzó una temperatura segura y una cocción uniforme.");
        fbCarneCruda = new MensajePopup("Falta cocción", imgFbCarneCruda,
            "El alimento quedó crudo en el centro, lo que representa un riesgo para la salud.");
        fbCarneQuemada = new MensajePopup("Exceso de calor", imgFbCarneQuemada,
            "La comida superó el tiempo máximo en el fuego y se quemó.");

        instEmplatado = new MensajePopup("Emplatado", imgInstEmplatado,
            "Colocá los ingredientes en el plato siguiendo el orden indicado.");
        fbEmplatado = new MensajePopup("¡Plato terminado!", imgFbEmplatado,
            "Completaste correctamente la preparación.");
    }

    // ---------------------------------------------------------
    // Motor de secuencia
    // ---------------------------------------------------------

    private void Mostrar(MensajePopup msg, UnityAction accionFinal = null)
    {
        if (msg.lineas == null || msg.lineas.Length == 0)
        {
            accionFinal?.Invoke();
            return;
        }

        secuenciaId++;
        MostrarLinea(msg, 0, accionFinal, secuenciaId);
    }

    private void MostrarLinea(MensajePopup msg, int index, UnityAction accionFinal, int id)
    {
        if (id != secuenciaId) return;

        if (PopupManager.Instance == null)
        {
            accionFinal?.Invoke();
            return;
        }

        bool esUltimaLinea = index >= msg.lineas.Length - 1;

        UnityAction continuar = () =>
        {
            if (id != secuenciaId) return;

            if (esUltimaLinea) accionFinal?.Invoke();
            else MostrarLinea(msg, index + 1, accionFinal, id);
        };

        // Omitir cancela toda la secuencia
        UnityAction omitir = () =>
        {
            if (id != secuenciaId) return;

            secuenciaId++;
            accionFinal?.Invoke();
        };

        PopupManager.Instance.MostrarPopup(msg.titulo, msg.lineas[index], msg.imagen, continuar, omitir);
    }

    // ---------------------------------------------------------
    // Instrucciones
    // ---------------------------------------------------------

    public void MostrarInstruccionesIngredientes(UnityAction accion = null) => Mostrar(instIngredientes, accion);
    public void MostrarInstruccionesLavadoVerduras(UnityAction accion = null) => Mostrar(instLavadoVerduras, accion);
    public void MostrarInstruccionesLavado(UnityAction accion = null) => Mostrar(instLavado, accion);
    public void MostrarInstruccionesCortado(UnityAction accion = null) => Mostrar(instCortado, accion);
    public void MostrarInstruccionesMezclado(UnityAction accion = null) => Mostrar(instMezclado, accion);
    public void MostrarInstruccionesCoccion(UnityAction accion = null) => Mostrar(instCoccion, accion);
    public void MostrarInstruccionesEmplatado(UnityAction accion = null) => Mostrar(instEmplatado, accion);

    public void MostrarInstruccionesReceta(UnityAction accion = null)
    {
        if (DayManager.Instance == null)
        {
            Debug.LogWarning("PopupContenido: DayManager.Instance es null.");
            accion?.Invoke();
            return;
        }

        var msg = new MensajePopup("Receta del día", DayManager.Instance.imagenRecetaActual,
            $"El plato del día es una {DayManager.Instance.recetaActual}.",
            "Acá está la lista de ingredientes, asegurate de que no falte ninguno.");

        Mostrar(msg, accion);
    }

    // ---------------------------------------------------------
    // Feedbacks
    // ---------------------------------------------------------

    public void MostrarFeedbackIngredientes(bool correcto, UnityAction accion = null) =>
        Mostrar(correcto ? fbIngredientesCorrectos : fbIngredientesIncorrectos, accion);

    public void MostrarFeedbackLavadoVerduras(UnityAction accion = null) => Mostrar(fbLavadoVerduras, accion);
    public void MostrarFeedbackLavado(UnityAction accion = null) => Mostrar(fbLavado, accion);

    public void MostrarFeedbackCortado(bool huboContaminacionCruzada, UnityAction accion = null) =>
        Mostrar(huboContaminacionCruzada ? fbCortadoContaminado : fbCortado, accion);

    public void MostrarFeedbackMezclado(UnityAction accion = null) => Mostrar(fbMezclado, accion);

    public void MostrarFeedbackCoccion(bool cruda, bool quemada, UnityAction accion = null) =>
        Mostrar(quemada ? fbCarneQuemada : cruda ? fbCarneCruda : fbCarneCorrecta, accion);

    public void MostrarFeedbackEmplatado(UnityAction accion = null) => Mostrar(fbEmplatado, accion);
}