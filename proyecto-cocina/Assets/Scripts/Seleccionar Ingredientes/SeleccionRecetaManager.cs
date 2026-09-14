using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class SeleccionRecetaManager : MonoBehaviour
{
    public static SeleccionRecetaManager Instance { get; private set; }

    private InventorySlot[] slots;
    private bool seleccionActiva;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        slots = FindObjectsByType<InventorySlot>(FindObjectsSortMode.None);
    }

    private void OnEnable()  => DraggableItem.OnAnyItemEndDrag += ChequearReceta;
    private void OnDisable() => DraggableItem.OnAnyItemEndDrag -= ChequearReceta;

    // =========================================================
    // INICIAR SELECCIÓN
    // =========================================================

    public void IniciarSeleccion()
    {
        seleccionActiva = true;
        string recetaActual = ObtenerRecetaActual();

        List<IngredienteData> ingredientesNecesarios = slots
            .SelectMany(s => s.GetComponentsInChildren<IngredienteData>())
            .Where(ing => PerteneceAReceta(ing, recetaActual))
            .ToList();

        RecetaUIManager.Instance?.MostrarReceta(ingredientesNecesarios);

        ChequearReceta();
    }

    public void FinalizarSeleccion()
    {
        seleccionActiva = false;
    }

    // =========================================================
    // COMPROBACIÓN DE INGREDIENTES EN MESA
    // =========================================================

    public void ChequearReceta()
    {
        if (!seleccionActiva || slots == null) return;

        string recetaActual = ObtenerRecetaActual();

        int necesarios = 0;
        int correctosEnMesa = 0;
        int incorrectosEnMesa = 0;

        foreach (InventorySlot slot in slots)
        {
            if (slot == null) continue;

            bool esMesa = slot.EsMesa;
            IngredienteData[] ingredientes = slot.GetComponentsInChildren<IngredienteData>();

            foreach (IngredienteData ing in ingredientes)
            {
                if (PerteneceAReceta(ing, recetaActual))
                {
                    necesarios++;
                    if (esMesa) correctosEnMesa++;
                }
                else if (esMesa)
                {
                    // Ingrediente en la mesa que no pertenece a la receta del día
                    incorrectosEnMesa++;
                }
            }
        }

        RecetaUIManager.Instance?.ActualizarLista();

        // Válido únicamente si están todos los requeridos en mesa y no hay sobrantes
        bool esValido = (necesarios > 0 && correctosEnMesa == necesarios && incorrectosEnMesa == 0);

        GameManager.Instance?.ActualizarEstadoSeleccionReceta(esValido);
    }

    // =========================================================
    // HELPERS
    // =========================================================

    private string ObtenerRecetaActual()
    {
        return DayManager.Instance != null 
            ? DayManager.Instance.recetaActual.Trim().ToLower() 
            : string.Empty;
    }

    private bool PerteneceAReceta(IngredienteData ing, string receta)
    {
        return ing != null && ing.nombreReceta.Trim().Equals(receta, StringComparison.OrdinalIgnoreCase);
    }
}