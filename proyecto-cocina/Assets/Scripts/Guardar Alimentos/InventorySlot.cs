using UnityEngine;
using UnityEngine.EventSystems;

public class InventorySlot : MonoBehaviour, IDropHandler
{
    [Header("Configuración del Estante")]
    public string tipoDeEstanteAceptado; 

    public void OnDrop(PointerEventData eventData)
    {
        GameObject dropped = eventData.pointerDrag;
        if (dropped == null) return;

        DraggableItem draggableItem = dropped.GetComponent<DraggableItem>();
        IngredienteData ingrediente = dropped.GetComponent<IngredienteData>();

        if (draggableItem == null || ingrediente == null)
            return;

        draggableItem.parentAfterDrag = transform;

        // Corregido: 'estadoActual' con minúscula.
        // Si NO estamos ordenando ingredientes, salimos sin validar.
        if (GameManager.Instance != null && GameManager.Instance.estadoActual != GameManager.EstadoJuego.OrdenandoIngredientes)
            return;

        // Solo validar cuando se están ordenando ingredientes
        if (ingrediente.tipoIngrediente == tipoDeEstanteAceptado)
        {
            Debug.Log($"¡Correcto! Colocaste {ingrediente.nombreIngrediente} en el estante de {tipoDeEstanteAceptado}.");
        }
        else
        {
            Debug.LogWarning($"¡Incorrecto! No puedes poner {ingrediente.nombreIngrediente} ({ingrediente.tipoIngrediente}) en el estante de {tipoDeEstanteAceptado}.");
        }
    }
}