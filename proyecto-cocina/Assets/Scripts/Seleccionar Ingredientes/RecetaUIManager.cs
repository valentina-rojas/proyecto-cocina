using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RecetaUIManager : MonoBehaviour
{
    public static RecetaUIManager Instance;

    [Header("Panel receta")]
    public GameObject panelReceta;

    public TextMeshProUGUI textoTitulo;
    public TextMeshProUGUI textoIngredientes;

    private List<IngredienteData> ingredientes = new();

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void MostrarReceta(List<IngredienteData> lista)
    {
        ingredientes = lista;

        if (panelReceta != null)
            panelReceta.SetActive(true);

        if (DayManager.Instance != null && textoTitulo != null)
            textoTitulo.text = DayManager.Instance.recetaActual;

        ActualizarLista();
    }

    public void ActualizarLista()
    {
        if (textoIngredientes == null) return;

        textoIngredientes.text = "";

        InventorySlot[] slots = FindObjectsByType<InventorySlot>(FindObjectsSortMode.None);

        foreach (IngredienteData ingrediente in ingredientes)
        {
            if (ingrediente == null) continue;

            bool estaEnMesa = false;

            foreach (InventorySlot slot in slots)
            {
                if (slot == null || !slot.EsMesa)
                    continue;

                if (ingrediente.transform.parent == slot.transform)
                {
                    estaEnMesa = true;
                    break;
                }
            }

            if (estaEnMesa)
            {
                textoIngredientes.text += $"<s>{ingrediente.nombreIngrediente}</s>\n";
            }
            else
            {
                textoIngredientes.text += $"{ingrediente.nombreIngrediente}\n";
            }
        }
    }
}