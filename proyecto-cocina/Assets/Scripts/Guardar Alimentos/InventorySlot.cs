using UnityEngine;
using UnityEngine.EventSystems;

public class InventorySlot : MonoBehaviour, IDropHandler
{
    private ShelfEstante shelfPadre;

    private void Awake()
    {
        shelfPadre = GetComponentInParent<ShelfEstante>();
    }

    public bool EsMesa => shelfPadre != null && shelfPadre.esMesa;

    public TipoAlimento TipoAceptado => shelfPadre != null ? shelfPadre.tipoAceptado : default;

    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag == null) return;

        DraggableItem draggableItem = eventData.pointerDrag.GetComponent<DraggableItem>();
        IngredienteData ingrediente = eventData.pointerDrag.GetComponent<IngredienteData>();

        if (draggableItem == null || ingrediente == null)
            return;

        // Evitar que dos elementos ocupen el mismo slot (las mesas pueden permitir múltiples si no usan slots individuales)
        if (!EsMesa && transform.childCount > 0)
            return;

        draggableItem.parentAfterDrag = transform;

        // Si no estamos ordenando ingredientes, se omite la validación de feedback
        if (GameManager.Instance != null && GameManager.Instance.estadoActual != GameManager.EstadoJuego.OrdenandoIngredientes)
            return;

        // Si se deposita en una mesa, no genera acierto ni error
        if (EsMesa)
            return;

        // Validación contra el tipo configurado en el estante padre
        if (ingrediente.tipo == TipoAceptado)
        {
            Debug.Log($"¡Correcto! Colocaste {ingrediente.nombreIngrediente} en el estante de {TipoAceptado}.");
        }
        else
        {
            Debug.LogWarning($"¡Incorrecto! No puedes poner {ingrediente.nombreIngrediente} ({ingrediente.tipo}) en el estante de {TipoAceptado}.");
        }
    }
}