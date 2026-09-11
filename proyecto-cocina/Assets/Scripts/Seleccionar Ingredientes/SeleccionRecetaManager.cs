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

        // Extrae los ingredientes necesarios directamente de los slots en escena
        List<IngredienteData> ingredientesNecesarios = slots
            .SelectMany(s => s.GetComponentsInChildren<IngredienteData>())
            .Where(ing => PerteneceAReceta(ing, recetaActual))
            .ToList();

        RecetaUIManager.Instance?.MostrarReceta(ingredientesNecesarios);

        ChequearReceta();
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
                    incorrectosEnMesa++;
                }
            }
        }

        RecetaUIManager.Instance?.ActualizarLista();

        // Todos los necesarios presentes y ningún elemento erróneo en mesa
        if (necesarios > 0 && correctosEnMesa == necesarios && incorrectosEnMesa == 0)
        {
            seleccionActiva = false;
            GameManager.Instance?.SeleccionRecetaCompleta();
        }
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