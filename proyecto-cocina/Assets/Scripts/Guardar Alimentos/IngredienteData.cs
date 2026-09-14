using UnityEngine;
using UnityEngine.UI;

public class IngredienteData : MonoBehaviour
{
    public string nombreIngrediente;
    public string nombreMostrar;
    public string nombreReceta;

    [Header("Clasificación")]
    public TipoAlimento tipo;

    [Header("Visual")]
    public Image imagenIngrediente;

    [Header("Configuración de Corte")]
    public CuttableIngredient prefabCortable; 

    [HideInInspector] public bool yaCortado;

    // Devuelve nombreMostrar si existe; si está vacío, usa nombreIngrediente como respaldo
    public string ObtenerNombreVisible()
    {
        return !string.IsNullOrWhiteSpace(nombreMostrar) ? nombreMostrar : nombreIngrediente;
    }

    private void Reset()
    {
        if (imagenIngrediente == null)
            imagenIngrediente = GetComponent<Image>();
    }
}