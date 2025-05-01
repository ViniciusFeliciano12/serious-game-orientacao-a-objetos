using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class DropZone : MonoBehaviour, IDropHandler
{
    public string acceptedWord;
    private Image backgroundImage;

    public Color correctColor = new(0f, 0f, 0f, 0f); // transparente
    private Color originalColor;
    public float transitionDuration = 0.5f; // duração da transição em segundos

    private void Start()
    {
        if (backgroundImage == null){
            LoadBackground();
        }
    }

    public void LoadBackground(){
        // Aqui, garantimos que o backgroundImage esteja pronto para ser acessado
        backgroundImage = GetComponent<Image>();
        if (backgroundImage != null)
        {
            originalColor = backgroundImage.color;
            Debug.Log("Original color: " + originalColor.ToString());
        }
        else
        {
            Debug.LogError("BackgroundImage não encontrado!");
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

                SetCorrectWord(droppedObj);
            }
            else
            {
                Debug.Log($"Palavra incorreta: {text.text}");
                droppedObj.GetComponent<DraggableText>().ResetPosition();
            }
        }
    }

    public void SetCorrectWord(GameObject wordObject)
    {
        wordObject.transform.SetParent(transform);
        wordObject.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;

        var drag = wordObject.GetComponent<DraggableText>();
        if (drag != null)
        {
            drag.enabled = false;
            drag.GetComponent<Image>().color = correctColor;
        }

        LoadBackground();
        backgroundImage.color = correctColor;

        LearnNewRecipeMinigameController.Instance.VerifyComplete();
    }
}
