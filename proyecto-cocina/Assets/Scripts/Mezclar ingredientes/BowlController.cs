using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class BowlController : MonoBehaviour, IDropHandler, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    [Header("Referencias Visuales")]
    [SerializeField] private Image bowlImage;
    [SerializeField] private Slider mixProgressBar;
    [SerializeField] private Image handsImage;

    [Header("Frames de las Manos")]
    [Tooltip("El frame 0 es la pose de reposo. Del 1 en adelante son las fases de la mezcla.")]
    [SerializeField] private Sprite[] handFrames;

    [Header("Configuración de Mezcla")]
    [SerializeField] private float turnsRequired = 3f;
    [SerializeField] private float degreesPerFrame = 30f;

    [Header("Gestor de Flujo")]
    [Tooltip("Referencia al MezcladoManager de la escena")]
    [SerializeField] private MezcladoManager mezcladoManager;

    private RectTransform rectTransform;
    private Canvas rootCanvas;
    private bool isMixingPhase = false;
    private bool isPointerDown = false;
    private Vector2 lastDirection;

    private float totalAccumulatedAngle = 0f;
    private float cycleAccumulatedAngle = 0f;
    private int currentFrameIndex = 0;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        rootCanvas = GetComponentInParent<Canvas>();

        if (mixProgressBar != null) mixProgressBar.gameObject.SetActive(false);
        if (handsImage != null) handsImage.gameObject.SetActive(false);
    }

    public void OnDrop(PointerEventData eventData)
    {
        IngredienteMezclable ingredient = eventData.pointerDrag?.GetComponent<IngredienteMezclable>();
        if (ingredient != null && !isMixingPhase)
        {
            if (bowlImage != null && ingredient.bowlStateSprite != null)
            {
                bowlImage.sprite = ingredient.bowlStateSprite;
            }

            ingredient.Consume();
            StartMixingPhase();
        }
    }

    private void StartMixingPhase()
    {
        isMixingPhase = true;
        totalAccumulatedAngle = 0f;
        cycleAccumulatedAngle = 0f;

        if (mixProgressBar != null)
        {
            mixProgressBar.value = 0f;
            mixProgressBar.gameObject.SetActive(true);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!isMixingPhase) return;
        isPointerDown = true;

        Vector2 centerScreenPos = GetCenterScreenPosition();
        lastDirection = (eventData.position - centerScreenPos).normalized;

        if (handsImage != null)
        {
            handsImage.gameObject.SetActive(true);
            SetFrame(0);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isMixingPhase || !isPointerDown) return;

        Vector2 centerScreenPos = GetCenterScreenPosition();
        Vector2 currentDir = (eventData.position - centerScreenPos).normalized;
        float angleDelta = Vector2.SignedAngle(lastDirection, currentDir);

        if (Mathf.Abs(angleDelta) > 0.5f && Mathf.Abs(angleDelta) < 90f)
        {
            float absAngle = Mathf.Abs(angleDelta);

            // 1. Progreso acumulado
            totalAccumulatedAngle += absAngle;
            float progress = totalAccumulatedAngle / (turnsRequired * 360f);
            if (mixProgressBar != null) mixProgressBar.value = Mathf.Clamp01(progress);

            // 2. Avance de frames
            cycleAccumulatedAngle += absAngle;
            if (cycleAccumulatedAngle >= degreesPerFrame)
            {
                int steps = Mathf.FloorToInt(cycleAccumulatedAngle / degreesPerFrame);
                cycleAccumulatedAngle %= degreesPerFrame;
                AdvanceFrame(steps);
            }

            // 3. Finalización
            if (progress >= 1f)
            {
                CompleteMixing();
                return;
            }
        }

        lastDirection = currentDir;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!isMixingPhase) return;
        isPointerDown = false;
        SetFrame(0);
    }

    private void AdvanceFrame(int stepCount)
    {
        if (handFrames == null || handFrames.Length <= 1) return;

        int loopCount = handFrames.Length - 1;
        currentFrameIndex = 1 + ((currentFrameIndex - 1 + stepCount) % loopCount);
        SetFrame(currentFrameIndex);
    }

    private void SetFrame(int index)
    {
        if (handsImage != null && handFrames != null && index >= 0 && index < handFrames.Length)
        {
            currentFrameIndex = index;
            handsImage.sprite = handFrames[index];
        }
    }

    private void CompleteMixing()
    {
        isMixingPhase = false;
        isPointerDown = false;

        if (mixProgressBar != null) mixProgressBar.gameObject.SetActive(false);
        if (handsImage != null) handsImage.gameObject.SetActive(false);

        // Avisa al MezcladoManager que se completó este paso
        if (mezcladoManager != null)
        {
            mezcladoManager.OnMezclaCompletada();
        }
    }

    private Vector2 GetCenterScreenPosition()
    {
        Camera cam = (rootCanvas != null && rootCanvas.renderMode != RenderMode.ScreenSpaceOverlay) 
            ? rootCanvas.worldCamera 
            : null;

        return RectTransformUtility.WorldToScreenPoint(cam, rectTransform.position);
    }
}