using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SoapController : MonoBehaviour,
    IBeginDragHandler,
    IDragHandler,
    IEndDragHandler
{
    public Image image;
    public RectTransform limiteArrastre;

    private Canvas canvas;
    private RectTransform rectTransform;

    private Transform parentOriginal;
    private Vector2 posicionOriginal;

    public bool EstaSiendoArrastrado { get; private set; }

    private void Awake()
    {
        canvas = GetComponentInParent<Canvas>();
        rectTransform = GetComponent<RectTransform>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        EstaSiendoArrastrado = true;

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
            Vector2 halfLimit = limiteArrastre.rect.size / 2f;
            Vector2 halfItem = rectTransform.rect.size / 2f;

            localPoint.x = Mathf.Clamp(localPoint.x,
                -halfLimit.x + halfItem.x,
                 halfLimit.x - halfItem.x);

            localPoint.y = Mathf.Clamp(localPoint.y,
                -halfLimit.y + halfItem.y,
                 halfLimit.y - halfItem.y);
        }

        rectTransform.localPosition = localPoint;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        EstaSiendoArrastrado = false;

        transform.SetParent(parentOriginal, false);
        rectTransform.anchoredPosition = posicionOriginal;

        image.raycastTarget = true;
    }
}