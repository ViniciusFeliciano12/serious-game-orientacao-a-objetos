using EJETAGame;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ItemInventoryController : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public string Index;
    public Texture defaultTexture;
    private Image backgroundImage;
    private ItemDatabase actualItem;

    void Start()
    {
        ResetDefaults();
        backgroundImage = gameObject.GetComponent<Image>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        for (int i = 0; i < InventoryController.Instance.Inventory.Length; i++)
        {
            if (InventoryController.Instance.Inventory[i] == this)
            {
                InventoryController.Instance.SelectItemAt(i);
                break;
            }
        }
    }

    public ItemDatabase returnActualItem(){
        return actualItem;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (actualItem != null)
        {
            TooltipUI.Instance.ShowTooltip(actualItem, gameObject.GetComponent<RectTransform>());
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        TooltipUI.Instance.HideTooltip();
    }


    public void ResetDefaults()
    {
        TextMeshProUGUI text = gameObject.GetComponentInChildren<TextMeshProUGUI>();
        if (text != null)
        {
            text.text = Index;
        }

        RawImage icon = gameObject.GetComponentInChildren<RawImage>();
        if (icon != null)
        {
            icon.texture = defaultTexture;
        }
    }

    public void ItemSelected(bool isActive)
    {
        backgroundImage.color = isActive ? new Color(255, 255, 0, 255) : new Color(255, 255, 255, 255);

        if (isActive && actualItem != null)
        {
            InteractionText.instance.SetTextTimeout(actualItem.nome);
        }
    }

    public void InstantiateItem(ItemDatabase item)
    {
        actualItem = item;

        RawImage icon = gameObject.GetComponentInChildren<RawImage>();
        if (icon != null)
        {
            icon.texture = actualItem.icon;
        }
    }
}
