using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class KnifeController : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Canvas canvas;
    private RectTransform rectTransform;

    public Image image;

    [Header("Opcional: límite de arrastre")]
    public RectTransform limiteArrastre;

    private Transform parentOriginal;
    private Vector2 posicionOriginal;
    private bool initialized = false;

    private void Awake()
    {
        canvas = GetComponentInParent<Canvas>();
        rectTransform = GetComponent<RectTransform>();

        if (!initialized)
        {
            parentOriginal = transform.parent;
            posicionOriginal = rectTransform.anchoredPosition;
            initialized = true;
        }
    }

    public void ReiniciarCuchillo()
    {
        // 1. Restaurar jerarquía original
        if (parentOriginal != null && transform.parent != parentOriginal)
        {
            transform.SetParent(parentOriginal, false);
        }

        // 2. Restaurar posición original
        if (rectTransform != null)
        {
            rectTransform.anchoredPosition = posicionOriginal;
            rectTransform.localRotation = Quaternion.identity;
        }

        // 3. Reactivar detección de raycast para poder volver a arrastrarlo
        if (image != null)
        {
            image.raycastTarget = true;
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        transform.SetParent(canvas.transform);
        transform.SetAsLastSibling();

        if (image != null)
            image.raycastTarget = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 localPoint;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            eventData.position,
            canvas.worldCamera,
            out localPoint
        );

        if (limiteArrastre != null)
        {
            Vector2 limitSize = limiteArrastre.rect.size;
            Vector2 itemSize = rectTransform.rect.size;

            Vector2 halfLimit = limitSize / 2f;
            Vector2 halfItem = itemSize / 2f;

            float minX = -halfLimit.x + halfItem.x;
            float maxX = halfLimit.x - halfItem.x;
            float minY = -halfLimit.y + halfItem.y;
            float maxY = halfLimit.y - halfItem.y;

            localPoint.x = Mathf.Clamp(localPoint.x, minX, maxX);
            localPoint.y = Mathf.Clamp(localPoint.y, minY, maxY);
        }

        rectTransform.localPosition = localPoint;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        ReiniciarCuchillo();
    }
}