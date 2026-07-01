using UnityEngine;
using UnityEngine.EventSystems;

public class InventorySlot : MonoBehaviour, IDropHandler
{
    [Header("Configuración del Estante")]
    // Ejemplo: "verdura", "carne", "lacteo", etc. (Asegúrate de escribirlo igual que en el ingrediente)
    public string tipoDeEstanteAceptado; 

    public void OnDrop(PointerEventData eventData) 
    {
        GameObject dropped = eventData.pointerDrag;
        if (dropped == null) return;

        DraggableItem draggableItem = dropped.GetComponent<DraggableItem>();
        IngredienteData ingrediente = dropped.GetComponent<IngredienteData>();

        if (draggableItem != null && ingrediente != null) 
        {
             draggableItem.parentAfterDrag = transform;

            if (ingrediente.tipoIngrediente == tipoDeEstanteAceptado) 
            {
                Debug.Log($"¡Correcto! Colocaste {ingrediente.nombreIngrediente} en el estante de {tipoDeEstanteAceptado}.");
                draggableItem.parentAfterDrag = transform;
            }
            else 
            {
                Debug.LogWarning($"¡Incorrecto! No puedes poner {ingrediente.nombreIngrediente} ({ingrediente.tipoIngrediente}) en el estante de {tipoDeEstanteAceptado}.");
                
            }
        }
    }
}