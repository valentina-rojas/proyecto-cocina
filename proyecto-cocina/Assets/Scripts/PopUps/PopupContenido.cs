using System;
using UnityEngine;
using UnityEngine.Events;

public class PopupContenido : MonoBehaviour
{
    public static PopupContenido Instance { get; private set; }

    [Serializable]
    public struct MensajePopup
    {
        public string titulo;
        [TextArea(2, 4)] public string texto;
        public Sprite imagen;

        public MensajePopup(string titulo, string texto, Sprite imagen = null)
        {
            this.titulo = titulo;
            this.texto = texto;
            this.imagen = imagen;
        }
    }

    [Header("Guardado de Alimentos")]
    public MensajePopup instIngredientes = new MensajePopup("Guardado de alimentos", "Almacená cada alimento en el lugar correspondiente.");
    public MensajePopup fbIngredientesCorrectos = new MensajePopup("¡Muy bien!", "¡Muy bien! Todos los ingredientes fueron almacenados correctamente.");
    public MensajePopup fbIngredientesIncorrectos = new MensajePopup("Revisá la organización", "Hay ingredientes almacenados en lugares incorrectos.");

    [Header("Lavado de Manos")]
    public MensajePopup instLavado = new MensajePopup("Lavado de manos", "Lavate las manos antes de comenzar a manipular los alimentos.");
    public MensajePopup fbLavado = new MensajePopup("¡Lavado completado!", "¡Muy bien! Completaste correctamente el lavado de manos.");

    [Header("Cortado")]
    public MensajePopup instCortado = new MensajePopup("Cortar los ingredientes", "Usá el cuchillo para cortar el ingrediente siguiendo los puntos de corte.");
    public MensajePopup fbCortado = new MensajePopup("¡Buen trabajo!", "¡Buen trabajo! Completaste correctamente el corte de los ingredientes.");
    public MensajePopup fbCortadoContaminado = new MensajePopup("¡Cuidado!", "Se produjo contaminación cruzada durante el cortado de los alimentos.");

    [Header("Cocción")]
    public MensajePopup instCoccion = new MensajePopup("Cocción", "Colocá la carne sobre la hornalla y controlá el indicador de cocción.");
    public MensajePopup fbCarneCorrecta = new MensajePopup("¡Cocción perfecta!", "¡Excelente! La carne alcanzó el punto de cocción adecuado.");
    public MensajePopup fbCarneCruda = new MensajePopup("Cocción incompleta", "La carne quedó cruda.");
    public MensajePopup fbCarneQuemada = new MensajePopup("Cocción incorrecta", "La carne se quemó.");

    [Header("Emplatado")]
    public MensajePopup instEmplatado = new MensajePopup("Emplatado", "Colocá los ingredientes en el plato siguiendo el orden indicado.");
    public MensajePopup fbEmplatado = new MensajePopup("¡Plato terminado!", "¡Plato terminado! Completaste correctamente la preparación.");

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Mostrar(MensajePopup msg, UnityAction accion = null)
    {
        if (PopupManager.Instance != null)
            PopupManager.Instance.MostrarPopup(msg.titulo, msg.texto, msg.imagen, accion);
        else
            accion?.Invoke();
    }

    // --- Métodos de Instrucciones ---
    public void MostrarInstruccionesIngredientes(UnityAction accion = null) => Mostrar(instIngredientes, accion);

    // Instrucción dinámica de la Receta generada desde el DayManager
    public void MostrarInstruccionesReceta(UnityAction accion = null)
    {
        if (DayManager.Instance != null)
        {
            string titulo = "Receta del día";
            Sprite imagen = DayManager.Instance.imagenRecetaActual;

            // Diálogo 1: Presenta el plato
            string textoPaso1 = $"El plato del día es: {DayManager.Instance.recetaActual}.";

            // Diálogo 2: Instrucción sobre los ingredientes
            string textoPaso2 = "Acá está la lista de ingredientes, asegurate de que no falte ninguno.";

            if (PopupManager.Instance != null)
            {
                // Muestra el primer mensaje; al hacer clic, abre el segundo mensaje
                PopupManager.Instance.MostrarPopup(titulo, textoPaso1, imagen, () =>
                {
                    PopupManager.Instance.MostrarPopup(titulo, textoPaso2, imagen, accion);
                });
            }
            else
            {
                accion?.Invoke();
            }
        }
        else
        {
            Debug.LogWarning("PopupContenido: DayManager.Instance es null.");
            accion?.Invoke();
        }
    }

    public void MostrarInstruccionesLavado(UnityAction accion = null)       => Mostrar(instLavado, accion);
    public void MostrarInstruccionesCortado(UnityAction accion = null)      => Mostrar(instCortado, accion);
    public void MostrarInstruccionesCoccion(UnityAction accion = null)      => Mostrar(instCoccion, accion);
    public void MostrarInstruccionesEmplatado(UnityAction accion = null)    => Mostrar(instEmplatado, accion);

    // --- Métodos de Feedbacks ---
    public void MostrarFeedbackIngredientes(bool correcto, UnityAction accion = null) => 
        Mostrar(correcto ? fbIngredientesCorrectos : fbIngredientesIncorrectos, accion);

    public void MostrarFeedbackLavado(UnityAction accion = null)  => Mostrar(fbLavado, accion);

    public void MostrarFeedbackCortado(bool huboContaminacionCruzada, UnityAction accion = null)
    {
        Mostrar(huboContaminacionCruzada ? fbCortadoContaminado : fbCortado, accion);
    }

    public void MostrarFeedbackCoccion(bool cruda, bool quemada, UnityAction accion = null)
    {
        if (quemada) Mostrar(fbCarneQuemada, accion);
        else if (cruda) Mostrar(fbCarneCruda, accion);
        else Mostrar(fbCarneCorrecta, accion);
    }

    public void MostrarFeedbackEmplatado(UnityAction accion = null) => Mostrar(fbEmplatado, accion);
}