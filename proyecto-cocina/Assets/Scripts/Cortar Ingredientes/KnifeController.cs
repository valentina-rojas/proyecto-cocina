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

    private void Awake()
    {
        canvas = GetComponentInParent<Canvas>();
        rectTransform = GetComponent<RectTransform>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        parentOriginal = transform.parent;
        posicionOriginal = rectTransform.anchoredPosition;

        transform.SetParent(canvas.transform);
        transform.SetAsLastSibling();

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
        transform.SetParent(parentOriginal, false);
        rectTransform.anchoredPosition = posicionOriginal;

        image.raycastTarget = true;
    }
}