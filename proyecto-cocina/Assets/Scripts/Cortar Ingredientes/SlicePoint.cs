using UnityEngine;

public class SlicePoint : MonoBehaviour
{
    public CuttableIngredient ingrediente;
    public bool cortado = false;

    public void Cortar()
    {
        if (cortado)
            return;

        cortado = true;

        ingrediente.RealizarCorte();
        CortadoManager.Instance.RegistrarCorte();
    }
}