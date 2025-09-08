using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DraggableText : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Vector2 originalPosition;
    private Transform originalParent;

    private Vector3 originalScale;
    private Image backgroundImage;
    private Color originalColor;
    public Color highlightColor = new Color(1f, 1f, 1f, 0.8f);

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        originalPosition = rectTransform.anchoredPosition;
        originalParent = transform.parent;

        originalScale = rectTransform.localScale;

        backgroundImage = GetComponent<Image>();
        if (backgroundImage != null)
            originalColor = backgroundImage.color;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = false;

        // Highlight visual
        rectTransform.localScale = originalScale * 1.1f;
        if (backgroundImage != null)
            backgroundImage.color = highlightColor;
    }

    public void OnDrag(PointerEventData eventData)
    {
        // Move o centro do objeto até a posição exata do mouse
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            (RectTransform)rectTransform.parent, eventData.position, eventData.pressEventCamera, out Vector2 localPoint))
        {
            rectTransform.anchoredPosition = localPoint;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;

        // Volta visual ao normal
        rectTransform.localScale = originalScale;
        if (backgroundImage != null)
            backgroundImage.color = originalColor;
    }

    public void ResetPosition()
    {
        rectTransform.anchoredPosition = originalPosition;
        transform.SetParent(originalParent);
    }
}
