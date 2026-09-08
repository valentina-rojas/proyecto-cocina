using UnityEngine;
using UnityEngine.Events;

public class PopupContenido : MonoBehaviour
{
    public static PopupContenido Instance;

    [Header("ORDENAR INGREDIENTES")]
    [TextArea(2, 5)]
    public string instruccionesIngredientes =
        "Almacená cada alimento en el lugar correspondiente.";

    [TextArea(2, 5)]
    public string feedbackIngredientesCorrectos =
        "¡Muy bien! Todos los ingredientes fueron almacenados correctamente.";

    [TextArea(2, 5)]
    public string feedbackIngredientesIncorrectos =
        "Hay ingredientes almacenados en lugares incorrectos. Recordá colocar cada alimento en el lugar correspondiente.";

    [Header("SELECCIÓN DE RECETA")]
    [TextArea(2, 5)]
    public string instruccionesReceta =
        "Elegí la receta que vas a preparar. Revisá los ingredientes necesarios antes de comenzar.";

    [Header("LAVADO DE MANOS")]
    [TextArea(2, 5)]
    public string instruccionesLavado =
        "Lavate las manos antes de comenzar a manipular los alimentos. Arrastrá el jabón sobre tus manos y frotá hasta completar la barra.";

    [TextArea(2, 5)]
    public string feedbackLavado =
        "¡Muy bien! Completaste correctamente el lavado de manos antes de manipular los alimentos.";

    [Header("CORTADO")]
    [TextArea(2, 5)]
    public string instruccionesCortado =
        "Usá el cuchillo para cortar el ingrediente siguiendo los puntos de corte. Realizá los cortes en orden, de arriba hacia abajo.";

    [TextArea(2, 5)]
    public string feedbackCortado =
        "¡Buen trabajo! Completaste correctamente el corte de los ingredientes.";

    [Header("COCCIÓN")]
    [TextArea(2, 5)]
    public string instruccionesCoccion =
        "Colocá la carne sobre la hornalla y controlá el indicador de cocción. Retirala del fuego cuando alcance el punto adecuado para evitar que quede cruda o se queme.";

    [TextArea(2, 5)]
    public string feedbackCarneCorrecta =
        "¡Excelente! La carne alcanzó el punto de cocción adecuado.";

    [TextArea(2, 5)]
    public string feedbackCarneCruda =
        "La carne quedó cruda. Recordá mantenerla en el fuego hasta alcanzar el punto de cocción adecuado.";

    [TextArea(2, 5)]
    public string feedbackCarneQuemada =
        "La carne se quemó. Retirala del fuego cuando alcance el punto de cocción adecuado.";

    [Header("EMPLATADO")]
    [TextArea(2, 5)]
    public string instruccionesEmplatado =
        "Colocá los ingredientes en el plato siguiendo el orden indicado para completar correctamente la preparación.";

    [TextArea(2, 5)]
    public string feedbackEmplatado =
        "¡Plato terminado! Completaste correctamente el proceso de preparación.";

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    //====================================================
    // MÉTODO GENERAL
    //====================================================

    private void Mostrar(
        string titulo,
        string descripcion,
        UnityAction accion = null)
    {
        if (PopupManager.Instance != null)
        {
            PopupManager.Instance.MostrarPopup(
                titulo,
                descripcion,
                accion
            );
        }
        else
        {
            Debug.LogError("PopupContenido: No existe PopupManager en la escena.");
            accion?.Invoke();
        }
    }

    //====================================================
    // INSTRUCCIONES
    //====================================================

    public void MostrarInstruccionesIngredientes(UnityAction accion = null)
    {
        Mostrar(
            "Guardado de alimentos",
            instruccionesIngredientes,
            accion
        );
    }

    public void MostrarInstruccionesReceta(UnityAction accion = null)
    {
        Mostrar(
            "Seleccioná la receta",
            instruccionesReceta,
            accion
        );
    }

    public void MostrarInstruccionesLavado(UnityAction accion = null)
    {
        Mostrar(
            "Lavado de manos",
            instruccionesLavado,
            accion
        );
    }

    public void MostrarInstruccionesCortado(UnityAction accion = null)
    {
        Mostrar(
            "Cortar los ingredientes",
            instruccionesCortado,
            accion
        );
    }

    public void MostrarInstruccionesCoccion(UnityAction accion = null)
    {
        Mostrar(
            "Cocción",
            instruccionesCoccion,
            accion
        );
    }

    public void MostrarInstruccionesEmplatado(UnityAction accion = null)
    {
        Mostrar(
            "Emplatado",
            instruccionesEmplatado,
            accion
        );
    }

    //====================================================
    // FEEDBACK
    //====================================================

    public void MostrarFeedbackIngredientes(bool correcto, UnityAction accion = null)
    {
        if (correcto)
        {
            Mostrar(
                "¡Muy bien!",
                feedbackIngredientesCorrectos,
                accion
            );
        }
        else
        {
            Mostrar(
                "Revisá la organización",
                feedbackIngredientesIncorrectos,
                accion
            );
        }
    }

    public void MostrarFeedbackLavado(UnityAction accion = null)
    {
        Mostrar(
            "¡Lavado completado!",
            feedbackLavado,
            accion
        );
    }

    public void MostrarFeedbackCortado(UnityAction accion = null)
    {
        Mostrar(
            "¡Cortado completado!",
            feedbackCortado,
            accion
        );
    }

    public void MostrarFeedbackCoccion(
        bool cruda,
        bool quemada,
        UnityAction accion = null)
    {
        if (quemada)
        {
            Mostrar(
                "Cocción incorrecta",
                feedbackCarneQuemada,
                accion
            );
        }
        else if (cruda)
        {
            Mostrar(
                "Cocción incompleta",
                feedbackCarneCruda,
                accion
            );
        }
        else
        {
            Mostrar(
                "¡Cocción perfecta!",
                feedbackCarneCorrecta,
                accion
            );
        }
    }

    public void MostrarFeedbackEmplatado(UnityAction accion = null)
    {
        Mostrar(
            "¡Plato terminado!",
            feedbackEmplatado,
            accion
        );
    }
}