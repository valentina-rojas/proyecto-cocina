using UnityEngine;

public class SlicePoint : MonoBehaviour
{
    private CuttableIngredient ingrediente;

    private bool iniciado;
    private bool cortado;

    public void Inicializar(CuttableIngredient ingrediente)
    {
        this.ingrediente = ingrediente;

        iniciado = false;
        cortado = false;
    }

    public void EmpezarCorte()
    {
        if (cortado)
            return;

        iniciado = true;
    }

    public void TerminarCorte()
    {
        if (cortado)
            return;

        if (!iniciado)
            return;

        cortado = true;

        ingrediente.RealizarCorte();
    }
}