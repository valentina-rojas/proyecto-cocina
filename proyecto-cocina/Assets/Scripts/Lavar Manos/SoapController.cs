using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SoapController : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Canvas canvas;
    private RectTransform rectTransform;
    private Vector3 posicionInicial;
    private Transform parentOriginal;
    private Image image;

    private void Awake()
    {
        canvas = GetComponentInParent<Canvas>();
        rectTransform = GetComponent<RectTransform>();
        image = GetComponent<Image>();
    }

    private void Start()
    {
        posicionInicial = rectTransform.localPosition;
        parentOriginal = rectTransform.parent;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (image != null) image.raycastTarget = false;
        rectTransform.SetParent(canvas.transform, true);
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 localPoint;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            eventData.position,
            canvas.worldCamera,
            out localPoint))
        {
            rectTransform.localPosition = localPoint;

            // Solo enviamos el movimiento si el puntero está físicamente dentro del área
            if (AreaLavado.JugadorEstaEncima)
            {
                LavadoManos.Instance?.ProcesarFrotado(eventData.delta.magnitude);
            }
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        rectTransform.SetParent(parentOriginal, true);
        rectTransform.localPosition = posicionInicial;
        rectTransform.rotation = Quaternion.identity;

        if (image != null) image.raycastTarget = true;

        LavadoManos.Instance?.ReiniciarLavado();
    }
}