using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RecetaUIManager : MonoBehaviour
{
    public static RecetaUIManager Instance { get; private set; }

    [Header("Panel receta")]
    [SerializeField] private GameObject panelReceta;

    [SerializeField] private TextMeshProUGUI textoTitulo;
    [SerializeField] private TextMeshProUGUI textoIngredientes;

    private List<IngredienteData> ingredientes =
        new List<IngredienteData>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // =========================================================
    // MOSTRAR RECETA
    // =========================================================

    public void MostrarReceta(List<IngredienteData> lista)
    {
        ingredientes = lista ?? new List<IngredienteData>();

        Debug.Log(
            "📋 Mostrando receta. Ingredientes: " +
            ingredientes.Count
        );

        if (panelReceta != null)
        {
            panelReceta.SetActive(true);
        }
        else
        {
            Debug.LogWarning(
                "⚠️ RecetaUIManager: panelReceta no está asignado."
            );
        }

        if (DayManager.Instance != null &&
            textoTitulo != null)
        {
            textoTitulo.text =
                DayManager.Instance.recetaActual;
        }

        ActualizarLista();
    }

    // =========================================================
    // ACTUALIZAR LISTA
    // =========================================================

    public void ActualizarLista()
    {
        if (textoIngredientes == null)
        {
            Debug.LogWarning(
                "⚠️ RecetaUIManager: textoIngredientes no está asignado."
            );

            return;
        }

        textoIngredientes.text = "";

        if (ingredientes == null ||
            ingredientes.Count == 0)
        {
            Debug.LogWarning(
                "⚠️ No hay ingredientes para mostrar en la receta."
            );

            return;
        }

        foreach (IngredienteData ingrediente in ingredientes)
        {
            if (ingrediente == null)
                continue;

            // Buscamos directamente el InventorySlot padre.
            InventorySlot slot =
                ingrediente.GetComponentInParent<InventorySlot>();

            bool estaEnMesa =
                slot != null &&
                slot.EsMesa;

            string nombre =
                ingrediente.ObtenerNombreVisible();

            if (estaEnMesa)
            {
                textoIngredientes.text +=
                    $"<s>{nombre}</s>\n";
            }
            else
            {
                textoIngredientes.text +=
                    $"{nombre}\n";
            }
        }
    }

    // =========================================================
    // OCULTAR RECETA
    // =========================================================

    public void OcultarReceta()
    {
        if (panelReceta != null)
        {
            panelReceta.SetActive(false);
        }
    }
}