using UnityEngine;
using UnityEngine.UI;

public class IngredienteData : MonoBehaviour
{
    public string nombreIngrediente;
    public string nombreReceta;
    public string tipoIngrediente;

    [Header("Visual")]
    public Image imagenIngrediente;

    [Header("Configuración de Corte")]
    public CuttableIngredient prefabCortable; 

    [HideInInspector] public bool yaCortado;

    private void Reset()
    {
        // Autoasigna el Image si está en el mismo GameObject
        if (imagenIngrediente == null)
            imagenIngrediente = GetComponent<Image>();
    }
}