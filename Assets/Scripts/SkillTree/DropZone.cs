using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class DropZone : MonoBehaviour, IDropHandler
{
    public string acceptedWord;
    private Image backgroundImage;

    public Color correctColor = new Color(0.6f, 1f, 0.6f); // verde claro
    private Color originalColor;
    public float transitionDuration = 0.5f; // duração da transição em segundos

    private void Awake()
    {
        backgroundImage = GetComponent<Image>();
        if (backgroundImage != null)
        {
            originalColor = backgroundImage.color;
            
            Debug.Log("Original color:"+ originalColor.ToString());
        }else{
            Debug.Log("BackgroundImage not found");
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        var droppedObj = eventData.pointerDrag;
        if (droppedObj != null)
        {
            var text = droppedObj.transform.Find("Palavra").GetComponent<TextMeshProUGUI>();
            if (text != null && text.text == acceptedWord)
            {
                Debug.Log("Palavra correta posicionada!");

                droppedObj.transform.SetParent(transform);
                droppedObj.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;

                var drag = droppedObj.GetComponent<DraggableText>();
                if (drag != null) drag.enabled = false;

                if (backgroundImage != null)
                    backgroundImage.color = correctColor;
            }
            else
            {
                Debug.Log($"Palavra incorreta: {text.text}");
                droppedObj.GetComponent<DraggableText>().ResetPosition();
            }
        }
    }
}
