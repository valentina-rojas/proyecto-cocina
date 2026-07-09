using UnityEngine;
using UnityEngine.UI;

public class CuttableIngredient : MonoBehaviour
{
    [Header("Visual")]
    public Image imagen;
    public Sprite[] estados;

    [Header("Cortes (en orden)")]
    public SlicePoint[] slicePoints;

    private int corteActual = 0;

    private void Start()
    {
        for (int i = 0; i < slicePoints.Length; i++)
        {
            slicePoints[i].Inicializar(this);

            // Solo el primero queda activo
            slicePoints[i].gameObject.SetActive(i == 0);
        }
    }

    public void RealizarCorte()
    {
        corteActual++;

        // Cambiar sprite
        if (corteActual < estados.Length)
        {
            imagen.sprite = estados[corteActual];
        }

        // Ocultar el corte que se hizo
        slicePoints[corteActual - 1].gameObject.SetActive(false);

        // Activar el siguiente
        if (corteActual < slicePoints.Length)
        {
            slicePoints[corteActual].gameObject.SetActive(true);
        }
        else
        {
            CortadoManager.Instance.IngredienteCortado();
        }
    }
}