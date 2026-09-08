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

    [Header("Selección de Receta")]
    public MensajePopup instReceta = new MensajePopup("Seleccioná la receta", "Elegí la receta que vas a preparar.");

    [Header("Lavado de Manos")]
    public MensajePopup instLavado = new MensajePopup("Lavado de manos", "Lavate las manos antes de comenzar a manipular los alimentos.");
    public MensajePopup fbLavado = new MensajePopup("¡Lavado completado!", "¡Muy bien! Completaste correctamente el lavado de manos.");

    [Header("Cortado")]
    public MensajePopup instCortado = new MensajePopup("Cortar los ingredientes", "Usá el cuchillo para cortar el ingrediente siguiendo los puntos de corte.");
    public MensajePopup fbCortado = new MensajePopup("¡Buen trabajo!", "¡Buen trabajo! Completaste correctamente el corte de los ingredientes.");

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
    public void MostrarInstruccionesReceta(UnityAction accion = null)       => Mostrar(instReceta, accion);
    public void MostrarInstruccionesLavado(UnityAction accion = null)       => Mostrar(instLavado, accion);
    public void MostrarInstruccionesCortado(UnityAction accion = null)      => Mostrar(instCortado, accion);
    public void MostrarInstruccionesCoccion(UnityAction accion = null)      => Mostrar(instCoccion, accion);
    public void MostrarInstruccionesEmplatado(UnityAction accion = null)    => Mostrar(instEmplatado, accion);

    // --- Métodos de Feedbacks ---
    public void MostrarFeedbackIngredientes(bool correcto, UnityAction accion = null) => 
        Mostrar(correcto ? fbIngredientesCorrectos : fbIngredientesIncorrectos, accion);

    public void MostrarFeedbackLavado(UnityAction accion = null)  => Mostrar(fbLavado, accion);
    public void MostrarFeedbackCortado(UnityAction accion = null) => Mostrar(fbCortado, accion);

    public void MostrarFeedbackCoccion(bool cruda, bool quemada, UnityAction accion = null)
    {
        if (quemada) Mostrar(fbCarneQuemada, accion);
        else if (cruda) Mostrar(fbCarneCruda, accion);
        else Mostrar(fbCarneCorrecta, accion);
    }

    public void MostrarFeedbackEmplatado(UnityAction accion = null) => Mostrar(fbEmplatado, accion);
}