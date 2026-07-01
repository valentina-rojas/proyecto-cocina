using UnityEngine;
using UnityEngine.EventSystems;

public class ShelfEstante : MonoBehaviour, IDropHandler
{
    public string tipoIngrediente;

    public void OnDrop(PointerEventData eventData)
    {
        DraggableItem item = eventData.pointerDrag.GetComponent<DraggableItem>();

        if (item == null)
            return;

        // El item se coloca siempre en este slot
        item.parentAfterDrag = transform;

        IngredienteData data = item.GetComponent<IngredienteData>();

        if (data.tipoIngrediente == tipoIngrediente)
        {
            Debug.Log("Correcto");
        }
        else
        {
            Debug.Log("Incorrecto");
        }
    }
}