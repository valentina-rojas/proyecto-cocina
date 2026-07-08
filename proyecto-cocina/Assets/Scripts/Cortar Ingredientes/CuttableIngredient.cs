using UnityEngine;
using UnityEngine.UI;

public class CuttableIngredient : MonoBehaviour
{
    public Image imagen;
    public Sprite[] estados;

    private int corteActual = 0;

    public void RealizarCorte()
    {
        corteActual++;

        if (corteActual < estados.Length)
        {
            imagen.sprite = estados[corteActual];
        }
    }
}