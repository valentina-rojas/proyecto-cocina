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
        InicializarIngrediente();
    }

    public void InicializarIngrediente()
    {
        corteActual = 0;

        // Colocar el sprite en su estado inicial (entero)
        if (estados != null && estados.Length > 0 && imagen != null)
        {
            imagen.sprite = estados[0];
        }

        // Configurar los puntos de corte: solo el primero queda activo
        if (slicePoints != null)
        {
            for (int i = 0; i < slicePoints.Length; i++)
            {
                if (slicePoints[i] != null)
                {
                    slicePoints[i].Inicializar(this);
                    slicePoints[i].gameObject.SetActive(i == 0);
                }
            }
        }
    }

    public void RealizarCorte()
    {
        corteActual++;

        // Cambiar sprite al nuevo estado
        if (estados != null && corteActual < estados.Length && imagen != null)
        {
            imagen.sprite = estados[corteActual];
        }

        // Ocultar el corte completado
        if (corteActual - 1 < slicePoints.Length && slicePoints[corteActual - 1] != null)
        {
            slicePoints[corteActual - 1].gameObject.SetActive(false);
        }

        // Activar el siguiente punto o notificar fin
        if (corteActual < slicePoints.Length)
        {
            if (slicePoints[corteActual] != null)
                slicePoints[corteActual].gameObject.SetActive(true);
        }
        else
        {
            CortadoManager.Instance?.IngredienteCortado();
        }
    }
}