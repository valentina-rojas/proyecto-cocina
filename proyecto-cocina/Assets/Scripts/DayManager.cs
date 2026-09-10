using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DayManager : MonoBehaviour
{
    public static DayManager Instance;

    [System.Serializable]
    public class Dia
    {
        public string nombreReceta;
        public Sprite imagenReceta;
    }

    [Header("Recetas de cada día")]
    public Dia[] dias;

    [Header("Catálogo de Ingredientes")]
    [Tooltip("Arrastra aquí los prefabs/objetos que tienen IngredienteData")]
    [SerializeField] private List<IngredienteData> catalogoIngredientes = new List<IngredienteData>();

    public string recetaActual { get; private set; }
    public Sprite imagenRecetaActual { get; private set; }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        if (dias != null && dias.Length > 0)
        {
            recetaActual = dias[0].nombreReceta;
            imagenRecetaActual = dias[0].imagenReceta;
        }
        else
        {
            recetaActual = "";
            imagenRecetaActual = null;
            Debug.LogWarning("No hay recetas configuradas en DayManager.");
        }
    }

    /// <summary>
    /// Devuelve únicamente los ingredientes de la receta del día que tengan prefabCortable asignado.
    /// </summary>
    public List<IngredienteData> ObtenerIngredientesCortablesDelDia()
    {
        List<IngredienteData> cortables = new List<IngredienteData>();

        if (string.IsNullOrEmpty(recetaActual)) return cortables;

        foreach (var ing in catalogoIngredientes)
        {
            if (ing == null) continue;

            // Compara ignorando mayúsculas/minúsculas y espacios sobrantes
            bool coincideReceta = ing.nombreReceta.Trim().Equals(recetaActual.Trim(), System.StringComparison.OrdinalIgnoreCase);
            bool tieneCorte = ing.prefabCortable != null;

            if (coincideReceta && tieneCorte)
            {
                cortables.Add(ing);
            }
        }

        return cortables;
    }
}