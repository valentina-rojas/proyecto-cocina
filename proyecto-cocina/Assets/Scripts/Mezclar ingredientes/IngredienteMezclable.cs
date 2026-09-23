using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(CanvasGroup))]
[RequireComponent(typeof(Image))]
public class IngredienteMezclable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("Configuración del Ingrediente")]
    public string ingredientId;

    [Tooltip("El sprite que se ve en la mesa/pantalla")]
    [SerializeField] private Sprite itemSprite;

    [Tooltip("El sprite que tomará el bowl al soltar este ingrediente")]
    [SerializeField] private Sprite _bowlStateSprite;

    // Propiedad pública que lee BowlController
    public Sprite bowlStateSprite => _bowlStateSprite;

    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Image ingredientImage;
    private Canvas rootCanvas;
    private Vector2 originalAnchoredPosition;
    private Transform originalParent;
    private bool wasConsumed = false;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        ingredientImage = GetComponent<Image>();
        rootCanvas = GetComponentInParent<Canvas>();

        originalAnchoredPosition = rectTransform.anchoredPosition;
        originalParent = transform.parent;

        ActualizarVisual();
    }

    public void ActualizarVisual()
    {
        if (ingredientImage != null && itemSprite != null)
        {
            ingredientImage.sprite = itemSprite;
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        wasConsumed = false;
        canvasGroup.blocksRaycasts = false; // Permite que el bowl detecte el OnDrop

        if (rootCanvas != null)
        {
            transform.SetParent(rootCanvas.transform, true);
        }
        transform.SetAsLastSibling();
    }

    public void OnDrag(PointerEventData eventData)
    {
        Camera cam = (rootCanvas != null && rootCanvas.renderMode == RenderMode.ScreenSpaceOverlay)
            ? null
            : eventData.pressEventCamera;

        if (RectTransformUtility.ScreenPointToWorldPointInRectangle(rectTransform, eventData.position, cam, out Vector3 worldPoint))
        {
            rectTransform.position = worldPoint;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;

        // Si no cayó dentro del bowl, regresa a su posición y padre originales
        if (!wasConsumed)
        {
            transform.SetParent(originalParent, true);
            rectTransform.anchoredPosition = originalAnchoredPosition;
        }
    }

    public void Consume()
    {
        wasConsumed = true;
        gameObject.SetActive(false); // O Destroy(gameObject); si prefieres eliminarlo
    }

    #if UNITY_EDITOR
    // Actualiza la imagen automáticamente en la vista de escena al cambiar el sprite en el inspector
    private void OnValidate()
    {
        if (ingredientImage == null) 
            ingredientImage = GetComponent<Image>();
            
        ActualizarVisual();
    }
    #endif
}