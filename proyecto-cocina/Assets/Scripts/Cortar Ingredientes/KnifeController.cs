using UnityEngine;
using UnityEngine.EventSystems;

public class KnifeController : MonoBehaviour, IDragHandler
{
    private RectTransform rect;

    private void Awake()
    {
        rect = GetComponent<RectTransform>();
    }

    public void OnDrag(PointerEventData eventData)
    {
        rect.position = eventData.position;
    }
}