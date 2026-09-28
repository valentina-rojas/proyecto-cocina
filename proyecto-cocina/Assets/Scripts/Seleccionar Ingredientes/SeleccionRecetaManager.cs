using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SeleccionRecetaManager : MonoBehaviour
{
    public static SeleccionRecetaManager Instance { get; private set; }

    private InventorySlot[] slots;
    private bool seleccionActiva;

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

    private IEnumerator Start()
    {
        yield return new WaitUntil(() =>
            FindObjectsByType<InventorySlot>(FindObjectsSortMode.None).Length > 0
        );

        ActualizarSlots();
    }

    private void OnEnable()
    {
        DraggableItem.OnAnyItemEndDrag += OnItemMoved;
    }

    private void OnDisable()
    {
        DraggableItem.OnAnyItemEndDrag -= OnItemMoved;
    }

    private void OnItemMoved()
    {
        if (!seleccionActiva)
            return;

        StartCoroutine(ChequearDespuesDeUnFrame());
    }

    private IEnumerator ChequearDespuesDeUnFrame()
    {
        yield return null;
        ChequearReceta();
    }

    public void ActualizarSlots()
    {
        slots = FindObjectsByType<InventorySlot>(FindObjectsSortMode.None);
    }

    public void IniciarSeleccion()
    {
        seleccionActiva = true;
        ActualizarSlots();

        string recetaActual = ObtenerRecetaActual();
        Debug.Log("Iniciando selección de receta: " + recetaActual);

        if (string.IsNullOrEmpty(recetaActual))
        {
            Debug.LogWarning("⚠️ No hay una receta actual configurada.");
            return;
        }

        List<IngredienteData> ingredientesNecesarios = new List<IngredienteData>();

        if (slots != null)
        {
            foreach (InventorySlot slot in slots)
            {
                if (slot == null)
                    continue;

                IngredienteData[] ingredientes = slot.GetComponentsInChildren<IngredienteData>(true);

                foreach (IngredienteData ingrediente in ingredientes)
                {
                    if (ingrediente != null && PerteneceAReceta(ingrediente, recetaActual))
                    {
                        ingredientesNecesarios.Add(ingrediente);
                    }
                }
            }
        }

        Debug.Log("Ingredientes encontrados para la receta: " + ingredientesNecesarios.Count);

        if (RecetaUIManager.Instance != null)
        {
            RecetaUIManager.Instance.MostrarReceta(ingredientesNecesarios);
        }

        ChequearReceta();
    }

    public void FinalizarSeleccion()
    {
        seleccionActiva = false;
    }

    public void ChequearReceta()
    {
        if (!seleccionActiva)
            return;

        if (slots == null || slots.Length == 0)
        {
            ActualizarSlots();
            if (slots == null || slots.Length == 0)
                return;
        }

        string recetaActual = ObtenerRecetaActual();
        if (string.IsNullOrEmpty(recetaActual))
            return;

        int necesarios = 0;
        int correctosEnMesa = 0;
        int incorrectosEnMesa = 0;

        foreach (InventorySlot slot in slots)
        {
            if (slot == null)
                continue;

            bool esMesa = slot.EsMesa;
            IngredienteData[] ingredientes = slot.GetComponentsInChildren<IngredienteData>(true);

            foreach (IngredienteData ing in ingredientes)
            {
                if (ing == null)
                    continue;

                if (PerteneceAReceta(ing, recetaActual))
                {
                    necesarios++;
                    if (esMesa)
                    {
                        correctosEnMesa++;
                    }
                }
                else if (esMesa)
                {
                    // Hay un ingrediente en la mesa que no pertenece a esta receta
                    incorrectosEnMesa++;
                }
            }
        }

        if (RecetaUIManager.Instance != null)
        {
            RecetaUIManager.Instance.ActualizarLista();
        }

        // Condición estricta:
        // 1. Existen ingredientes necesarios.
        // 2. Todos los ingredientes requeridos están en la mesa.
        // 3. No hay ningún ingrediente ajeno/extra en la mesa.
        bool esValido = necesarios > 0 && correctosEnMesa == necesarios && incorrectosEnMesa == 0;

        GameManager.Instance?.ActualizarEstadoSeleccionReceta(esValido);
    }

    private string ObtenerRecetaActual()
    {
        if (DayManager.Instance == null || string.IsNullOrEmpty(DayManager.Instance.recetaActual))
            return string.Empty;

        return DayManager.Instance.recetaActual.Trim().ToLower();
    }

    private bool PerteneceAReceta(IngredienteData ing, string receta)
    {
        if (ing == null || string.IsNullOrEmpty(ing.nombreReceta))
            return false;

        return ing.nombreReceta.Trim().Equals(receta, StringComparison.OrdinalIgnoreCase);
    }
}