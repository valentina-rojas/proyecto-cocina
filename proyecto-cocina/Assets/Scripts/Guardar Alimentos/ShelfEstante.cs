using UnityEngine;
using UnityEngine.EventSystems;

public class ShelfEstante : MonoBehaviour, IDropHandler
{
    [Header("Configuración de Superficie")]
    [Tooltip("Marcar si este mueble/superficie es la mesa inicial")]
    public bool esMesa = false;

    [Tooltip("Categoría que acepta este estante (se ignora si es mesa)")]
    public TipoAlimento tipoAceptado;

    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag == null) return;

        DraggableItem item = eventData.pointerDrag.GetComponent<DraggableItem>();

        if (item == null)
            return;

        // El item se coloca siempre en este slot
        item.parentAfterDrag = transform;

        // Si es la mesa, no evaluamos acierto ni error
        if (esMesa)
            return;

        IngredienteData data = item.GetComponent<IngredienteData>();

        if (data == null)
            return;

        // Comparación corregida usando 'tipoAceptado'
        if (data.tipo == tipoAceptado)
        {
            Debug.Log("Correcto");
        }
        else
        {
            Debug.Log("Incorrecto");
        }
    }
}