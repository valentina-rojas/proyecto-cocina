using UnityEngine;
using UnityEngine.EventSystems;

public class PlateDropArea : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag == null)
            return;

        IngredienteEmplatado ingrediente =
            eventData.pointerDrag.GetComponent<IngredienteEmplatado>();

        if (ingrediente == null)
            return;

        bool correcto = PlateManager.Instance.IntentarAgregarIngrediente(ingrediente);

        if (!correcto)
        {
            Debug.Log("Orden incorrecto.");
        }
    }
}