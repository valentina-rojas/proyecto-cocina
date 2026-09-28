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
        // Esperamos hasta que existan los InventorySlot
        yield return new WaitUntil(() =>
            FindObjectsByType<InventorySlot>(
                FindObjectsSortMode.None
            ).Length > 0
        );

        ActualizarSlots();
    }

    private void OnEnable()
    {
        DraggableItem.OnAnyItemEndDrag += ChequearReceta;
    }

    private void OnDisable()
    {
        DraggableItem.OnAnyItemEndDrag -= ChequearReceta;
    }

    // =========================================================
    // ACTUALIZAR SLOTS
    // =========================================================

    private void ActualizarSlots()
    {
        slots = FindObjectsByType<InventorySlot>(
            FindObjectsSortMode.None
        );
    }

    // =========================================================
    // INICIAR SELECCIÓN
    // =========================================================

    public void IniciarSeleccion()
    {
        seleccionActiva = true;

        // Por seguridad, volvemos a buscar los slots.
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

                IngredienteData[] ingredientes =
                    slot.GetComponentsInChildren<IngredienteData>(true);

                foreach (IngredienteData ingrediente in ingredientes)
                {
                    if (ingrediente != null &&
                        PerteneceAReceta(ingrediente, recetaActual))
                    {
                        ingredientesNecesarios.Add(ingrediente);
                    }
                }
            }
        }

        Debug.Log(
            "Ingredientes encontrados para la receta: " +
            ingredientesNecesarios.Count
        );

        if (ingredientesNecesarios.Count == 0)
        {
            Debug.LogWarning(
                "⚠️ No se encontraron ingredientes para la receta '" +
                recetaActual + "'. " +
                "Revisá nombreReceta en los IngredienteData."
            );
        }

        if (RecetaUIManager.Instance != null)
        {
            RecetaUIManager.Instance.MostrarReceta(
                ingredientesNecesarios
            );
        }
        else
        {
            Debug.LogWarning(
                "⚠️ No existe RecetaUIManager en la escena."
            );
        }

        ChequearReceta();
    }

    // =========================================================
    // FINALIZAR SELECCIÓN
    // =========================================================

    public void FinalizarSeleccion()
    {
        seleccionActiva = false;
    }

    // =========================================================
    // COMPROBAR RECETA
    // =========================================================

    public void ChequearReceta()
    {
        if (!seleccionActiva)
            return;

        // Por seguridad, si todavía no tenemos slots los buscamos.
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

            IngredienteData[] ingredientes =
                slot.GetComponentsInChildren<IngredienteData>(true);

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
                    // Ingrediente que está en la mesa
                    // pero no pertenece a la receta.
                    incorrectosEnMesa++;
                }
            }
        }

        if (RecetaUIManager.Instance != null)
        {
            RecetaUIManager.Instance.ActualizarLista();
        }

        bool esValido =
            necesarios > 0 &&
            correctosEnMesa == necesarios &&
            incorrectosEnMesa == 0;

      /*  Debug.Log(
            $"🍔 Receta: {recetaActual} | " +
            $"Necesarios: {necesarios} | " +
            $"En mesa: {correctosEnMesa} | " +
            $"Incorrectos: {incorrectosEnMesa} | " +
            $"Válida: {esValido}"
        );*/

        GameManager.Instance?.ActualizarEstadoSeleccionReceta(
            esValido
        );
    }

    // =========================================================
    // OBTENER RECETA
    // =========================================================

    private string ObtenerRecetaActual()
    {
        if (DayManager.Instance == null)
            return string.Empty;

        if (string.IsNullOrEmpty(DayManager.Instance.recetaActual))
            return string.Empty;

        return DayManager.Instance.recetaActual
            .Trim()
            .ToLower();
    }

    // =========================================================
    // COMPROBAR SI PERTENECE A LA RECETA
    // =========================================================

    private bool PerteneceAReceta(
        IngredienteData ing,
        string receta
    )
    {
        if (ing == null)
            return false;

        if (string.IsNullOrEmpty(ing.nombreReceta))
            return false;

        return ing.nombreReceta
            .Trim()
            .Equals(
                receta,
                StringComparison.OrdinalIgnoreCase
            );
    }
}